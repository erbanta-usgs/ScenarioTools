#ifndef MODELELEMENTDIALOGRIV_H
#define MODELELEMENTDIALOGRIV_H

#include <QDialog>

namespace Ui {
    class ModelElementDialogRIV;
}

class ModelElementRIV;

class ModelElementDialogRIV : public QDialog
{
    Q_OBJECT

public:
    explicit ModelElementDialogRIV(ModelElementRIV *modelElement, QWidget *parent = 0);
    ~ModelElementDialogRIV();

private:
    Ui::ModelElementDialogRIV *ui;
    ModelElementRIV *_modelElement;
};

#endif // MODELELEMENTDIALOGRIV_H
