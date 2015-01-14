#include <QDebug>

#include "fileutil.h"
#include "platform.h"
#include <QFile>
#include <QTextStream>

QList<QString> FileUtil::getFileContents(QString path) {
    // Make the list for the file's contents.
    QList<QString> fileContents;

    // If the file exists and can be opened, add the contents of the file to the list.
    QFile file(path);
    if (file.exists(path)) {
        if (file.open(QIODevice::ReadOnly)) {
            // Read the entire file into the list.
            while (!file.atEnd()) {
                fileContents.append(file.readLine().trimmed());
            }

            // Close the file.
            file.close();
        }
    }

    // Return the result.
    qDebug() << "retrieved " << fileContents.count() << " lines from file";
    return fileContents;
}

void FileUtil::writeFileContents(QList<QString> fileContents, QString path) {
    // Open the file and write the contents.
    QFile file(path);
    if (file.open(QIODevice::WriteOnly)) {
        QTextStream fileStream(&file);

        for (int i = 0; i < fileContents.length(); i++) {
            fileStream << fileContents.at(i);
            if (i != fileContents.length() - 1) {
                fileStream << END_LINE;
            }
        }

        // Close the file.
        file.close();
    }
}
