#include "modelelementdialogdis.h"
#include "modelelementdis.h"
#include "ui_modelelementdialogdis.h"

#include <QDebug>

ModelElementDialogDIS::ModelElementDialogDIS(ModelElementDIS *modelElement, QWidget *parent) :
    QDialog(parent),
    ui(new Ui::ModelElementDialogDIS)
{
    // Setup the interface.
    ui->setupUi(this);

    // Store a reference to the model element.
    _modelElement = modelElement;
}

void ModelElementDialogDIS::accept() {
    // Invoke the superclass method.
    QDialog::accept();

    // Update the report element name.
    _modelElement->setName(ui->lineEditName->text());
}

ModelElementDialogDIS::~ModelElementDialogDIS()
{
    delete ui;
}
