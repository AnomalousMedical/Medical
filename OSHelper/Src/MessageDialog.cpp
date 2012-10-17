#include "StdAfx.h"
#include "NativeDialog.h"
#include "NativeOSWindow.h"

#ifdef WINDOWS

extern "C" _AnomalousExport void MessageDialog_showErrorDialog(NativeOSWindow* parent, String msg, String cap)
{
	HWND hWnd = NULL;
	if(parent != NULL)
	{
		hWnd = (HWND)parent->getHandle();
	}
	MessageBox(hWnd, msg, cap, MB_OK | MB_ICONEXCLAMATION);
}

extern "C" _AnomalousExport NativeDialogResult MessageDialog_showQuestionDialog(NativeOSWindow* parent, String msg, String cap)
{
	HWND hWnd = NULL;
	if(parent != NULL)
	{
		hWnd = (HWND)parent->getHandle();
	}
	switch(MessageBox(hWnd, msg, cap, MB_YESNO | MB_ICONQUESTION))
	{
		case IDOK:
			return OK;
		case IDYES:
			return YES;
		case IDNO:
			return NO;
		default:
			return CANCEL;
	}
}

#endif