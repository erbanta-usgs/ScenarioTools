//#include "packageprocessorwel.h"

#include "packageprocessorwel.h"

#include "modelelementdis.h"
#include "shapepoint2d.h"
#include "shapepoint2dset.h"
#include "orthogrid.h"
#include "timeseries.h"
#include "timeseriessample.h"
#include "point2d.h"
#include "attributekeys.h"
#include "dateutil.h"
#include "stressperiod.h"

#include <QDebug>
#include <QFile>
#include <vector>
#include <iostream>

#define HEADER_LINE "PARAMETER  0         0"

using namespace std;


PackageProcessorWEL::PackageProcessorWEL(QObject *parent) : QThread(parent)
{
}

PackageProcessorWEL::PackageProcessorWEL(QString shapefilePath, QString sampleFilePath, ModelElementDIS *discretization, QString idKey, QString outputShapefilePath,
                                         QString wellPackagePath)
{
    // Flag that this is not a multiple processor.
    _multiple = false;

    // Store the configuration.
    _shapefilePath = shapefilePath;
    _sampleFilePath = sampleFilePath;
    _discretization = discretization;
    _idKey = idKey;
    _outputShapefilePath = outputShapefilePath;
    _wellPackagePath = wellPackagePath;
}

PackageProcessorWEL::PackageProcessorWEL(QList<QString> shapefilePaths, QList<QString> sampleFilePaths, ModelElementDIS *discretization, QList<QString> idKeys,
                                         QString outputShapefilePath, QString wellPackagePath) {
    // Flag that this is a multiple processor.
    _multiple = true;

    // Store the configuration.
    _shapefilePaths = shapefilePaths;
    _sampleFilePaths = sampleFilePaths;
    _discretization = discretization;
    _idKeys = idKeys;
    _outputShapefilePath = outputShapefilePath;
    _wellPackagePath = wellPackagePath;
}

void PackageProcessorWEL::process()
{
    emit processingBegan("Well Package processor started");

    if (_multiple) {
        processMultiple();
    }
    else {
        processSingle();
    }

    emit processingEnded("Well Package processor completed successfully");
}

void PackageProcessorWEL::run() {
    this->process();
}

