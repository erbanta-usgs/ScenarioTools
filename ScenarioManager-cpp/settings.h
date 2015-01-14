#ifndef SETTINGS_H
#define SETTINGS_H

#include <QString>

#define BROWSE_DIRECTORY "browse_directory"

class Settings
{
public:
    static void setValue(QString key, QString value);
    static QString getValue(QString key, QString defaultValue = "");
};

#endif // SETTINGS_H
