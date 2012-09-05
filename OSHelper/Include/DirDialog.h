#pragma once

#include "Enums.h"

#include <string>

class NativeOSWindow;

class DirDialog
{
public:
	DirDialog(NativeOSWindow* parent, String message, String startPath);

	virtual ~DirDialog();

	NativeDialogResult showModal();

	void setPath(String path);

	String getPath();

private:
	NativeOSWindow* parent;
	std::string message;
	std::string startPath;

	std::string path;
};