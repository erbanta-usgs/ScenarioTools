/*
#ifndef PACKAGEPROCESSORWEL_H
#define PACKAGEPROCESSORWEL_H

#include <QThread>

class PackageProcessorWEL : public QThread
{
    Q_OBJECT
public:
    explicit PackageProcessorWEL(QObject *parent = 0);

signals:

public slots:

};

#endif // PACKAGEPROCESSORWEL_H
*/

#ifndef PACKAGEPROCESSORWEL_H
#define PACKAGEPROCESSORWEL_H

#include <QString>
#include <QList>
#include <QThread>
#include "timespan.h"

class ShapePoint2DSet;
class ModelElementDIS;
class QTextStream;

#include <QThread>

class PackageProcessorWEL : public QThread
{
    Q_OBJECT
public:
    explicit PackageProcessorWEL(QObject *parent = 0);

    PackageProcessorWEL(QString shapefilePath, QString sampleFilePath, ModelElementDIS *discretization, QString idKey, QString outputShapefilePath, QString wellPackagePath);
    PackageProcessorWEL(QList<QString> shapefilePaths, QList<QString> sampleFilePaths, ModelElementDIS *discretization, QList<QString> idKeys,
                                             QString outputShapefilePath, QString wellPackagePath);

    void process();
    void processSingle();
    void processMultiple();
    void run();

signals:
    void processingBegan(QString message);
    void processingContinues(QString message);
    void processingEnded(QString message);

public slots:

private:
    void writeWellPackage(ShapePoint2DSet *points, ModelElementDIS *discretization, QString identifierKey);

    void writeStressPeriod(QTextStream *ts, int stressPeriodIndex, ShapePoint2DSet *wells, QString identifierKey, TimeSpan timeSpan);

    bool _multiple;

    QList<QString> _shapefilePaths;
    QList<QString> _sampleFilePaths;
    QList<QString> _idKeys;

    QString _shapefilePath;
    QString _sampleFilePath;
    ModelElementDIS *_discretization;
    QString _idKey;
    QString _outputShapefilePath;
    QString _wellPackagePath;
};

/*
class PackageProcessorWEL : public QThread
{
    Q_OBJECT

public:
    explicit PackageProcessorWEL(QObject *parent = 0);





//signals:


private:


};
*/

#endif // PACKAGEPROCESSORWEL_H


