#include "StdAfx.h"
#include "NativeOSWindow.h"
#include "Enums.h"
#include "NativeString.h"
#include "NativeStringEnumerator.h"

#ifdef USE_WXWIDGETS

extern "C" _AnomalousExport wxFileDialog* FileOpenDialog_new(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple)
{
	int style = wxFD_OPEN | wxFD_FILE_MUST_EXIST;
	if(selectMultiple)
	{
		style |= wxFD_MULTIPLE;
	}
	return new wxFileDialog(parent, message, defaultDir, defaultFile, wildcard, style);
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

extern "C" _AnomalousExport void FileOpenDialog_getPaths(wxFileDialog* fileDialog, NativeStringEnumerator* nativeStringEnumerator)
{
	fileDialog->GetPaths(nativeStringEnumerator->getArrayString());
}

#endif

#ifndef USE_WXWIDGETS

#include "FileOpenDialog.h"

FileOpenDialog::FileOpenDialog(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple)
	:parent(parent),
	message(message),
	defaultDir(defaultDir),
	defaultFile(defaultFile),
	wildcard(wildcard),
	selectMultiple(selectMultiple)
{

}

FileOpenDialog::~FileOpenDialog()
{

}

//NativeDialogResult FileOpenDialog::showModal();, this is implemented in Win32FileOpenDialog.cpp or CocoaFileDialog.mm depending on os

void FileOpenDialog::setWildcard(String wildcard)
{
	this->wildcard = wildcard;
}

String FileOpenDialog::getWildcard()
{
	return wildcard.c_str();
}

void FileOpenDialog::setPath(String path)
{
	paths.clear();
	paths.push_back(path);
}

String FileOpenDialog::getPath()
{
	if(paths.size() > 0)
	{
		return paths[0].c_str();
	}
	return 0;
}

int FileOpenDialog::getNumPaths()
{
	return paths.size();
}

String FileOpenDialog::getPath(int index)
{
	if(index < paths.size())
	{
		return paths[index].c_str();
	}
	return 0;
}

//PInvoke
extern "C" _AnomalousExport FileOpenDialog* FileOpenDialog_new(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple)
{
	return new FileOpenDialog(parent, message, defaultDir, defaultFile, wildcard, selectMultiple);
}

extern "C" _AnomalousExport void FileOpenDialog_delete(FileOpenDialog* fileDialog)
{
	delete fileDialog;
}

extern "C" _AnomalousExport NativeDialogResult FileOpenDialog_showModal(FileOpenDialog* fileDialog)
{
    return fileDialog->showModal();
}

extern "C" _AnomalousExport void FileOpenDialog_setWildcard(FileOpenDialog* fileDialog, String value)
{
	fileDialog->setWildcard(value);
}

extern "C" _AnomalousExport String FileOpenDialog_getWildcard(FileOpenDialog* fileDialog)
{
	return fileDialog->getWildcard();
}

extern "C" _AnomalousExport void FileOpenDialog_setPath(FileOpenDialog* fileDialog, String value)
{
	fileDialog->setPath(value);
}

extern "C" _AnomalousExport String FileOpenDialog_getPath(FileOpenDialog* fileDialog)
{
	return fileDialog->getPath();
}

extern "C" _AnomalousExport int FileOpenDialog_getNumPaths(FileOpenDialog* fileDialog)
{
	return fileDialog->getNumPaths();
}

extern "C" _AnomalousExport String FileOpenDialog_getPathIndex(FileOpenDialog* fileDialog, int index)
{
	return fileDialog->getPath(index);
}

#endif