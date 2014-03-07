#include "StdAfx.h"

#include "NativeOSWindow.h"
#include "NativeDialog.h"
#include <string>
typedef std::wstring StdString;

#include <shlobj.h>
#include "Commdlg.h"
#define FILE_NAME_BUFFER_SIZE 65534

//Wildcard, these are in the format description|extension|description|extension
void convertWildcards(const StdString& wildcard, StdString& filterBuffer)
{
	size_t pos = 0;
	pos = wildcard.find('|');
	if(pos == StdString::npos)
	{
		//Consider the whole string the filter
		filterBuffer = wildcard + L'\0' + wildcard + L'\0';
	}
	else
	{
		int pipeCount = 0;
		filterBuffer.assign(wildcard);
		do
		{
			++pipeCount;
			filterBuffer[pos] = L'\0';
			pos = wildcard.find('|', pos + 1);
		}
		while(pos != StdString::npos);
		//If the last character was not a pipe make sure to add a trailing null
		if(wildcard.rfind('|') + 1 != wildcard.length())
		{
			filterBuffer += L'\0';
			++pipeCount;
		}
		//Make sure we have an even set of pairs
		if(pipeCount % 2 != 0)
		{
			filterBuffer += L"*.*";
			filterBuffer += L'\0';
		}
	}
}

extern "C" _AnomalousExport void FileOpenDialog_showModal(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple, FileOpenDialogSetPathString setPathString, FileOpenDialogResultCallback resultCallback)
{
	OPENFILENAME of;
	ZeroMemory(&of, sizeof(of));
	of.lStructSize = sizeof(of);

	//Result buffer
	wchar_t fileNames[FILE_NAME_BUFFER_SIZE] = L"";
	of.lpstrFile = fileNames;
	of.nMaxFile = FILE_NAME_BUFFER_SIZE;

	//Parent Window
	if(parent != 0)
	{
		of.hwndOwner = (HWND)parent->getHandle();
	}

	//Title
	if(message != 0)
	{
		of.lpstrFileTitle = const_cast<wchar_t *>(message);
	}

	//Default dir
	if(defaultDir != 0)
	{
		of.lpstrInitialDir = defaultDir;
	}

	//Wildcard, these are in the format description|extension|description|extension
	StdString filterBuffer;
	if(wildcard != 0)
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

	NativeDialogResult dlgResult = CANCEL;
	//Show Dialog
	if(GetOpenFileName(&of))
	{
		//Determine how many files were selected
		bool multipleFiles = (of.lpstrFile[of.nFileOffset - 1] == '\0');
		if(multipleFiles)
		{
			StdString dirPath;
			StdString file;
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
				setPathString((dirPath + file).c_str());
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
			StdString file(of.lpstrFile);
			setPathString(file.c_str());
		}
		dlgResult = OK;
	}

	resultCallback(dlgResult);
}

extern "C" _AnomalousExport void FileSaveDialog_showModal(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, FileSaveDialogResultCallback resultCallback)
{
	OPENFILENAME of;
	ZeroMemory(&of, sizeof(of));
	of.lStructSize = sizeof(of);

	//Result buffer
	wchar_t fileNames[FILE_NAME_BUFFER_SIZE] = L"";
	if(defaultFile != 0)
	{
		wcsncpy(fileNames, defaultFile, FILE_NAME_BUFFER_SIZE);
	}
	of.lpstrFile = fileNames;
	of.nMaxFile = FILE_NAME_BUFFER_SIZE;

	//Parent Window
	if(parent != 0)
	{
		of.hwndOwner = (HWND)parent->getHandle();
	}

	//Title
	if(message != 0)
	{
		of.lpstrTitle = message;
	}

	//Default dir
	if(defaultDir != 0)
	{
		of.lpstrInitialDir = defaultDir;
	}

	//Wildcard, these are in the format description|extension|description|extension
	StdString filterBuffer;
	if(wildcard != 0)
	{
		convertWildcards(wildcard, filterBuffer);
		of.lpstrFilter = filterBuffer.c_str();
	}

	//Flags
	DWORD flags = OFN_EXPLORER | OFN_HIDEREADONLY | OFN_PATHMUSTEXIST | OFN_ENABLESIZING;
	of.Flags = flags;

	NativeDialogResult result = CANCEL;
	StdString path;
	//Show Dialog
	if(GetSaveFileName(&of))
	{
		path = of.lpstrFile;
		result = OK;

		if(of.nFileExtension == 0 && of.nFilterIndex != 0)
		{
			int pairIndex = 0;
			int extPos = 0;
			int extEnd = 0;
			while(pairIndex < of.nFilterIndex)
			{
				extPos = filterBuffer.find(L'\0', extPos);
				if(extPos != StdString::npos)
				{
					++extPos;
					extEnd = filterBuffer.find(L'\0', extPos);
					if(extEnd != StdString::npos)
					{
						if(pairIndex == of.nFilterIndex - 1)
						{
							extPos = filterBuffer.find('.', extPos);
							if(extPos != StdString::npos)
							{
								path += filterBuffer.substr(extPos, extEnd - extPos);
								break;
							}
						}
						else
						{
							extPos = extEnd + 1;
							++pairIndex;
						}
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
		}
	}

	resultCallback(result, path.c_str());
}

extern "C" _AnomalousExport void DirDialog_showModal(NativeOSWindow* parent, String message, String startPath, DirDialogResultCallback resultCallback)
{
	wchar_t path[MAX_PATH] = L"";

	LPITEMIDLIST pidl     = NULL;
	BROWSEINFO   bi       = { 0 };
	BOOL         bResult  = FALSE;

	if(parent != 0)
	{
		bi.hwndOwner = (HWND)parent->getHandle();
	}
	bi.pszDisplayName = path;
	bi.pidlRoot       = NULL;
	bi.lpszTitle      = message;
	bi.ulFlags        = BIF_RETURNONLYFSDIRS | BIF_USENEWUI;

	StdString strPath;
	if ((pidl = SHBrowseForFolder(&bi)) != NULL)
	{
		bResult = SHGetPathFromIDList(pidl, path);
		strPath = path;
		CoTaskMemFree(pidl);
	}

	NativeDialogResult result = CANCEL;
	if(bResult)
	{
		result = OK;
	}
	resultCallback(result, strPath.c_str());
}

extern "C" _AnomalousExport void ColorDialog_showModal(NativeOSWindow* parent, Color color, ColorDialogResultCallback resultCallback)
{
	// initialize the struct used by Windows
    CHOOSECOLOR cc;
    ZeroMemory(&cc, sizeof(cc));
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

	NativeDialogResult result = CANCEL;
	Color resColor(0.0f, 0.0f, 0.0f);
    cc.Flags = CC_RGBINIT;
    if (ChooseColor(&cc))
    {
		resColor.r = GetRValue(cc.rgbResult) / 255.0f;
		resColor.g = GetGValue(cc.rgbResult) / 255.0f;
		resColor.b = GetBValue(cc.rgbResult) / 255.0f;
        result = OK;
    }

	resultCallback(result, resColor);
}