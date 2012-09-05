#include "StdAfx.h"

#include "FileOpenDialog.h"
#include "NativeOSWindow.h"

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