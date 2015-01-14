#include "smdocument.h"
#include "modelelement.h"
#include "modelelementriv.h"
#include "modelelementdis.h"

#include <QDebug>
#include <qdom.h>
#include <QFile>

static int _uniqueIDGenerator;

SMDocument::SMDocument(QObject *parent) :
    QAbstractItemModel(parent)
{
    // Make the list for the report elements and the unique identifiers.
    _modelElements = new QList<ModelElement *>();
    _uniqueIdentifiers = new QList<int>();
}

SMDocument *SMDocument::createFromXML(QString path) {
    // Get the contents of the XML document.
    QDomDocument doc;
    QFile xmlFile(path);
    doc.setContent(&xmlFile);

    // Get the document element.
    QDomElement docElement = doc.documentElement();

    // Make a discretization node from the discretization element.
    QDomNode node = docElement.firstChild();
    qDebug() << "The first node is " << node.nodeName();
    ModelElementDIS *disElement = ModelElementDIS::createFromXMLNode(node);

    // Make a model element out of each child node.
   /* QDomNodeList modelElementNodes = docElement.childNodes();
    int numNodes = modelElementNodes.length();
    int numGoodNodes = 0;
    for (int i = 0; i < numNodes; i++) {

    }
*/

    // Get the first node and create a river package from it.
    /*
    QDomNode node = docElement.firstChild();
    qDebug() << "The first node is " << node.nodeName();
    ModelElementRIV *riverElement = ModelElementRIV::createFromXMLNode(node);
    */

    // Make the new scenario manager document and add the river package to it.
    SMDocument *document = new SMDocument();
    document->addModelElement(disElement);

    return document;
}

void SMDocument::addModelElement(ModelElement *element) {
    // Add the element to the list.
    _modelElements->append(element);

    // Generate the unique identifier and add it to the list.
    int uniqueID = _uniqueIDGenerator++;
    _uniqueIdentifiers->append(uniqueID);

    // Send a signal that an element has been added to the list.
    emit elementAdded(NULL, _modelElements->size() - 1, uniqueID);

    // Send a signal that the QAbstractItemModel layout has changed.
    emit layoutChanged();
}

ModelElement *SMDocument::getElement(int uniqueID) {
    for (int i = 0; i < _modelElements->size(); i++) {
        if (_uniqueIdentifiers->at(i) == uniqueID) {
            return _modelElements->at(i);
        }
    }

    return NULL;
}

QModelIndex SMDocument::index(int row, int column, const QModelIndex &parent) const {
    //qDebug() << "Calling index for row " << row << ", col " << column << ", parent " << parent << " : " << callNumber++;

    if (row < _modelElements->size() && column == 0) {
        QModelIndex index = createIndex(row, column, _uniqueIdentifiers->at(row));
        qDebug() << "index data " << index.internalId();
        return index;
    }
    else {
        return QModelIndex();
    }
}

QModelIndex SMDocument::parent(const QModelIndex &child) const {
    return QModelIndex();
}

int SMDocument::rowCount(const QModelIndex &parent) const {
    //qDebug() << "Calling parent for " << parent;

    if (parent.isValid()) {
        return 0;
    }
    else {
        int count = _modelElements->size();
        //qDebug() << "going to return a row count of " << count;
        return count;
    }
}

int SMDocument::columnCount(const QModelIndex &parent) const {
    //qDebug() << "Calling for column count of " << parent;

    if (parent.isValid()) {
        return 0;
    }
    else {
        return 1;
    }
}
Qt::ItemFlags SMDocument::flags(const QModelIndex &index) const {
    if (index.isValid()) {
        return
                Qt::ItemIsEnabled |
                Qt::ItemIsSelectable |
                Qt::ItemIsDragEnabled |
                Qt::ItemIsDropEnabled |
                Qt::ItemIsTristate;
    }
    else {
        return Qt::ItemIsDropEnabled;
    }
}
QVariant SMDocument::data(const QModelIndex &index, int role) const {
   // qDebug() << "asked for data of role " << role;

    if (role == Qt::UserRole) {
        qDebug() << "YAAAAAAAAAY: USER ROLE!!!!!";
    }

    if (role == Qt::DisplayRole && index.row() < _modelElements->size()) {
        QString s = _modelElements->at(index.row())->name();
        //qDebug() << "The display string is: " << s;
        return QVariant(s);
    }
    else {
        return QVariant();
    }
}

bool SMDocument::insertRows(int row, int count, const QModelIndex &parent) {
    qDebug() << "asked to add row: " << row << ", count: " << count << ", parent: " << parent;

    if (parent.isValid()) {
        return false;
    }

    else {
        if (row >= _modelElements->size()) {
            int uniqueID = _uniqueIDGenerator++;
            _uniqueIdentifiers->append(uniqueID);
            _modelElements->append(new ModelElement());
        }
        else {
            int uniqueID = _uniqueIDGenerator++;
            _uniqueIdentifiers->insert(row, uniqueID);
            _modelElements->insert(row, new ModelElement());
        }



        emit layoutChanged();

        return true;
    }
}
