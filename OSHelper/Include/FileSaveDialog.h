#pragma once

#include "Enums.h"

#include <string>

class NativeOSWindow;

class FileSaveDialog
{
public:
	FileSaveDialog(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard);

	virtual ~FileSaveDialog();

	NativeDialogResult showModal();

	void setWildcard(String wildcard);

	String getWildcard();

	void setPath(String path);

	String getPath();

private:
	NativeOSWindow* parent;
	std::string message;
	std::string defaultDir;
	std::string defaultFile;
	std::string wildcard;

	std::string path;
};