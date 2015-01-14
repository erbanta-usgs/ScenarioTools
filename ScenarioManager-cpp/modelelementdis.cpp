#include <QDebug>

#include "modelelementdis.h"
#include "orthogrid.h"
#include "stressperiod.h"
#include "timescale.h"
#include "xmlutil.h"
#include "dateutil.h"

#define DEFAULT_ELEMENT_NAME "Unnamed Discretization"


ModelElementDIS::ModelElementDIS(QObject *parent) : ModelElement(parent)
{
    _grid = NULL;
    _stressPeriods = NULL;
    _stressPeriodTimeSpans = NULL;
    _numStressPeriods = 0;
}

ModelElementDIS *ModelElementDIS::createFromXMLNode(QDomNode node)
{
    // Get the element name from the node.
    QString elementName = node.nodeName();
    qDebug() << "node name (should be ModelElementDIS): " << node.nodeName();

    /*
    double xCorner,
    double yCorner,
    double cellSize,
    int nCols,
    int nRows,
    int nStressPeriods,
    Timescale timescale
    */


    // Get the other data from the node attributes.
    double xCornerL = XMLUtil::safeGetDoubleAttributeByName(node, "xCornerL");
    double yCornerL = XMLUtil::safeGetDoubleAttributeByName(node, "yCornerL");
    double cellSize = XMLUtil::safeGetDoubleAttributeByName(node, "cellSize");
    int numCols = XMLUtil::safeGetIntegerAttributeByName(node, "numCols");
    int numRows = XMLUtil::safeGetIntegerAttributeByName(node, "numRows");
    int numStressPeriods = XMLUtil::safeGetIntegerAttributeByName(node, "numStressPeriods");
    QDateTime startDate = XMLUtil::safeGetDateAttributeByName(node, "startDate");
    Timescale timescale = XMLUtil::safeGetTimescaleAttributeByName(node, "stressPeriodScale");

    // Show the data.
    qDebug() << "xCornerL: " << xCornerL;
    qDebug() << "yCornerL: " << yCornerL;
    qDebug() << "cellSize: " << cellSize;
    qDebug() << "numCols: " << numCols;
    qDebug() << "numRows: " << numRows;
    qDebug() << "numStressPeriods " << numStressPeriods;
    qDebug() << "startDate: " << startDate.toString();
    qDebug() << "timescale: " << timescale;

    /*
    // Make the stress periods.
    StressPeriod *stressPeriods = new StressPeriod[nStressPeriods];
    for (int i = 0; i < nStressPeriods; i++) {
        stressPeriods[i] = StressPeriod(1.0);
    }

    // Make the grid.
    OrthoGrid *grid = new OrthoGrid(nCols, nRows, cellSize, Point2D(xCorner, yCorner));

    // Make the discretization.
    ModelElementDIS *discretization = new ModelElementDIS();
    discretization->initWithGridAndStressPeriods(grid, stressPeriods, nStressPeriods, timescale);
    */

    return NULL;
}

ModelElementDIS::~ModelElementDIS()
{
    if (_grid != NULL) { delete _grid; }
    if (_stressPeriods != NULL) { delete[] _stressPeriods; }
    if (_stressPeriodTimeSpans != NULL) { delete[] _stressPeriodTimeSpans; }
}
void ModelElementDIS::initWithGridAndStressPeriods(OrthoGrid *grid, StressPeriod *stressPeriods, int numStressPeriods, Timescale timescale)
{
    _grid = grid;
    _stressPeriods = stressPeriods;
    _numStressPeriods = numStressPeriods;
    _timescale = timescale;
}
void ModelElementDIS::setName(QString name) {
    static int defaultIndex = 1;

    _name = name.trimmed();
    if (_name.isEmpty()) {
        _name = QString(DEFAULT_ELEMENT_NAME).append(" %1").arg(defaultIndex++);
    }
}
OrthoGrid *ModelElementDIS::grid() {
    return _grid;
}
int ModelElementDIS::numStressPeriods() {
    return _numStressPeriods;
}
StressPeriod ModelElementDIS::stressPeriod(int index) {
    return _stressPeriods[index];
}
TimeSpan ModelElementDIS::stressPeriodTimeSpan(int index) {
    if (_stressPeriodTimeSpans == NULL) {
        qDebug() << "time spans are null";
        return TimeSpan();
    }
    else {
        return _stressPeriodTimeSpans[index];
    }
}

