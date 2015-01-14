#ifndef SMDOCUMENT_H
#define SMDOCUMENT_H

#include <QObject>
#include <QAbstractItemModel>

class ModelElement;

class SMDocument : public QAbstractItemModel
{
    Q_OBJECT
public:
    explicit SMDocument(QObject *parent = 0);

    void addModelElement(ModelElement *element);
    ModelElement *getElement(int uniqueID);

    static SMDocument *createFromXML(QString path);

    // Implemented virtual functions from QAbstractItemModel
    QModelIndex index(int row, int column, const QModelIndex &parent) const;
    QModelIndex parent(const QModelIndex &child) const;
    int rowCount(const QModelIndex &parent) const;
    int columnCount(const QModelIndex &parent) const;
    QVariant data(const QModelIndex &index, int role) const;
    bool insertRows(int row, int count, const QModelIndex &parent);
    Qt::ItemFlags flags(const QModelIndex &index) const;

signals:
    void elementAdded(ModelElement *parent, int index, int uniqueID);
    void elementMoved(ModelElement *previousParent, int previousIndex, ModelElement *newParent, int newIndex);

public slots:

private:
    QList<ModelElement *> *_modelElements;
    QList<int> *_uniqueIdentifiers;

};

#endif // SMDOCUMENT_H
