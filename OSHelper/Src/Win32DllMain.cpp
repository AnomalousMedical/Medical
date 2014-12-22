#include "StdAfx.h"
#include "Win32Window.h"
#include <objbase.h>

#pragma comment(linker, \
    "\"/manifestdependency:type='Win32' "\
    "name='Microsoft.Windows.Common-Controls' "\
    "version='6.0.0.0' "\
    "processorArchitecture='*' "\
    "publicKeyToken='6595b64144ccf1df' "\
    "language='*'\"")

BOOL APIENTRY DllMain(HANDLE hModule, DWORD ulReasonForCall, LPVOID lpReserved)
{
	SetErrorMode(SEM_FAILCRITICALERRORS);
	if (ulReasonForCall == DLL_PROCESS_ATTACH)
	{
		CoInitializeEx(NULL, COINIT_MULTITHREADED);
		Win32Window::createWindowClass(hModule);
	}
	else if(ulReasonForCall == DLL_PROCESS_DETACH)
	{
		Win32Window::destroyWindowClass();
		CoUninitialize();
	}
	return TRUE;
}