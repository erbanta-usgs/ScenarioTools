#ifndef MODELELEMENTDIALOGWEL_H
#define MODELELEMENTDIALOGWEL_H

#include <QDialog>

class ModelElementWEL;

namespace Ui {
    class ModelElementDialogWEL;
}

class ModelElementDialogWEL : public QDialog
{
    Q_OBJECT

public:
    explicit ModelElementDialogWEL(ModelElementWEL *modelElement, QWidget *parent = 0);
    ~ModelElementDialogWEL();

public slots:
    void shapefileBrowseButtonClicked();
    void sampleFileBrowseButtonClicked();

protected:
    void accept();

private:
    Ui::ModelElementDialogWEL *ui;
    ModelElementWEL *_modelElement;
};

#endif // MODELELEMENTDIALOGWEL_H
