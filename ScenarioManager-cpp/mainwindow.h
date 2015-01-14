#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QList>

#include "modelelement.h"

class SMDocument;
class QTreeWidgetItem;
class QLabel;

namespace Ui {
    class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow();

public slots:
    void addNewDIS();
    void addNewRIV();
    void addNewSWR();
    void addNewWEL();

    // These are the slots for the Export menu.
    void exportPackageWEL();

    // These are the slots for the Window menu.
    void toggleProgressWindow();
    void splitterSizeChanged(int topSize, int bottomSize);

    void treeItemClicked(QTreeWidgetItem *item, int column);
    void treeItemDoubleClicked(QTreeWidgetItem *item, int column);
    void refreshSelectedTreeItem();

    void processingBegan(QString message);
    void processingContinues(QString message);
    void processingEnded(QString message);

private:
    Ui::MainWindow *ui;
    SMDocument *_document;
    QLabel *labels[10];

    QList<ModelElement *> _elements;

    int numPackagesProcessing;
};

#endif // MAINWINDOW_H
