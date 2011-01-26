#include "StdAfx.h"
#include "..\Include\NativeString.h"

extern "C" _AnomalousExport void NativeString_delete(NativeString* nativeString)
{
	delete nativeString;
}

extern "C" _AnomalousExport String NativeString_c_str(NativeString* nativeString)
{
	return nativeString->c_str();
}
