#include "modelelementdialogwel.h"
#include "modelelementwel.h"
#include "ui_modelelementdialogwel.h"
#include "settings.h"

#include <QDebug>
#include <QFileDialog>
#include <QFile>

ModelElementDialogWEL::ModelElementDialogWEL(ModelElementWEL *modelElement, QWidget *parent) :
    QDialog(parent),
    ui(new Ui::ModelElementDialogWEL)
{
    // Setup the interface.
    ui->setupUi(this);

    // Store a reference to the model element.
    _modelElement = modelElement;

    // Populate the text boxes.
    ui->lineEditName->setText(_modelElement->name());
    ui->lineEditShapefile->setText(_modelElement->shapefilePath());
    ui->lineEditSampleFile->setText(_modelElement->sampleFilePath());
    ui->lineEditKeyField->setText(_modelElement->keyField());

    // Connect the signals and slots for the browse buttons.
    connect(ui->pushButtonShapefile, SIGNAL(clicked()), this, SLOT(shapefileBrowseButtonClicked()));
    connect(ui->pushButtonSampleFile, SIGNAL(clicked()), this, SLOT(sampleFileBrowseButtonClicked()));
}

void ModelElementDialogWEL::accept() {
    // Invoke the superclass method.
    QDialog::accept();

    // Update the report element name.
    _modelElement->setName(ui->lineEditName->text());

    // Update the shapefile and sample file paths.
    _modelElement->setShapefilePath(ui->lineEditShapefile->text());
    _modelElement->setSampleFilePath(ui->lineEditSampleFile->text());
    _modelElement->setKeyField(ui->lineEditKeyField->text());
}
void ModelElementDialogWEL::shapefileBrowseButtonClicked() {
    qDebug() << "browsing for a shapefile";

    // Display a dialog and get the filename.
    QString filename = QFileDialog::getOpenFileName(this, tr("Open File"), Settings::getValue(BROWSE_DIRECTORY), tr("Shapefiles (*.shp)"));
    qDebug() << "shapefile selected: " << filename;

    if (!filename.isNull()) {
       // Store the directory.
       QDir directory(filename);
       directory.cdUp();
       Settings::setValue(BROWSE_DIRECTORY, directory.absolutePath());

       ui->lineEditShapefile->setText(filename);
    }
}

void ModelElementDialogWEL::sampleFileBrowseButtonClicked() {
    qDebug() << "browsing for a sample file";

    // Display a dialog and get the filename.
    QString filename = QFileDialog::getOpenFileName(this, tr("Open File"), Settings::getValue(BROWSE_DIRECTORY), tr("Sample files (*.smp)"));
    qDebug() << "sample file selected: " << filename;

    // Set the filename in the textbox.
    if (!filename.isNull()) {
       ui->lineEditSampleFile->setText(filename);
    }
}

ModelElementDialogWEL::~ModelElementDialogWEL()
{
    delete ui;
}