void ModelElementDIS::setAnchorDate(QDateTime date) {
    // If the stress period time span array has not been made, make it.
    if (_stressPeriodTimeSpans == NULL) {
        _stressPeriodTimeSpans = new TimeSpan[_numStressPeriods];
    }

    // Populate the time span array.
    QDateTime startOfStressPeriod = date;
    for (int i = 0; i < _numStressPeriods; i++) {
        // Calculate the end of the stress period. The default step is daily.
        QDateTime endOfStressPeriod;
        if (_timescale == TIMESCALE_YEARLY) {
            endOfStressPeriod = startOfStressPeriod.addYears(this->stressPeriod(i).periodLength());
        }
        else if (_timescale == TIMESCALE_MONTHLY) {
            endOfStressPeriod = startOfStressPeriod.addMonths(this->stressPeriod(i).periodLength());
        }
        else {
            endOfStressPeriod = startOfStressPeriod.addDays(this->stressPeriod(i).periodLength());
        }
        qDebug() << "making stress period time span " << i << ": " << startOfStressPeriod << " - " << endOfStressPeriod;

        // Store the time span.
        _stressPeriodTimeSpans[i] = TimeSpan(startOfStressPeriod, endOfStressPeriod);

        // Advance the stress period.
        startOfStressPeriod = endOfStressPeriod;
    }
}

ModelElementDIS *ModelElementDIS::createSimpleDiscretization(double xCorner, double yCorner, double cellSize, int nCols, int nRows, int nStressPeriods,
                                                             Timescale timescale) {
    // Make the stress periods.
    StressPeriod *stressPeriods = new StressPeriod[nStressPeriods];
    for (int i = 0; i < nStressPeriods; i++) {
        stressPeriods[i] = StressPeriod(1.0);
    }

    // Make the grid.
    OrthoGrid *grid = new OrthoGrid(nCols, nRows, cellSize, Point2D(xCorner, yCorner));

    // Make the discretization.
    ModelElementDIS *discretization = new ModelElementDIS();
    discretization->initWithGridAndStressPeriods(grid, stressPeriods, nStressPeriods, timescale);

    // Return the result.
    return discretization;
}

ModelElementDIS *ModelElementDIS::createDefaultDiscretization() {
    // Make the stress periods.
    int numStressPeriods = 36;
    StressPeriod *stressPeriods = new StressPeriod[numStressPeriods];

    for (int i = 0; i < numStressPeriods; i++) {
        stressPeriods[i] = StressPeriod(1.0);
    }

    // Make the grid.
    OrthoGrid *grid = new OrthoGrid(930, 1730, 50.0, Point2D(543750.0, 2785750.0));

    // Make the discretization.
    ModelElementDIS *discretization = new ModelElementDIS();
    discretization->initWithGridAndStressPeriods(grid, stressPeriods, numStressPeriods, TIMESCALE_MONTHLY);

    // Set the name.
    discretization->setName("default discretization");
    discretization->setAnchorDate(DateUtil::CreateDateYMD(2002, 1, 1));

    // Return the result.
    return discretization;
}

int ModelElementDIS::modelElementType() {
    return MODEL_ELEMENT_TYPE_DIS;
}


QList<QString> ModelElementDIS::information() {
    qDebug() << "building information";

    // Build the information list.
    QList<QString> information;
    information.append(QString("Model Discretization: %1").arg(_name));
    information.append(QString("number of columns: %1").arg(_grid == NULL ? 0 : _grid->nCols()));
    information.append(QString("number of rows: %1").arg(_grid == NULL ? 0 : _grid->nRows()));
    information.append(QString("number of stress periods: %1").arg(_numStressPeriods));

    qDebug() << "going to add grid information";
    information.append(QString("average column width: %1").arg(_grid == NULL ? 0 : _grid->averageCellWidth()));
    information.append(QString("average row height: %1").arg(_grid == NULL ? 0 : _grid->averageCellHeight()));
    qDebug() << "added grid information";

    // Return the list.
    return information;
}
