#include "StdAfx.h"
#include "Win32Window.h"
#include <objbase.h>

BOOL APIENTRY DllMain(HANDLE hModule, DWORD ulReasonForCall, LPVOID lpReserved)
{
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