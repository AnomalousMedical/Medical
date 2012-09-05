#pragma once

#include "Enums.h"

#include <string>
#include <vector>

class NativeOSWindow;

class FileOpenDialog
{
public:
	FileOpenDialog(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple);

	virtual ~FileOpenDialog();

	NativeDialogResult showModal();

	void setWildcard(String wildcard);

	String getWildcard();

	void setPath(String path);

	String getPath();

	int getNumPaths();

	String getPath(int index);

private:
	NativeOSWindow* parent;
	std::string message;
	std::string defaultDir;
	std::string defaultFile;
	std::string wildcard;
	bool selectMultiple;

	std::vector<std::string> paths;
};