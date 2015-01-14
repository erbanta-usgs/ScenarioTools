#include "mapwidget.h"
#include <QPainter>
#include "polyline2d.h"

MapWidget::MapWidget(QWidget *parent) :
    QWidget(parent)
{
    this->polylines = NULL;
}

void MapWidget::setPolylines(Polyline2D **polylines, int numPolylines) {
    // Store the polylines.
    this->polylines = polylines;
    this->numPolylines = numPolylines;
}

void MapWidget::paintEvent(QPaintEvent *) {
    // Make the painter object.
    QPainter painter(this);

    // Set the pen and brush appropriately.
    painter.setPen(Qt::black);
    painter.setBrush(Qt::NoBrush);

    painter.drawEllipse(QPoint(0, 0), 50, 50);

    // Only continue if the polylines are not null.
    if (polylines != NULL) {
        painter.drawLine(0, 0, 10, 10);

        for (int i = 0; i < numPolylines; i++) {
            painter.drawLine(0, 0, 50, 10 * i);
        }
    }
}
