#include "Stdafx.h"
#include "Enums.h"
#include "NativeString.h"
#include "NativeOSWindow.h"
#include "DirDialog.h"

DirDialog::DirDialog(NativeOSWindow* parent, String message, String startPath)
	:parent(parent),
	message(message),
	startPath(startPath)
{

}

DirDialog::~DirDialog()
{

}

void DirDialog::setPath(String path)
{
	this->path = path;
}

String DirDialog::getPath()
{
	return path.c_str();
}

//PInvoke
extern "C" _AnomalousExport DirDialog* DirDialog_new(NativeOSWindow* parent, String message, String startPath)
{
	return new DirDialog(parent, message, startPath);
}

extern "C" _AnomalousExport void DirDialog_delete(DirDialog* dirDialog)
{
	delete dirDialog;
}

extern "C" _AnomalousExport NativeDialogResult DirDialog_showModal(DirDialog* dirDialog)
{
	return dirDialog->showModal();
}

extern "C" _AnomalousExport void DirDialog_setPath(DirDialog* dirDialog, String path)
{
	dirDialog->setPath(path);
}

extern "C" _AnomalousExport String DirDialog_getPath(DirDialog* dirDialog)
{
	return dirDialog->getPath();
}