#ifndef MODELELEMENTDIS_H
#define MODELELEMENTDIS_H

#include <QObject>

#include "modelelement.h"
#include "timescale.h"
#include "timespan.h"
#include "qdom.h"

class OrthoGrid;
class StressPeriod;

class ModelElementDIS : public ModelElement
{
    Q_OBJECT

public:
    explicit ModelElementDIS(QObject *parent = 0);
    ~ModelElementDIS();
    int modelElementType();

    void initWithGridAndStressPeriods(OrthoGrid *grid, StressPeriod *stressPeriods, int numStressPeriods, Timescale timescale);
    void setName(QString name);

    OrthoGrid *grid();
    StressPeriod stressPeriod(int index);
    TimeSpan stressPeriodTimeSpan(int index);
    int numStressPeriods();

    void setAnchorDate(QDateTime date);

    static ModelElementDIS *createFromXMLNode(QDomNode node);

    static ModelElementDIS *createDefaultDiscretization();
    static ModelElementDIS *createSimpleDiscretization(double xCorner, double yCorner, double cellSize, int nCols, int nRows, int nStressPeriods,
                                                       Timescale timescale);

    QList<QString> information();

signals:

public slots:

private:
    OrthoGrid *_grid;
    StressPeriod *_stressPeriods;
    TimeSpan *_stressPeriodTimeSpans;

    Timescale _timescale;
    int _numStressPeriods;
};

#endif // MODELELEMENTDIS_H
