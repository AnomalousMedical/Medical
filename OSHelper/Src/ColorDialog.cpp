#include "stdafx.h"
#include <wx/colordlg.h>
#include "Enums.h"

extern "C" _AnomalousExport wxColourDialog* ColorDialog_new()
{
	return new wxColourDialog();
}

extern "C" _AnomalousExport void ColorDialog_delete(wxColourDialog* colorDialog)
{
	delete colorDialog;
}

extern "C" _AnomalousExport NativeDialogResult ColorDialog_showModal(wxColourDialog* colorDialog)
{
	return interpretResults(colorDialog->ShowModal());
}

extern "C" _AnomalousExport Color ColorDialog_getColor(wxColourDialog* colorDialog)
{
	return colorDialog->GetColourData().GetColour();
}

extern "C" _AnomalousExport void ColorDialog_setColor(wxColourDialog* colorDialog, Color color)
{
	colorDialog->GetColourData().SetColour(color.toWx());
}