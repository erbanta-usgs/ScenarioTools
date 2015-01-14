#ifndef PACKAGEPROCESSORWELL2_H
#define PACKAGEPROCESSORWELL2_H

#include <QThread>

class PackageProcessorWell2 : public QThread
{
    Q_OBJECT
public:
    explicit PackageProcessorWell2(QObject *parent = 0);

signals:

public slots:

};

#endif // PACKAGEPROCESSORWELL2_H
