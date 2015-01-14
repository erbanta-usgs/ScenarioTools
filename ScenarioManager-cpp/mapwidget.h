#ifndef MAPWIDGET_H
#define MAPWIDGET_H

#include <QWidget>

class Polyline2D;

class MapWidget : public QWidget
{
    Q_OBJECT
public:
    explicit MapWidget(QWidget *parent = 0);
    void setPolylines(Polyline2D **polylines, int numPolylines);

private:
    void paintEvent(QPaintEvent *);
    Polyline2D **polylines;
    int numPolylines;

signals:

public slots:

};

#endif // MAPWIDGET_H
