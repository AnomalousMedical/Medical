#include "StdAfx.h"
#include "NativeOSWindow.h"
#include "NativeDialog.h"
#include "FileSaveDialog.h"

FileSaveDialog::FileSaveDialog(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard)
	:parent(parent),
	message(message),
	defaultDir(defaultDir),
	defaultFile(defaultFile),
	wildcard(wildcard)
{
	
}

FileSaveDialog::~FileSaveDialog()
{
	
}

void FileSaveDialog::setWildcard(String wildcard)
{
	wildcard = wildcard;
}

String FileSaveDialog::getWildcard()
{
	return wildcard.c_str();
}

void FileSaveDialog::setPath(String path)
{
	this->path = path;
}

String FileSaveDialog::getPath()
{
	return path.c_str();
}

//PInvoke
extern "C" _AnomalousExport FileSaveDialog* FileSaveDialog_new(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard)
{
	return new FileSaveDialog(parent, message, defaultDir, defaultFile, wildcard);
}

extern "C" _AnomalousExport void FileSaveDialog_delete(FileSaveDialog* fileDialog)
{
	delete fileDialog;
}

extern "C" _AnomalousExport NativeDialogResult FileSaveDialog_showModal(FileSaveDialog* fileDialog)
{
    return fileDialog->showModal();
}

extern "C" _AnomalousExport void FileSaveDialog_setWildcard(FileSaveDialog* fileDialog, String value)
{
	fileDialog->setWildcard(value);
}

extern "C" _AnomalousExport String FileSaveDialog_getWildcard(FileSaveDialog* fileDialog)
{
	return fileDialog->getWildcard();
}

extern "C" _AnomalousExport void FileSaveDialog_setPath(FileSaveDialog* fileDialog, String value)
{
	fileDialog->setPath(value);
}

extern "C" _AnomalousExport String FileSaveDialog_getPath(FileSaveDialog* fileDialog)
{
	return fileDialog->getPath();
}