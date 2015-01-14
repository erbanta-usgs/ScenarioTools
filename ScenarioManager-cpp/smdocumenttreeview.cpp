#include "smdocumenttreeview.h"
#include <QDragMoveEvent>
#include <QDebug>

SMDocumentTreeView::SMDocumentTreeView(QWidget *parent) :
    QTreeView(parent)
{
}

void SMDocumentTreeView::mousePressEvent(QMouseEvent *event)
{
    // Invoke the superclass method.
    QTreeView::mousePressEvent(event);

    qDebug() << "mouse pressed: " << event;
    /*
    QRect square = targetSquare(event->pos());
    int found = findPiece(square);

    if (found == -1)
        return;

    QPoint location = pieceLocations[found];
    QPixmap pixmap = piecePixmaps[found];
    pieceLocations.removeAt(found);
    piecePixmaps.removeAt(found);
    pieceRects.removeAt(found);

    if (location == QPoint(square.x()/80, square.y()/80))
        inPlace--;

    update(square);

    QByteArray itemData;
    QDataStream dataStream(&itemData, QIODevice::WriteOnly);

    dataStream << pixmap << location;

    QMimeData *mimeData = new QMimeData;
    mimeData->setData("image/x-puzzle-piece", itemData);

    QDrag *drag = new QDrag(this);
    drag->setMimeData(mimeData);
    drag->setHotSpot(event->pos() - square.topLeft());
    drag->setPixmap(pixmap);

    if (drag->start(Qt::MoveAction) == 0) {
        pieceLocations.insert(found, location);
        piecePixmaps.insert(found, pixmap);
        pieceRects.insert(found, square);
        update(targetSquare(event->pos()));

        if (location == QPoint(square.x()/80, square.y()/80))
            inPlace++;
    }*/
}

void SMDocumentTreeView::dragMoveEvent(QDragMoveEvent *event)
{
    // Invoke the superclass method.
    QTreeView::dragMoveEvent(event);

    qDebug() << "MOVING";

    /*
    QRect updateRect = highlightedRect.unite(targetSquare(event->pos()));

    if (event->mimeData()->hasFormat("image/x-puzzle-piece")
        && findPiece(targetSquare(event->pos())) == -1) {

        highlightedRect = targetSquare(event->pos());
        event->setDropAction(Qt::MoveAction);
        event->accept();
    } else {
        highlightedRect = QRect();
        event->ignore();
    }

    update(updateRect);*/
}
