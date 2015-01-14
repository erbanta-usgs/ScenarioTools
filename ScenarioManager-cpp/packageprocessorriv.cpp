#include "packageprocessorriv.h"
#include "modelelementdis.h"
#include "polyline2dset.h"
#include "polygon2dset.h"
#include "orthogrid.h"
#include "timeseries.h"
#include "timeseriessample.h"
#include "polyline2d.h"
#include "attributekeys.h"
#include "dateutil.h"
#include "stressperiod.h"

#include <QDebug>
#include <QFile>
#include <vector>
#include <iostream>

#define HEADER_LINE "PARAMETER  0         0"

using namespace std;

PackageProcessorRIV::PackageProcessorRIV(QString shapefilePath, QString sampleFilePath, ModelElementDIS *discretization, QString idKey, QString widthKey,
                                         QString conductivityKey, QString flowLengthKey, QString riverBottomKey, QString clippedShapefilePath, QString riverPackagePath)
{
    // Store the configuration.
    _shapefilePath = shapefilePath;
    _sampleFilePath = sampleFilePath;
    _discretization = discretization;
    _idKey = idKey;
    _widthKey = widthKey;
    _conductivityKey = conductivityKey;
    _flowLengthKey = flowLengthKey;
    _riverBottomKey = riverBottomKey;
    _clippedShapefilePath = clippedShapefilePath;
    _riverPackagePath = riverPackagePath;
}

void PackageProcessorRIV::process()
{
    QDateTime start = QDateTime::currentDateTime();

    // Make a polyline set and initialize it from the shapefile.
    Polyline2DSet *canals = new Polyline2DSet();
    canals->initFromShapefile(_shapefilePath);

    // Compute the conductance ratio for each canal. This is the factor that, when multiplied by the length, yields the conductance.
    // conductance = ((length * width * k) / dl)
    for (int i = 0; i < canals->numPolylines(); i++) {
        Polyline2D *canal = canals->polyline(i);
        double width = atof(canal->getAttribute(_widthKey).toAscii());
        double k = atof(canal->getAttribute(_conductivityKey).toAscii());
        double flowLength = atof(canal->getAttribute(_flowLengthKey).toAscii());
        double conductanceRatio = width * k / flowLength;
        canal->setDoubleAttribute(CONDUCTANCE_RATIO_KEY, conductanceRatio);
    }
    qDebug() << "calculated conductance ratios";

    // Get the time series.
    qDebug() << "going to read SMP file " << _sampleFilePath;
    vector<TimeSeries *> *timeSeries = TimeSeries::fromSMPFile(_sampleFilePath);
    qDebug() << "got " << timeSeries->size() << " series from SMP file";

    // Make the mapping from time series identifier to time series index.
    QMap<QString, int> timeSeriesKeyToIndexMapping;
    for (uint i = 0; i < timeSeries->size(); i++) {
        QString key = timeSeries->at(i)->name();
        qDebug() << "adding key " << key << " to mapping";
        timeSeriesKeyToIndexMapping.insert(timeSeries->at(i)->name(), i);
    }

    // Attach the time series to the canals.
    int numCanals = canals->numPolylines();
    for (int i = 0; i < numCanals; i++) {
        Polyline2D *polyline = canals->polyline(i);
        QString timeSeriesKey = polyline->getStringAttribute(_idKey);

        if (timeSeriesKeyToIndexMapping.contains(timeSeriesKey)) {
            int timeSeriesIndex = timeSeriesKeyToIndexMapping.find(timeSeriesKey).value();
            TimeSeries *selectedTimeSeries = timeSeries->at(timeSeriesIndex);
            polyline->setDataAttribute(TIME_SERIES_KEY, selectedTimeSeries);
        }
        else {
            qDebug() << "unable to attach time series for polyline " << polyline;
        }
    }

    // Segment the polylines by the grid.
    Polygon2DSet *gridPolygons = _discretization->grid()->createPolygonSet();
    Polyline2DSet *clipped = canals->createClippedSetWithPolygons(gridPolygons);

    qDebug() << "finished segmenting polylines";
    qDebug() << "the size of the clipped set is " << clipped->numPolylines();

    // Write the polyline set to a shapefile.
    qDebug() << "the clipped shapefile path is " << _clippedShapefilePath;
    clipped->writeToShapefile(_clippedShapefilePath);

    qDebug() << "wrote clipped set to shapefile";

    // Write the river package.
    writeRiverPackage(clipped, _discretization, _idKey);

    qDebug() << "going to return";

    // Clean up.
    delete canals;
    delete timeSeries;

    QDateTime end = QDateTime::currentDateTime();
    double elapsed = start.msecsTo(end) / 1000.0;
    qDebug() << "elapsed time: " << elapsed << " seconds";
}

