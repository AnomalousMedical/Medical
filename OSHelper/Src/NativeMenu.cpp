#include "Stdafx.h"
#include "Enums.h"

#ifdef USE_WXWIDGETS

extern "C" _AnomalousExport wxMenuItem* NativeMenu_append(wxMenu* menu, CommonMenuItems id, String text, String helpText, bool subMenu)
{
	wxMenu* mySubMenu = NULL;
	if(subMenu)
	{
		mySubMenu = new wxMenu();
	}
	wxMenuItem* menuItem = new wxMenuItem(menu, convertMenuItemID(id), wxString::FromAscii(text), wxString::FromAscii(helpText), wxITEM_NORMAL, mySubMenu);
	menu->Append(menuItem);
	return menuItem;
}

extern "C" _AnomalousExport void NativeMenu_appendSeparator(wxMenu* menu)
{
	menu->Append(new wxMenuItem(NULL, wxID_SEPARATOR, wxEmptyString, wxEmptyString, wxITEM_SEPARATOR, NULL));
}

extern "C" _AnomalousExport wxMenuItem* NativeMenu_insert(wxMenu* menu, int index, CommonMenuItems id, String text, String helpText, bool subMenu)
{
	wxMenu* mySubMenu = NULL;
	if(subMenu)
	{
		mySubMenu = new wxMenu();
	}
	wxMenuItem* menuItem = new wxMenuItem(menu, convertMenuItemID(id), wxString::FromAscii(text), wxString::FromAscii(helpText), wxITEM_NORMAL, mySubMenu);
	menu->Insert(index, menuItem);
	return menuItem;
}

extern "C" _AnomalousExport void NativeMenu_insertItem(wxMenu* menu, int index, wxMenuItem* menuItem)
{
	menu->Insert(index, menuItem);
}

extern "C" _AnomalousExport void NativeMenu_remove(wxMenu* menu, wxMenuItem* menuItem)
{
	menu->Remove(menuItem);
}

#endif