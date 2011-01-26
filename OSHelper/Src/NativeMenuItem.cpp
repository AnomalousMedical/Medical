#include "Stdafx.h"
#include "NativeString.h"

extern "C" _AnomalousExport void NativeMenuItem_delete(wxMenuItem* item)
{
	delete item;
}

extern "C" _AnomalousExport void NativeMenuItem_setEnabled(wxMenuItem* item, bool value)
{
	item->Enable(value);
}

extern "C" _AnomalousExport bool NativeMenuItem_getEnabled(wxMenuItem* item)
{
	return item->IsEnabled();
}

extern "C" _AnomalousExport int NativeMenuItem_getID(wxMenuItem* item)
{
	return item->GetId();
}

extern "C" _AnomalousExport wxMenu* NativeMenuItem_getSubMenu(wxMenuItem* item)
{
	return item->GetSubMenu();
}

extern "C" _AnomalousExport void NativeMenuItem_setHelp(wxMenuItem* item, String helpText)
{
	item->SetHelp(wxString::FromAscii(helpText));
}

extern "C" _AnomalousExport NativeString* NativeMenuItem_getHelp(wxMenuItem* item)
{
	return new NativeString(item->GetHelp());
}