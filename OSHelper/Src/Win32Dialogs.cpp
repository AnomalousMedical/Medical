#include "StdAfx.h"

#include "FileOpenDialog.h"
#include "FileSaveDialog.h"
#include "DirDialog.h"
#include "ColorDialog.h"
#include "NativeOSWindow.h"

#include <shlobj.h>
#include "Commdlg.h"
#define FILE_NAME_BUFFER_SIZE 65534

//Wildcard, these are in the format description|extension|description|extension
void convertWildcards(const std::string& wildcard, std::string& filterBuffer)
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
}

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

	//Title
	if(message.length() > 0)
	{
		of.lpstrFileTitle = const_cast<char *>(message.c_str());
	}

	//Default dir
	if(defaultDir.length() > 0)
	{
		of.lpstrInitialDir = defaultDir.c_str();
	}

	//Wildcard, these are in the format description|extension|description|extension
	std::string filterBuffer;
	if(wildcard.length() > 0)
	{
		convertWildcards(wildcard, filterBuffer);
		of.lpstrFilter = filterBuffer.c_str();
	}

	//Flags
	DWORD flags = OFN_EXPLORER | OFN_HIDEREADONLY | OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST | OFN_ENABLESIZING;
	if(selectMultiple)
	{
		flags |= OFN_ALLOWMULTISELECT;
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

NativeDialogResult FileSaveDialog::showModal()
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

	//Title
	if(message.length() > 0)
	{
		of.lpstrFileTitle = const_cast<char *>(message.c_str());
	}

	//Default dir
	if(defaultDir.length() > 0)
	{
		of.lpstrInitialDir = defaultDir.c_str();
	}

	//Wildcard, these are in the format description|extension|description|extension
	std::string filterBuffer;
	if(wildcard.length() > 0)
	{
		convertWildcards(wildcard, filterBuffer);
		of.lpstrFilter = filterBuffer.c_str();
	}

	//Flags
	DWORD flags = OFN_EXPLORER | OFN_HIDEREADONLY | OFN_PATHMUSTEXIST | OFN_ENABLESIZING;
	of.Flags = flags;

	path = "";
	//Show Dialog
	if(GetSaveFileName(&of))
	{
		path = of.lpstrFile;
		return OK;
	}

	return CANCEL;
}

NativeDialogResult DirDialog::showModal()
{
	char path[MAX_PATH] = "";

	LPITEMIDLIST pidl     = NULL;
	BROWSEINFO   bi       = { 0 };
	BOOL         bResult  = FALSE;

	if(parent != 0)
	{
		bi.hwndOwner = (HWND)parent->getHandle();
	}
	bi.pszDisplayName = path;
	bi.pidlRoot       = NULL;
	bi.lpszTitle      = message.c_str();
	bi.ulFlags        = BIF_RETURNONLYFSDIRS | BIF_USENEWUI;

	if ((pidl = SHBrowseForFolder(&bi)) != NULL)
	{
		bResult = SHGetPathFromIDList(pidl, path);
		this->path = path;
		CoTaskMemFree(pidl);
	}

	if(bResult)
	{
		return OK;
	}
	return CANCEL;
}

NativeDialogResult ColorDialog::showModal()
{
	// initialize the struct used by Windows
    CHOOSECOLOR cc;
    ZeroMemory(&cc, 0, sizeof(cc));
    cc.lStructSize = sizeof(cc);

    if (parent != 0)
	{
        cc.hwndOwner = (HWND)parent->getHandle();
	}

    cc.rgbResult = RGB((byte)(255 * color.r), (byte)(255 * color.g), (byte)(255 * color.b));

	static COLORREF custColors[16];
	static bool firstRun = true;

	if(firstRun)
	{
		for(int i = 0; i < 16; ++i)
		{
			custColors[i] = RGB(0, 0, 0);
		}
		firstRun = false;
	}

	cc.lpCustColors = custColors;

    cc.Flags = CC_RGBINIT;
    if (ChooseColor(&cc))
    {
		color.r = GetRValue(cc.rgbResult) / 255.0f;
		color.g = GetGValue(cc.rgbResult) / 255.0f;
		color.b = GetBValue(cc.rgbResult) / 255.0f;
        return OK;
    }
	return CANCEL;
}