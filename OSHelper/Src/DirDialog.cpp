#include "Stdafx.h"
#include "Enums.h"
#include "NativeString.h"
#include "NativeOSWindow.h"

#ifdef USE_WXWIDGETS

extern "C" _AnomalousExport wxDirDialog* DirDialog_new(NativeOSWindow* parent, String message, String startPath)
{
	return new wxDirDialog(parent, wxString::FromAscii(message), wxString::FromAscii(startPath));
}

extern "C" _AnomalousExport void DirDialog_delete(wxDirDialog* dirDialog)
{
	delete dirDialog;
}

extern "C" _AnomalousExport NativeDialogResult DirDialog_showModal(wxDirDialog* dirDialog)
{
	return interpretResults(dirDialog->ShowModal());
}

extern "C" _AnomalousExport void DirDialog_setPath(wxDirDialog* dirDialog, String path)
{
	dirDialog->SetPath(path);
}

extern "C" _AnomalousExport NativeString* DirDialog_getPath(wxDirDialog* dirDialog)
{
	return new NativeString(dirDialog->GetPath());
}

#endif