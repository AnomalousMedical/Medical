#include "StdAfx.h"

#include "NativeOSWindow.h"
#include "NativeDialog.h"
#include "AnomalousRTFramework.h"

////Wildcard, these are in the format description|extension|description|extension
//void convertWildcards(const StdString& wildcard, StdString& filterBuffer)
//{
//	size_t pos = 0;
//	pos = wildcard.find('|');
//	if(pos == StdString::npos)
//	{
//		//Consider the whole string the filter
//		filterBuffer = wildcard + L'\0' + wildcard + L'\0';
//	}
//	else
//	{
//		int pipeCount = 0;
//		filterBuffer.assign(wildcard);
//		do
//		{
//			++pipeCount;
//			filterBuffer[pos] = L'\0';
//			pos = wildcard.find('|', pos + 1);
//		}
//		while(pos != StdString::npos);
//		//If the last character was not a pipe make sure to add a trailing null
//		if(wildcard.rfind('|') + 1 != wildcard.length())
//		{
//			filterBuffer += L'\0';
//			++pipeCount;
//		}
//		//Make sure we have an even set of pairs
//		if(pipeCount % 2 != 0)
//		{
//			filterBuffer += L"*.*";
//			filterBuffer += L'\0';
//		}
//	}
//}

extern "C" _AnomalousExport void FileOpenDialog_showModal(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple, FileOpenDialogSetPathString setPathString, FileOpenDialogResultCallback resultCallback)
{
	
}

extern "C" _AnomalousExport void FileSaveDialog_showModal(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, FileSaveDialogResultCallback resultCallback)
{
	
}

extern "C" _AnomalousExport void DirDialog_showModal(NativeOSWindow* parent, String message, String startPath, DirDialogResultCallback resultCallback)
{
	
}

extern "C" _AnomalousExport void ColorDialog_showModal(NativeOSWindow* parent, Color color, ColorDialogResultCallback resultCallback)
{
	
}