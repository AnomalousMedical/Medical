#include "StdAfx.h"
#include "ColorDialog.h"

ColorDialog::ColorDialog(NativeOSWindow* parent)
	:parent(parent)
{

}

ColorDialog::~ColorDialog()
{

}

void ColorDialog::setColor(Color color)
{
	this->color = color;
}

Color ColorDialog::getColor()
{
	return color;
}

extern "C" _AnomalousExport ColorDialog* ColorDialog_new(NativeOSWindow* parent)
{
	return new ColorDialog(parent);
}

extern "C" _AnomalousExport void ColorDialog_delete(ColorDialog* colorDialog)
{
	delete colorDialog;
}

extern "C" _AnomalousExport NativeDialogResult ColorDialog_showModal(ColorDialog* colorDialog)
{
	return colorDialog->showModal();
}

extern "C" _AnomalousExport Color ColorDialog_getColor(ColorDialog* colorDialog)
{
	return colorDialog->getColor();
}

extern "C" _AnomalousExport void ColorDialog_setColor(ColorDialog* colorDialog, Color color)
{
	colorDialog->setColor(color);
}