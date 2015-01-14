#ifndef MODELELEMENTDIALOGDIS_H
#define MODELELEMENTDIALOGDIS_H

#include <QDialog>

class ModelElementDIS;

namespace Ui {
    class ModelElementDialogDIS;
}

class ModelElementDialogDIS : public QDialog
{
    Q_OBJECT

public:
    explicit ModelElementDialogDIS(ModelElementDIS *modelElement, QWidget *parent = 0);
    ~ModelElementDialogDIS();

protected:
    void accept();

private:
    Ui::ModelElementDialogDIS *ui;
    ModelElementDIS *_modelElement;
};

#endif // MODELELEMENTDIALOGDIS_H
