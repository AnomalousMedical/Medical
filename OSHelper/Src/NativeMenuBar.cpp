#include "StdAfx.h"

extern "C" _AnomalousExport wxMenu* NativeMenuBar_append(wxMenuBar* menuBar, String text)
{
	wxMenu* menu = new wxMenu();
	menuBar->Append(menu, text);
	return menu;
}