void PackageProcessorWEL::processSingle() {
    QDateTime start = QDateTime::currentDateTime();

    // Make a point set and initialize it from the shapefile.
    ShapePoint2DSet *wells = new ShapePoint2DSet();
    wells->initFromShapefile(_shapefilePath);

    // Get the time series.
    //qDebug() << "going to read SMP file " << _sampleFilePath;
    vector<TimeSeries *> *timeSeries = TimeSeries::fromSMPFile(_sampleFilePath);
    //qDebug() << "got " << timeSeries->size() << " series from SMP file";

    // Make the mapping from time series identifier to time series index.
    QMap<QString, int> timeSeriesKeyToIndexMapping;
    for (uint i = 0; i < timeSeries->size(); i++) {
        QString key = timeSeries->at(i)->name();
        //qDebug() << "adding key " << key << " to mapping";
        timeSeriesKeyToIndexMapping.insert(timeSeries->at(i)->name(), i);
    }

    // Attach the time series to the wells.
    int numWells = wells->numPoints();
    for (int i = 0; i < numWells; i++) {
        ShapePoint2D *point = wells->point(i);
        QString timeSeriesKey = point->getStringAttribute(_idKey);

        if (timeSeriesKeyToIndexMapping.contains(timeSeriesKey)) {
            int timeSeriesIndex = timeSeriesKeyToIndexMapping.find(timeSeriesKey).value();
            TimeSeries *selectedTimeSeries = timeSeries->at(timeSeriesIndex);
            point->setDataAttribute(TIME_SERIES_KEY, selectedTimeSeries);
        }
        else {
            //qDebug() << "unable to attach time series for well " << point << " :: " << timeSeriesKey;
        }
    }

    // Attach the grid information.
    Polygon2DSet *gridPolygons = _discretization->grid()->createPolygonSet();
    wells->assignGridCells(gridPolygons);

    //qDebug() << "attached grid information to polygons";

    // Write the well set to the shapefile.
    //qDebug() << "the output shapefile path is " << _outputShapefilePath;
    wells->writeToShapefile(_outputShapefilePath);

    //qDebug() << "wrote output set to shapefile";

    // Write the well package.
    writeWellPackage(wells, _discretization, _idKey);

    //qDebug() << "going to return";

    // Clean up.
    delete wells;
    delete timeSeries;

    QDateTime end = QDateTime::currentDateTime();
    double elapsed = start.msecsTo(end) / 1000.0;
    //qDebug() << "elapsed time: " << elapsed << " seconds";
}
void PackageProcessorWEL::processMultiple() {
    QDateTime start = QDateTime::currentDateTime();

    QList<ShapePoint2D *> allWellsList;
    for (int j = 0; j < _shapefilePaths.length(); j++) {
        // Make a point set and initialize it from the shapefile.
        ShapePoint2DSet *wells = new ShapePoint2DSet();
        wells->initFromShapefile(_shapefilePaths.at(j));

        // Get the time series.
        emit processingContinues(QString("Going to read SMP file %1").arg(_sampleFilePath));
        vector<TimeSeries *> *timeSeries = TimeSeries::fromSMPFile(_sampleFilePaths.at(j));
        emit processingContinues(QString("Got %1 time series from SMP file").arg(timeSeries->size()));

        // Make the mapping from time series identifier to time series index.
        QMap<QString, int> timeSeriesKeyToIndexMapping;
        for (uint i = 0; i < timeSeries->size(); i++) {
            timeSeriesKeyToIndexMapping.insert(timeSeries->at(i)->name(), i);
        }
        emit processingContinues("Added keys for time series lookups");

        // Attach the time series to the wells.
        int numWells = wells->numPoints();
        for (int i = 0; i < numWells; i++) {
            ShapePoint2D *point = wells->point(i);
            QString timeSeriesKey = point->getStringAttribute(_idKeys.at(j));

            if (timeSeriesKeyToIndexMapping.contains(timeSeriesKey)) {
                int timeSeriesIndex = timeSeriesKeyToIndexMapping.find(timeSeriesKey).value();
                TimeSeries *selectedTimeSeries = timeSeries->at(timeSeriesIndex);
                point->setDataAttribute(TIME_SERIES_KEY, selectedTimeSeries);

                emit processingContinues(QString("Attached time series for well %1").arg(timeSeriesKey));
            }
            else {
                emit processingContinues(QString("Unable to attach time series for well %1").arg(timeSeriesKey));
            }
        }

        // Attach the grid information.
        Polygon2DSet *gridPolygons = _discretization->grid()->createPolygonSet();
        wells->assignGridCells(gridPolygons);
        emit processingContinues("Assigned grid cells to wells");

        // Add all wells from the set to the combined list.
        for (int i = 0; i < wells->numPoints(); i++) {
            allWellsList.append(wells->point(i));
        }
    }

    // Make a new well set.
    //qDebug() << "there are a total of " << allWellsList.length() << " in the combined list";
    ShapePoint2D *allWellsArray[allWellsList.length()];
    for (int i = 0; i < allWellsList.length(); i++) {
        allWellsArray[i] = allWellsList.at(i);
    }
    ShapePoint2DSet *combinedSet = new ShapePoint2DSet(allWellsArray, allWellsList.length());
    emit processingContinues(QString("Created combined set of %1 wells").arg(allWellsList.length()));

    // Write the well set to the shapefile.
    combinedSet->writeToShapefile(_outputShapefilePath);
    emit processingContinues(QString("Wrote output shapefile to %1").arg(_outputShapefilePath));

    // Write the well package.
    // TODO: MAP THE ID FROM OTHER SHAPEFILES TO A COMMON FIELD.
    writeWellPackage(combinedSet, _discretization, _idKey);

    //qDebug() << "going to return";

    // Clean up.
    //delete wells;
    //delete timeSeries;

    QDateTime end = QDateTime::currentDateTime();
    double elapsed = start.msecsTo(end) / 1000.0;
    emit processingEnded(QString("Processing complete (elapsed: %1 seconds)").arg(elapsed));
}

