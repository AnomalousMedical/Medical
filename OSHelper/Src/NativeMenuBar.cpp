#include "StdAfx.h"

#ifdef USE_WXWIDGETS

extern "C" _AnomalousExport wxMenu* NativeMenuBar_createMenu(wxMenuBar* menuBar, String text)
{
	return new wxMenu();
}

extern "C" _AnomalousExport void NativeMenuBar_append(wxMenuBar* menuBar, wxMenu* nativeMenu, String text)
{
	menuBar->Append(nativeMenu, wxString::FromAscii(text));
}

#endif