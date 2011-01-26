#include "StdAfx.h"
#include "NativeOSWindow.h"
#include "Enums.h"
#include "NativeString.h"

extern "C" _AnomalousExport wxFileDialog* FileOpenDialog_new(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard)
{
	return new wxFileDialog(parent, message, defaultDir, defaultFile, wildcard, wxFD_OPEN);
}

extern "C" _AnomalousExport void FileOpenDialog_delete(wxFileDialog* fileDialog)
{
	delete fileDialog;
}

extern "C" _AnomalousExport NativeDialogResult FileOpenDialog_showModal(wxFileDialog* fileDialog)
{
    return interpretResults(fileDialog->ShowModal());
}

extern "C" _AnomalousExport void FileOpenDialog_setWildcard(wxFileDialog* fileDialog, String value)
{
	fileDialog->SetWildcard(value);
}

extern "C" _AnomalousExport NativeString* FileOpenDialog_getWildcard(wxFileDialog* fileDialog)
{
	return new NativeString(fileDialog->GetWildcard());
}

extern "C" _AnomalousExport void FileOpenDialog_setPath(wxFileDialog* fileDialog, String value)
{
	fileDialog->SetPath(value);
}

extern "C" _AnomalousExport NativeString* FileOpenDialog_getPath(wxFileDialog* fileDialog)
{
	return new NativeString(fileDialog->GetPath());
}