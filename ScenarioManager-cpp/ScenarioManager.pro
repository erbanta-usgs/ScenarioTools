#-------------------------------------------------
#
# Project created by QtCreator 2010-09-07T16:20:39
#
#-------------------------------------------------

QT       += core gui xml

TARGET = ScenarioManager
TEMPLATE = app

#Put these back for a command-line app.
#CONFIG   += console
#CONFIG   -= app_bundle

SOURCES += main.cpp\
        mainwindow.cpp \
    smdocument.cpp \
    modelelement.cpp \
    modelelementdis.cpp \
    modelelementdialogdis.cpp \
    smdocumenttreeview.cpp \
    packageprocessorriv.cpp \
    point2d.cpp \
    polygon2d.cpp \
    linesegment2d.cpp \
    range1d.cpp \
    polyline2d.cpp \
    shapelib/shpopen.c \
    polyline2dset.cpp \
    stressperiod.cpp \
    orthogrid.cpp \
    polygon2dset.cpp \
    range2d.cpp \
    shapelib/dbfopen.c \
    timeseriessample.cpp \
    timeseries.cpp \
    stringutil.cpp \
    dateutil.cpp \
    timespan.cpp \
    modelelementdialogriv.cpp \
    modelelementriv.cpp \
    riverpackagemain.cpp \
    xmlutil.cpp \
    mapwidget.cpp \
    shapepoint2d.cpp \
    shapepoint2dset.cpp \
    treemodel.cpp \
    treeitem.cpp \
    modelelementdialogwel.cpp \
    modelelementwel.cpp \
    settings.cpp \
    fileutil.cpp \
    packageprocessorwell2.cpp \
    packageprocessorwel.cpp \
    shapefileutil.cpp

HEADERS  += mainwindow.h \
    smdocument.h \
    modelelement.h \
    modelelementdis.h \
    modelelementdialogdis.h \
    smdocumenttreeview.h \
    packageprocessorriv.h \
    point2d.h \
    polygon2d.h \
    linesegment2d.h \
    range1d.h \
    polyline2d.h \
    shapelib/shapefil.h \
    polyline2dset.h \
    stressperiod.h \
    orthogrid.h \
    polygon2dset.h \
    range2d.h \
    timeseriessample.h \
    timeseries.h \
    stringutil.h \
    attribute.h \
    attributekeys.h \
    dateutil.h \
    timespan.h \
    modelelementdialogriv.h \
    modelelementriv.h \
    xmlutil.h \
    timescale.h \
    mapwidget.h \
    shapepoint2d.h \
    shapepoint2dset.h \
    treemodel.h \
    treeitem.h \
    modelelementdialogwel.h \
    modelelementwel.h \
    settings.h \
    fileutil.h \
    platform.h \
    packageprocessorwell2.h \
    packageprocessorwel.h \
    shapefileutil.h

FORMS    += mainwindow.ui \
    modelelementdialogdis.ui \
    modelelementdialogriv.ui \
    modelelementdialogwel.ui

RESOURCES += \
    resources.qrc
