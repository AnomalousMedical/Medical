#include "stdafx.h"
#include "MultiTouch.h"
#include "MultiTouchImpl.h"

#ifdef WINDOWS
#define MULTITOUCH 1
#endif

#ifdef MAC_OSX
#define MULTITOUCH 1
#endif

#if MULTITOUCH != 0

#ifdef WINDOWS

HMODULE mtDriver = NULL;
WINDOWS_REGISTRATION_FUNC registerWithWindows = NULL;

extern "C" _AnomalousExport MultiTouch* MultiTouch_new(HWND hwnd, TouchEventDelegate touchStartedCB, TouchEventDelegate touchEndedCB, TouchEventDelegate touchMovedCB, TouchEventCanceledDelegate touchCanceledCB)
{
	MultiTouch* multiTouch = new MultiTouchImpl(hwnd, touchStartedCB, touchEndedCB, touchMovedCB, touchCanceledCB);
	registerWithWindows(hwnd, multiTouch);
	return multiTouch;
}

extern "C" _AnomalousExport void MultiTouch_delete(MultiTouch* multiTouch)
{
	delete multiTouch;
}

extern "C" _AnomalousExport bool MultiTouch_isMultitouchAvailable()
{
	bool loaded = false;
	if(mtDriver == NULL)
	{
		mtDriver = LoadLibraryEx("WinMTDriver.dll", NULL, 0);
		if(mtDriver != NULL)
		{
			registerWithWindows = (WINDOWS_REGISTRATION_FUNC)GetProcAddress(mtDriver, "registerWithWindows");
			if(registerWithWindows != NULL)
			{
				loaded = true;
			}
		}
	}
	else if(registerWithWindows != NULL)
	{
		loaded = true;
	}
	return loaded;
}

#endif //WINDOWS

#ifdef MAC_OSX

#include "OSXMultiTouch.h"

extern "C" _AnomalousExport MultiTouch* MultiTouch_new(WindowType window, TouchEventDelegate touchStartedCB, TouchEventDelegate touchEndedCB, TouchEventDelegate touchMovedCB, TouchEventCanceledDelegate touchCanceledCB)
{
	MultiTouch* multiTouch = new MultiTouchImpl(window, touchStartedCB, touchEndedCB, touchMovedCB, touchCanceledCB);
	registerWithObjectiveC(window, multiTouch);
	return multiTouch;
}

extern "C" _AnomalousExport void MultiTouch_delete(MultiTouch* multiTouch)
{
	delete multiTouch;
}

extern "C" _AnomalousExport bool MultiTouch_isMultitouchAvailable()
{
	return true;
}

#endif //MAC_OSX

#else //Else from MULTITOUCH

extern "C" _AnomalousExport bool MultiTouch_isMultitouchAvailable()
{
	return false;
}

#endif //MULTITOUCH Else