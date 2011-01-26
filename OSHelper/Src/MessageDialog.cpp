#include "StdAfx.h"
#include "Enums.h"

extern "C" _AnomalousExport void MessageDialog_showErrorDialog(String msg, String cap)
{
	wxMessageBox(wxString::FromAscii(msg), wxString::FromAscii(cap), wxOK | wxICON_ERROR);
}

extern "C" _AnomalousExport NativeDialogResult MessageDialog_showQuestionDialog(String msg, String cap)
{
	return interpretResults(wxMessageBox(wxString::FromAscii(msg), wxString::FromAscii(cap), wxYES_NO | wxICON_QUESTION));
	
}