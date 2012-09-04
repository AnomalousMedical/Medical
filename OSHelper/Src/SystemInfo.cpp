#include "stdafx.h"

#ifdef USE_WXWIDGETS

#include <wx/display.h>

extern "C" _AnomalousExport uint SystemInfo_getDisplayCount()
{
	return wxDisplay::GetCount();
}

extern "C" _AnomalousExport void SystemInfo_getDisplayLocation(int displayIndex, int& x, int& y)
{
	wxDisplay display(displayIndex);
	wxRect geometry = display.GetGeometry();
	x = geometry.x;
	y = geometry.y;
}

#endif

#ifndef USE_WXWIDGETS

extern "C" _AnomalousExport uint SystemInfo_getDisplayCount()
{
	return 1;
}

extern "C" _AnomalousExport void SystemInfo_getDisplayLocation(int displayIndex, int& x, int& y)
{
	x = 0;
	y = 0;
}

#endif