#include "StdAfx.h"
#include "NativeString.h"

#ifdef USE_WXWIDGETS

extern "C" _AnomalousExport void NativeString_delete(NativeString* nativeString)
{
	delete nativeString;
}

extern "C" _AnomalousExport String NativeString_c_str(NativeString* nativeString)
{
	return nativeString->c_str();
}

#endif