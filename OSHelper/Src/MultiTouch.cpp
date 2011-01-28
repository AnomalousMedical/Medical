#include "stdafx.h"
#include "MultiTouch.h"

#ifdef WINDOWS
#define MULTITOUCH WINVER > 0x600
#endif

#ifdef MAC_OSX
#define MULTITOUCH 1
#endif

#if MULTITOUCH != 0

#ifdef WINDOWS

#include "Windows7MultiTouch.h"

extern "C" _AnomalousExport MultiTouch* MultiTouch_new(HWND hwnd, TouchEventDelegate touchStartedCB, TouchEventDelegate touchEndedCB, TouchEventDelegate touchMovedCB)
{
	MultiTouch* multiTouch = new MultiTouch(hwnd, touchStartedCB, touchEndedCB, touchMovedCB);
	registerWithWindows(hwnd, multiTouch);
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

#endif //WINDOWS

#ifdef MAC_OSX

#include "OSXMultiTouch.h"

extern "C" _AnomalousExport void MultiTouch_registerMultiTouchEventHandler(void* hwnd)
{
	registerWithObjectiveC(hwnd);
}

extern "C" _AnomalousExport bool MultiTouch_isMultitouchAvailable()
{
	return true;
}

#endif //MAC_OSX

#else //Else from MULTITOUCH

extern "C" _AnomalousExport void MultiTouch_registerMultiTouchEventHandler(void* hwnd)
{

}

extern "C" _AnomalousExport bool MultiTouch_isMultitouchAvailable()
{
	return false;
}

#endif //MULTITOUCH Else