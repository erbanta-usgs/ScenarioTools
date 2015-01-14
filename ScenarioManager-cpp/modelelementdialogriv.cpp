#include "modelelementdialogriv.h"
#include "ui_modelelementdialogriv.h"

ModelElementDialogRIV::ModelElementDialogRIV(ModelElementRIV *modelElement, QWidget *parent) :
    QDialog(parent),
    ui(new Ui::ModelElementDialogRIV)
{
    // Setup the interface.
    ui->setupUi(this);

    // Store a reference to the model element.
    _modelElement = modelElement;
}

ModelElementDialogRIV::~ModelElementDialogRIV()
{
    delete ui;
}
