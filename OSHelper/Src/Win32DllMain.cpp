#include "StdAfx.h"
#include "Win32Window.h"

BOOL APIENTRY DllMain(HANDLE hModule, DWORD ulReasonForCall, LPVOID lpReserved)
{
	if (ulReasonForCall == DLL_PROCESS_ATTACH)
	{
		Win32Window::createWindowClass(hModule);
	}
	else if(ulReasonForCall == DLL_PROCESS_DETACH)
	{
		Win32Window::destroyWindowClass();
	}
	return TRUE;
}