void PackageProcessorWEL::writeStressPeriod(QTextStream *ts, int stressPeriodIndex, ShapePoint2DSet *wells, QString identifierKey, TimeSpan timeSpan) {
    // Write the stress period header.
    //   <10 char-><10 char-><------------34 char-------------><5ch>
    // **         0         0                     Stress Period    1
    int numWells = wells->numPoints();
    (*ts) << QString("%1").arg(numWells, 10);          // sb.append(StringUtil.padLeft(numActive + "", " ", 10));
    (*ts) << "         0                     Stress Period";
    (*ts) << QString("%1").arg(stressPeriodIndex, 5);
    (*ts) << endl;

    //qDebug() << "about to write wells for ts " << ts;

    // Write the wells.
    for (int i = 0; i < numWells; i++) {
        //   <10 char-><10 char-><10 char-><10 char-><-11 char-><---15 char---><9 char->
        //        layer       row       col     stage       cond           rbot      xyz
        // **         1        24       132     0.128   172.2664      -1.520000      917
        // Modification according to instructions (all ten wide, last ignored):
        // <10 char-><10 char-><10 char-><10 char-><10 char-><10 char-><-----20 char------>

        // Get the well from the set.
        ShapePoint2D *well = wells->point(i);

        // Numbers in parentheses indicate field width.
        // Write the layer number (10).
        (*ts) << "         1";

        // Write the row (10).
        int row = well->getIntegerAttribute(ROW_KEY);
        (*ts) << QString("%1").arg(row, 10);

        // Write the column (10).
        int column = well->getIntegerAttribute(COL_KEY);
        (*ts) << QString("%1").arg(column, 10);

        // Write the stage (10).
        TimeSeries *timeSeries = (TimeSeries *)well->getDataAttribute(TIME_SERIES_KEY);
        float stage = timeSeries == NULL ? 0.0f : timeSeries->valueInTimeSpan(timeSpan);
        QString stageString = QString("%1").arg(stage, 10);
        stageString.truncate(10);
        (*ts) << stageString;

        // Write the identifier (20).
        (*ts) << QString("%1").arg(well->getStringAttribute(identifierKey), 20);

        // Close the line.
        (*ts) << endl;
    }
}
void PackageProcessorWEL::writeWellPackage(ShapePoint2DSet *wells, ModelElementDIS *discretization, QString identifierKey) {
    // Open the output file.
    //qDebug() << "going to open file";
    QFile file(_wellPackagePath);
    file.open(QIODevice::WriteOnly);
    QTextStream ts(&file);
    //qDebug() << "opened file";

    // Write the comment line.
    //ts << COMMENT_LINE << endl;

    // Write the header line.
    ts << HEADER_LINE << endl;

    // Write the total-wells line.
    QString wellString = QString("%1").arg(wells->numPoints(), 10);
    ts << wellString << "         0" << endl;

    // Write the stress periods.
    int numStressPeriods = discretization->numStressPeriods();
    for (int i = 0; i < numStressPeriods; i++) {
        // Get the time span for this stress period.
        TimeSpan timeSpan = discretization->stressPeriodTimeSpan(i);

        //qDebug() << "processing stress period " << i << " of " << numStressPeriods << " :: " << timeSpan.StartDate << " - " << timeSpan.EndDate;

        // Write the stress period.
        writeStressPeriod(&ts, i + 1, wells, identifierKey, timeSpan);
    }

    // Flush the stream and close the file.
    ts.flush();
    file.close();

    //qDebug() << "closed file";
}

