#ifndef FILEUTIL_H
#define FILEUTIL_H

#include <QList>
#include <QString>

class FileUtil
{
public:
    static QList<QString> getFileContents(QString path);
    static void writeFileContents(QList<QString> fileContents, QString path);
};

#endif // FILEUTIL_H
