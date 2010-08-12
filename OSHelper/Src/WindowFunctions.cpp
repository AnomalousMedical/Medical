#include "Stdafx.h"

enum WindowIcons
{
	ICON_SKULL,
};

#ifdef WINDOWS

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include "..\Resource.h"

extern "C" _AnomalousExport int WindowFunctions_changeWindowIcon(HWND hwnd, WindowIcons icon)
{
	int errorCode = 0;
	HMODULE module;
	if(!GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, reinterpret_cast<LPCTSTR>(WindowFunctions_changeWindowIcon), &module))
	{
		errorCode = GetLastError();
	}
	else
	{
		HICON hIcon = NULL;

		switch(icon)
		{
			case ICON_SKULL:
			default:
				hIcon = LoadIcon(module, MAKEINTRESOURCE(IDI_SKULL));
				break;
		}

		if(hIcon == NULL)
		{
			errorCode = GetLastError();
		}
		else if(!SetClassLongPtr(hwnd, GCL_HICON, (LONG_PTR)hIcon))
		{
			errorCode = GetLastError();
		}
		FreeModule(module);
	}
	return errorCode;
}

extern "C" _AnomalousExport int WindowFunctions_maximizeWindow(HWND hwnd)
{
	if(!ShowWindow(hwnd, SW_MAXIMIZE))
	{
		return GetLastError();
	}
	return 0;
}

#endif

#ifdef MAC_OSX

extern "C" _AnomalousExport int WindowFunctions_changeWindowIcon(void* hwnd, int iconID)
{
	return 0;
}

#endif