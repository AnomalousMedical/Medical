#include "StdAfx.h"
#include "NativeOSWindow.h"
#include "Enums.h"
#include "NativeString.h"

extern "C" _AnomalousExport wxFileDialog* FileSaveDialog_new(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard)
{
	return new wxFileDialog(parent, message, defaultDir, defaultFile, wildcard, wxFD_SAVE);
}

extern "C" _AnomalousExport void FileSaveDialog_delete(wxFileDialog* fileDialog)
{
	delete fileDialog;
}

extern "C" _AnomalousExport NativeDialogResult FileSaveDialog_showModal(wxFileDialog* fileDialog)
{
    return interpretResults(fileDialog->ShowModal());
}

extern "C" _AnomalousExport void FileSaveDialog_setWildcard(wxFileDialog* fileDialog, String value)
{
	fileDialog->SetWildcard(value);
}

extern "C" _AnomalousExport NativeString* FileSaveDialog_getWildcard(wxFileDialog* fileDialog)
{
	return new NativeString(fileDialog->GetWildcard());
}

extern "C" _AnomalousExport void FileSaveDialog_setPath(wxFileDialog* fileDialog, String value)
{
	fileDialog->SetPath(value);
}

extern "C" _AnomalousExport NativeString* FileSaveDialog_getPath(wxFileDialog* fileDialog)
{
	return new NativeString(fileDialog->GetPath());
}