void PackageProcessorRIV::writeStressPeriod(QTextStream *ts, int stressPeriodIndex, Polyline2DSet *polylines, QString identifierKey, TimeSpan timeSpan) {
    // Write the stress period header.
    //   <10 char-><10 char-><------------34 char-------------><5ch>
    // **         0         0                     Stress Period    1
    int numPolylines = polylines->numPolylines();
    (*ts) << QString("%1").arg(numPolylines, 10);          // sb.append(StringUtil.padLeft(numActive + "", " ", 10));
    (*ts) << "         0                     Stress Period";
    (*ts) << QString("%1").arg(stressPeriodIndex, 5);
    (*ts) << endl;

    // Write the canals.
    for (int i = 0; i < numPolylines; i++) {
        //   <10 char-><10 char-><10 char-><10 char-><-11 char-><---15 char---><9 char->
        //        layer       row       col     stage       cond           rbot      xyz
        // **         1        24       132     0.128   172.2664      -1.520000      917
        // Modification according to instructions (all ten wide, last ignored):
        // <10 char-><10 char-><10 char-><10 char-><10 char-><10 char-><-----20 char------>

        // Get the polyline from the set.
        Polyline2D *polyline = polylines->polyline(i);

        // Numbers in parentheses indicate field width.
        // Write the layer number (10).
        (*ts) << "         1";

        // Write the row (10).
        int row = polyline->getIntegerAttribute(ROW_KEY);
        (*ts) << QString("%1").arg(row, 10);

        // Write the column (10).
        int column = polyline->getIntegerAttribute(COL_KEY);
        (*ts) << QString("%1").arg(column, 10);

        // Write the stage (10).
        TimeSeries *timeSeries = (TimeSeries *)polyline->getDataAttribute(TIME_SERIES_KEY);
        float stage = timeSeries->valueInTimeSpan(timeSpan);
        QString stageString = QString("%1").arg(stage, 10);
        stageString.truncate(10);
        (*ts) << stageString;

        // Write the conductance (10).
        float conductance = (float)polyline->getDoubleAttribute(CONDUCTANCE_KEY);
        QString conductanceString = QString("%1").arg(conductance, 10);
        conductanceString.truncate(10);
        (*ts) << conductanceString;

        // Write the river bottom elevation (10).
        float riverBottom = (float)polyline->getDoubleAttribute(_riverBottomKey);
        QString riverBottomString = QString("%1").arg(riverBottom, 10);
        riverBottomString.truncate(10);
        (*ts) << riverBottomString;

        // Write the identifier (20).
        (*ts) << QString("%1").arg(polyline->getStringAttribute(identifierKey), 20);

        // Close the line.
        (*ts) << endl;
    }
}
void PackageProcessorRIV::writeRiverPackage(Polyline2DSet *clippedPolylines, ModelElementDIS *discretization, QString identifierKey) {
    // Open the output file.
    qDebug() << "going to open file";
    QFile file(_riverPackagePath);
    file.open(QIODevice::WriteOnly);
    QTextStream ts(&file);
    qDebug() << "opened file";

    // Write the comment line.
    //ts << COMMENT_LINE << endl;

    // Write the header line.
    ts << HEADER_LINE << endl;

    // Write the total-canals line.
    QString canalString = QString("%1").arg(clippedPolylines->numPolylines(), 10);
    ts << canalString << "         0" << endl;

    // Write the stress periods.
    int numStressPeriods = discretization->numStressPeriods();
    for (int i = 0; i < numStressPeriods; i++) {
        // Get the time span for this stress period.
        TimeSpan timeSpan = discretization->stressPeriodTimeSpan(i);

        qDebug() << "processing stress period " << i << " of " << numStressPeriods << " :: " << timeSpan.StartDate << " - " << timeSpan.EndDate;

        // Write the stress period.
        writeStressPeriod(&ts, i + 1, clippedPolylines, identifierKey, timeSpan);
    }

    // Flush the stream and close the file.
    ts.flush();
    file.close();

    qDebug() << "closed file";
}
