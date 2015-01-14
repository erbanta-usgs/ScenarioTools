#ifndef SMDOCUMENTTREEVIEW_H
#define SMDOCUMENTTREEVIEW_H

#include <QTreeView>

class SMDocumentTreeView : public QTreeView
{
    Q_OBJECT
public:
    explicit SMDocumentTreeView(QWidget *parent = 0);

protected:
    void mousePressEvent(QMouseEvent *event);
    void dragMoveEvent(QDragMoveEvent *event);

signals:

public slots:

};

#endif // SMDOCUMENTTREEVIEW_H
