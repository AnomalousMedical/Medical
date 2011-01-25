#include "StdAfx.h"

enum NativeDialogResult
{
    YES = 1,
    OK = 2,
    NO = 4,
    CANCEL = 8,
};

extern "C" _AnomalousExport void MessageDialog_showErrorDialog(String msg, String cap)
{
	wxMessageBox(wxString::FromAscii(msg), wxString::FromAscii(cap), wxOK | wxICON_ERROR);
}

extern "C" _AnomalousExport NativeDialogResult MessageDialog_showQuestionDialog(String msg, String cap)
{
	int result = wxMessageBox(wxString::FromAscii(msg), wxString::FromAscii(cap), wxYES_NO | wxICON_QUESTION);
	switch(result)
	{
		case wxID_OK:
			return OK;
		case wxID_YES:
			return YES;
		case wxID_NO:
			return NO;
		default:
			return CANCEL;
	}
}