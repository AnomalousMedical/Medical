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

#include <string>
#include <vector>

class FileOpenDialog
{
public:
	FileOpenDialog(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple)
		:parent(parent),
		message(message),
		defaultDir(defaultDir),
		defaultFile(defaultFile),
		wildcard(wildcard),
		selectMultiple(selectMultiple)
	{

	}

	~FileOpenDialog()
	{

	}

	NativeDialogResult showModal();

	void setWildcard(String wildcard)
	{
		this->wildcard = wildcard;
	}

	String getWildcard()
	{
		return wildcard.c_str();
	}

	void setPath(String path)
	{
		paths.clear();
		paths.push_back(path);
	}

	String getPath()
	{
		if(paths.size() > 0)
		{
			return paths[0].c_str();
		}
		return 0;
	}

	int getNumPaths()
	{
		return paths.size();
	}

	String getPath(int index)
	{
		if(index < paths.size())
		{
			return paths[index].c_str();
		}
		return 0;
	}

private:
	NativeOSWindow* parent;
	std::string message;
	std::string defaultDir;
	std::string defaultFile;
	std::string wildcard;
	bool selectMultiple;

	std::vector<std::string> paths;
};

#ifdef WINDOWS

#include "Commdlg.h"
#define FILE_NAME_BUFFER_SIZE 1024

NativeDialogResult FileOpenDialog::showModal()
{
	OPENFILENAME of;
	ZeroMemory(&of, sizeof(of));
	of.lStructSize = sizeof(of);

	//Result buffer
	char fileNames[FILE_NAME_BUFFER_SIZE] = "";
	of.lpstrFile = fileNames;
	of.nMaxFile = FILE_NAME_BUFFER_SIZE;

	//Parent Window
	if(parent != 0)
	{
		of.hwndOwner = (HWND)parent->getHandle();
	}

	//Wildcard, these are in the format description|extension|description|extension
	std::string filterBuffer;
	if(wildcard.length() > 0)
	{
		size_t pos = 0;
		pos = wildcard.find('|');
		if(pos == std::string::npos)
		{
			//Consider the whole string the filter
			filterBuffer = wildcard + '\0' + wildcard + '\0';
		}
		else
		{
			int pipeCount = 0;
			filterBuffer.assign(wildcard);
			do
			{
				++pipeCount;
				filterBuffer[pos] = '\0';
				pos = wildcard.find('|', pos + 1);
			}
			while(pos != std::string::npos);
			//If the last character was not a pipe make sure to add a trailing null
			if(wildcard.rfind('|') + 1 != wildcard.length())
			{
				filterBuffer += '\0';
				++pipeCount;
			}
			//Make sure we have an even set of pairs
			if(pipeCount % 2 != 0)
			{
				filterBuffer += "*.*";
				filterBuffer += '\0';
			}
		}
		of.lpstrFilter = filterBuffer.c_str();
	}

	//Flags
	DWORD flags = OFN_HIDEREADONLY | OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST | OFN_ENABLESIZING;
	if(selectMultiple)
	{
		flags |= OFN_EXPLORER | OFN_ALLOWMULTISELECT;
	}
	of.Flags = flags;

	paths.clear();
	//Show Dialog
	if(GetOpenFileName(&of))
	{
		//Determine how many files were selected
		bool multipleFiles = (of.lpstrFile[of.nFileOffset - 1] == '\0');
		if(multipleFiles)
		{
			std::string dirPath;
			std::string file;
			dirPath.assign(of.lpstrFile, of.nFileOffset - 1);
			bool isRoot = dirPath[dirPath.length() - 1] == '\\';
			if(!isRoot)
			{
				dirPath += '\\';
			}

			//Read files
			int offset = of.nFileOffset;
			file.assign(&(of.lpstrFile[offset]));
			while(file.length() > 0)
			{
				paths.push_back(dirPath + file);
				offset += file.length() + 1;
				if(offset < FILE_NAME_BUFFER_SIZE)
				{
					file.assign(&(of.lpstrFile[offset]));
				}
				else
				{
					break;
				}
			}
		}
		else
		{
			paths.push_back(of.lpstrFile);
		}
		return OK;
	}

	return CANCEL;
}

#endif

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