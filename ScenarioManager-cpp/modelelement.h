#ifndef SCENARIOELEMENT_H
#define SCENARIOELEMENT_H

#include <QObject>

#define MODEL_ELEMENT_TYPE_PLAIN 0
#define MODEL_ELEMENT_TYPE_DIS 1
#define MODEL_ELEMENT_TYPE_RIV 2
#define MODEL_ELEMENT_TYPE_SWR 3
#define MODEL_ELEMENT_TYPE_WEL 4

class ModelElement : public QObject
{
    Q_OBJECT
public:
    explicit ModelElement(QObject *parent = 0);

    QString name();
    void setName(QString name = 0);

    virtual QList<QString> information();
    virtual int modelElementType();

signals:

public slots:

protected:
    QString _name;

};

#endif // SCENARIOELEMENT_H
