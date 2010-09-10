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

extern "C" _AnomalousExport void WindowFunctions_pumpMessages()
{
	MSG  msg;
	while( PeekMessage( &msg, NULL, 0U, 0U, PM_REMOVE ) )
	{
		TranslateMessage( &msg );
		DispatchMessage( &msg );
	}
}

#endif

#ifdef MAC_OSX

#include <Carbon/Carbon.h>

extern "C" _AnomalousExport int WindowFunctions_changeWindowIcon(void* hwnd, int iconID)
{
	return 0;
}

extern "C" _AnomalousExport int WindowFunctions_maximizeWindow(void* hwnd)
{
	return 0;
}

extern "C" _AnomalousExport void WindowFunctions_pumpMessages()
{
	// OSX Message Pump
	EventRef event = NULL;
	EventTargetRef targetWindow;
	targetWindow = GetEventDispatcherTarget();
	
	// If we are unable to get the target then we no longer care about events.
	if( !targetWindow ) return false;
	
	// Grab the next event, process it if it is a window event
	if( ReceiveNextEvent( 0, NULL, kEventDurationNoWait, true, &event ) == noErr )
	{
		// Dispatch the event
		SendEventToEventTarget( event, targetWindow );
		ReleaseEvent( event );
	}
}

#endif