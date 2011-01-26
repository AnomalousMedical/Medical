#include "StdAfx.h"
#include "Enums.h"
#include "NativeOSWindow.h"

extern "C" _AnomalousExport void MessageDialog_showErrorDialog(NativeOSWindow* parent, String msg, String cap)
{
	wxMessageBox(wxString::FromAscii(msg), wxString::FromAscii(cap), wxOK | wxICON_ERROR, parent);
}

extern "C" _AnomalousExport NativeDialogResult MessageDialog_showQuestionDialog(NativeOSWindow* parent, String msg, String cap)
{
	return interpretResults(wxMessageBox(wxString::FromAscii(msg), wxString::FromAscii(cap), wxYES_NO | wxICON_QUESTION, parent));
	
}