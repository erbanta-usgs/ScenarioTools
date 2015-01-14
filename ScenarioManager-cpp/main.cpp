#include <QtGui/QApplication>
#include <QDebug>
#include <iostream>

#include "mainwindow.h"
#include "packageprocessorriv.h"
#include "modelelementdis.h"
#include "dateutil.h"
#include "modelelementriv.h"
#include "smdocument.h"

#define SUCCESS 0
#define FAILURE 1

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    MainWindow w;
    w.show();

    return a.exec();
}
