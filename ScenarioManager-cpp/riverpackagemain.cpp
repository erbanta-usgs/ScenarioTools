#include <QDebug>

#include <QString>
#include "modelelementdis.h"
#include "dateutil.h"
#include "packageprocessorriv.h"

#define TIME_SERIES_SHAPEFILE_KEY "DB_NAME"
#define RIVER_BOTTOM_KEY "BOT_NAVDM"
#define WIDTH_KEY "TOP_WID_M"
#define CONDUCTIVITY_KEY "HYDR_COND"
#define FLOW_LENGTH_KEY "LENGTH"

int riverPackageMain(int argc, char *argv[]) {
    /*
    argv = (char **)malloc(19 * sizeof(char *));
    argv[1] = (char *)"C:\\test\\riv_gen\\mod_shp\\md.shp";
    argv[2] = (char *)"C:\\test\\riv_gen\\output_navd_all.smp";
    argv[3] = (char *)"543750.0";
    argv[4] = (char *)"2785750.0";
    argv[5] = (char *)"500";
    argv[6] = (char *)"93";
    argv[7] = (char *)"173";
    argv[8] = (char *)"10";         // number of stress periods
    argv[9] = (char *)"1996";
    argv[10] = (char *)"1";
    argv[11] = (char *)"1";
    argv[12] = (char *)"D";
    argv[13] = (char *)TIME_SERIES_SHAPEFILE_KEY;
    argv[14] = (char *)WIDTH_KEY;
    argv[15] = (char *)CONDUCTIVITY_KEY;
    argv[16] = (char *)FLOW_LENGTH_KEY;
    argv[17] = (char *)RIVER_BOTTOM_KEY;
    argv[18] = (char *)"c:\\test\\clipped.shp";
    argv[19] = (char *)"c:\\test\\out_500m.riv";
    argc = 20;
    */

    if (argc != 20) {
        qDebug() << "The wrong number of arguments was supplied: " << (argc - 1);
        return 1;
    }

    QString shapefilePath = argv[1];
    QString sampleFilePath = argv[2];
    double xCornerLL = atof(argv[3]);
    double yCornerLL = atof(argv[4]);
    double gridSize = atof(argv[5]);
    int nCols = atoi(argv[6]);
    int nRows = atoi(argv[7]);
    int nStressPeriods = atoi(argv[8]);
    int year = atoi(argv[9]);
    int month = atoi(argv[10]);
    int day = atoi(argv[11]);
    QString timescaleArg = argv[12];
    QString idKey = argv[13];
    QString widthKey = argv[14];
    QString conductivityKey = argv[15];
    QString flowLengthKey = argv[16];
    QString riverBottomKey = argv[17];
    QString clippedShapefilePath = argv[18];
    QString riverPackagePath = argv[19];

    qDebug() << "                   Supplied Arguments                   ";
    qDebug() << "=========================================================";
    qDebug() << "shapefile path:" << shapefilePath;
    qDebug() << "sample file path: " << sampleFilePath;

    // Make the discretization.
    Timescale timescale = timescaleArg.compare("M", Qt::CaseInsensitive) == 0 ? TIMESCALE_MONTHLY : TIMESCALE_DAILY;
    ModelElementDIS *discretization = ModelElementDIS::createSimpleDiscretization(xCornerLL, yCornerLL, gridSize, nCols, nRows, nStressPeriods, timescale);

    // Make the start date and anchor the discretization.
    QDateTime startDate = DateUtil::CreateDateYMD(year, month, day);
    discretization->setAnchorDate(startDate);

    PackageProcessorRIV *processorRIV = new PackageProcessorRIV(shapefilePath, sampleFilePath, discretization, idKey, widthKey, conductivityKey, flowLengthKey,
                                                                riverBottomKey, clippedShapefilePath, riverPackagePath);
    processorRIV->process();

    return 0;
}
