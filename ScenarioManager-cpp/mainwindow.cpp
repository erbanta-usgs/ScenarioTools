#include "mainwindow.h"
#include "ui_mainwindow.h"

#include "smdocument.h"
#include "modelelementdis.h"
#include "modelelementriv.h"
#include "modelelementwel.h"
#include "modelelementdialogdis.h"
#include "modelelementdialogriv.h"
#include "modelelementdialogwel.h"
#include "dateutil.h"

#include "packageprocessorriv.h"

#include <QDebug>
#include <QMessageBox>

#include "shapepoint2d.h"
#include "shapepoint2dset.h"
#include "packageprocessorwel.h"

#define NUM_INFO_LINES 10
#define NUM_PROGRESS_LINES 6

MainWindow::MainWindow(QWidget *parent) : QMainWindow(parent), ui(new Ui::MainWindow)
{
    ui->setupUi(this);

    // Hide the progress window.
    QList<int> sizes;
    sizes.append(700);
    sizes.append(0);
    ui->splitter->setSizes(sizes);

    // Store the package labels in the label array.
    labels[0] = ui->label;
    labels[1] = ui->label_2;
    labels[2] = ui->label_3;
    labels[3] = ui->label_4;
    labels[4] = ui->label_5;
    labels[5] = ui->label_6;
    labels[6] = ui->label_7;
    labels[7] = ui->label_8;
    labels[8] = ui->label_9;
    labels[9] = ui->label_10;

    // Clear the package labels.
    for (int i = 0; i < 10; i++) {
        labels[i]->setText("");
    }

    // Zero the package-processing counter.
    numPackagesProcessing = 0;

    // Connect the tree widget signals to the appropriate main-window slots.
    // TODO: Move this connection to the new form designer.
    connect(ui->treeWidget, SIGNAL(itemClicked(QTreeWidgetItem*,int)), this, SLOT(treeItemClicked(QTreeWidgetItem*,int)));
    connect(ui->treeWidget, SIGNAL(itemSelectionChanged()), this, SLOT(refreshSelectedTreeItem()));
    connect(ui->treeWidget, SIGNAL(itemDoubleClicked(QTreeWidgetItem*,int)), this, SLOT(treeItemDoubleClicked(QTreeWidgetItem*,int)));

    // Connect the WEL button to the appropriate main-window slot.
    connect(ui->actionNewWEL, SIGNAL(activated()), this, SLOT(addNewWEL()));

    // Connect the Export menu items to the appropriate slots.
    connect(ui->actionWell_Package, SIGNAL(triggered()), this, SLOT(exportPackageWEL()));

    // Connect the Window menu items to the appropriate slots.
    connect(ui->actionProgress, SIGNAL(triggered()), this, SLOT(toggleProgressWindow()));
    connect(ui->splitter, SIGNAL(splitterMoved(int,int)), this, SLOT(splitterSizeChanged(int, int)));

    // Add a default discretization.
    QTreeWidgetItem *defaultDiscretizationItem = new QTreeWidgetItem();
    ModelElementDIS *discretization = ModelElementDIS::createDefaultDiscretization();
    defaultDiscretizationItem->setText(0, discretization->name());
    defaultDiscretizationItem->setData(0, Qt::UserRole, (int)discretization);
    ui->treeWidget->addTopLevelItem(defaultDiscretizationItem);
}
void MainWindow::splitterSizeChanged(int topSize, int bottomSize) {
    // Get the splitter sizes.
    ui->actionProgress->setChecked(ui->splitter->sizes().at(1) > 0);
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::addNewDIS() {
    qDebug() << "going to add a new DIS";

    // Make a new DIS element and show a dialog for it. Only continue if the dialog is accepted.
    ModelElementDIS *element = new ModelElementDIS();

    // Make and show the dialog. Only continue if the dialog is accepted.
    ModelElementDialogDIS *dialog = new ModelElementDialogDIS(element);
    if (dialog->exec() == QDialog::Accepted) {
        qDebug() << "the dialog was accepted";

        qDebug() << "the element name is " << element->name();

        QTreeWidgetItem *item = new QTreeWidgetItem();
        item->setText(0, element->name());
        item->setData(0, Qt::UserRole, (int)element);
        ui->treeWidget->addTopLevelItem(item);

        qDebug() << "added item: " << element->name();
    }
}

void MainWindow::addNewRIV() {
    // Make a new RIV element and show a dialog for it. Only continue if the dialog is accepted.
    ModelElementRIV *element = new ModelElementRIV();

    // Make and show the dialog. Only continue if the dialog is accepted.
    ModelElementDialogRIV *dialog = new ModelElementDialogRIV(element);
    if (dialog->exec() == QDialog::Accepted) {
        // Add the map to the document.
        _document->addModelElement(element);

        // Update the tree.
        //ui->treeWidget->update();
    }
}

void MainWindow::addNewSWR() {
}

void MainWindow::addNewWEL() {
    // Make a new WEL element and show a dialog for it. Only continue if the dialog is accepted.
    ModelElementWEL *element = new ModelElementWEL();

    // Make and show the dialog. Only continue if the dialog is accepted.
    ModelElementDialogWEL *dialog = new ModelElementDialogWEL(element);
    if (dialog->exec() == QDialog::Accepted) {
        qDebug() << "The dialog was accepted. The element name is " << element->name() << ".";

        QTreeWidgetItem *item = new QTreeWidgetItem();
        item->setText(0, element->name());
        item->setData(0, Qt::UserRole, (int)element);
        ui->treeWidget->addTopLevelItem(item);

        qDebug() << "added item: " << element->name();

        ui->treeWidget->setCurrentItem(item);
    }

    // Free the dialog.
    delete dialog;

    // Refresh the selected tree item.
    this->refreshSelectedTreeItem();
}
void MainWindow::refreshSelectedTreeItem() {
    // Get the selected tree items. If fewer or more than one is selected, clear the right panel and return.
    QList<QTreeWidgetItem *> items = ui->treeWidget->selectedItems();
    if (items.length() != 1) {
        for (int i = 0; i < 10; i++) {
            labels[i]->setText("");
        }

        return;
    }

    // Get the model element from the item.
    QVariant v = items.at(0)->data(0, Qt::UserRole);
    ModelElement *element = (ModelElement *)v.value<int>();

    qDebug() << "element " << element->name() << " was clicked";

    // Get the information from the tree item.
    QList<QString> information = element->information();

    // Populate the labels with the information.
    for (int i = 0; i < information.size(); i++) {
        labels[i]->setText(information.at(i));
    }

    // Clear the rest of the labels.
    for (int i = information.size(); i < 10; i++) {
        labels[i]->setText("");
    }

    // Refresh the name in the tree item.
    items.at(0)->setText(0, element->name());
}

void MainWindow::treeItemClicked(QTreeWidgetItem *item, int column) {
    this->refreshSelectedTreeItem();
}

void MainWindow::treeItemDoubleClicked(QTreeWidgetItem *item, int column) {
    // Get the model element from the item.
    QVariant v = item->data(0, Qt::UserRole);
    ModelElement *element = (ModelElement *)v.value<int>();

    qDebug() << "element " << element->name() << " was double-clicked";

    // Show the appropriate dialog, depending on the type of the element.
    // More types will be added as the dialogs are supported.
    int type = element->modelElementType();
    qDebug() << "the element type is " << type;
    if (type == MODEL_ELEMENT_TYPE_WEL) {
        // Make and show the dialog.
        ModelElementDialogWEL *dialog = new ModelElementDialogWEL((ModelElementWEL *)element);
        dialog->exec();

        // Free the dialog.
        delete dialog;
    }

    // Refresh the selected tree item.
    this->refreshSelectedTreeItem();
}

void MainWindow::exportPackageWEL() {
    qDebug() << "going to export a well package";

    QList<QString> shapefilePaths;
    QList<QString> sampleFilePaths;
    QList<QString> keyFields;

    // Get all WEL model elements in the list.
    int numItems = ui->treeWidget->topLevelItemCount();
    QList<ModelElementWEL *> wellItems;
    for (int i = 0; i < numItems; i++) {
        QTreeWidgetItem *treeWidget = ui->treeWidget->topLevelItem(i);
        ModelElement *element = (ModelElement *)treeWidget->data(0, Qt::UserRole).value<int>();
        if (element->modelElementType() == MODEL_ELEMENT_TYPE_WEL) {
            ModelElementWEL *elementWEL = (ModelElementWEL *)element;

            wellItems.append((ModelElementWEL *)element);

            shapefilePaths.append(elementWEL->shapefilePath());
            sampleFilePaths.append(elementWEL->sampleFilePath());
            keyFields.append(elementWEL->keyField());
        }
    }

    // If there are no well packages configured, show a message and return.
    if (wellItems.size() == 0) {
        this->processingEnded("No Well Packages configured");
        return;
    }

    // Get the discretization.
    ModelElementDIS *dis = NULL;
    for (int i = 0; i < numItems && dis == NULL; i++) {
        QTreeWidgetItem *treeWidget = ui->treeWidget->topLevelItem(i);
        ModelElement *element = (ModelElement *)treeWidget->data(0, Qt::UserRole).value<int>();
        if (element->modelElementType() == MODEL_ELEMENT_TYPE_DIS) {
            qDebug() << "YAY -- it's a discretization file!";

            dis = (ModelElementDIS *)element;
        }
    }

    // Make the package processor.
    PackageProcessorWEL *ppw = new PackageProcessorWEL(shapefilePaths, sampleFilePaths, dis, keyFields, "out.shp", "out.wel");

    // Connect the signals and slots for updating the status of the processing.
    connect(ppw, SIGNAL(processingBegan(QString)), this, SLOT(processingBegan(QString)));
    connect(ppw, SIGNAL(processingContinues(QString)), this, SLOT(processingContinues(QString)));
    connect(ppw, SIGNAL(processingEnded(QString)), this, SLOT(processingEnded(QString)));

    // Start the package processor.
    ppw->start();

    qDebug() << "completed exporting well package";
}

void MainWindow::toggleProgressWindow() {
    // If the size of the bottom is currently zero (hidden), set it to 100 (unhidden).
    if (ui->splitter->sizes().at(1) == 0) {
        QList<int> sizes;
        sizes.append(500);
        sizes.append(100);
        ui->splitter->setSizes(sizes);
    }
    else {
        QList<int> sizes;
        sizes.append(500);
        sizes.append(0);
        ui->splitter->setSizes(sizes);
    }
}

void MainWindow::processingBegan(QString message) {
    // Set the background color on the status bar and show the message.
    ui->statusBar->setStyleSheet("QStatusBar { background: #008000; }");
    ui->statusBar->showMessage(message);

    // Also, append the message to the text box.
    ui->textBrowser->append(message);
}

void MainWindow::processingContinues(QString message) {
    // Set the background color on the status bar and show the message.
    ui->statusBar->setStyleSheet("QStatusBar { background: #008000; }");
    ui->statusBar->showMessage(message);

    // Also, append the message to the text box.
    ui->textBrowser->append(message);
}

void MainWindow::processingEnded(QString message) {
    // Set the background color on the status bar and show the message.
    ui->statusBar->setStyleSheet("QStatusBar { }");
    ui->statusBar->showMessage(message);

    // Also, append the message to the text box.
    ui->textBrowser->append(message);
}
