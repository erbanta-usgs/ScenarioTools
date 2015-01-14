#include "settings.h"

#include "fileutil.h"
#include <QStringList>

#define SETTINGS_FILE "settings.ini"
#define DELIMITER "="

void Settings::setValue(QString key, QString value) {
    // Get the contents of the settings file.
    QList<QString> fileContents = FileUtil::getFileContents(SETTINGS_FILE);

    // Trim whitespace from the key.
    key = key.trimmed();

    // If the key is located in the file, replace it.
    bool foundMatch = false;
    for (int i = 0; i < fileContents.count() && !foundMatch; i++) {
        QStringList split = fileContents.at(i).split(DELIMITER);
        if (split.length() > 0) {
            // If the key is found, replace the line with the new key/value pair and mark that a match has been found.
            if (split.at(0).trimmed().compare(key, Qt::CaseInsensitive) == 0) {
                fileContents.replace(i, key.append(DELIMITER).append(value));
                foundMatch = true;
            }
        }
    }

    // If we haven't found a match, append the value to the end of the file.
    if (!foundMatch) {
        fileContents.append(key.append(DELIMITER).append(value));
    }

    // Write the array to the file.
    FileUtil::writeFileContents(fileContents, "settings.ini");
}

QString Settings::getValue(QString key, QString defaultValue) {
    // Get the contents of the settings file.
    QList<QString> fileContents = FileUtil::getFileContents(SETTINGS_FILE);

    // Try to find the specified setting.
    for (int i = 0; i < fileContents.count(); i++) {
        QStringList split = fileContents.at(i).split(DELIMITER);
        if (split.length() > 1) {
            // If the key is found, return the value.
            if (split.at(0).trimmed().compare(key, Qt::CaseInsensitive) == 0) {
                return split.at(1).trimmed();
            }
        }
    }

    // If we're still here, return the default value.
    return defaultValue;
}
