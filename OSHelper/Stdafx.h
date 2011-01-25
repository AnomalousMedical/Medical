// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

#ifdef WINDOWS
#define _AnomalousExport __declspec(dllexport)
#endif

#ifdef MAC_OSX
#define _AnomalousExport __attribute__ ((visibility("default")))
#endif

//#define ENABLE_HASP_PROTECTION

typedef unsigned int uint;
typedef unsigned char byte;
typedef unsigned short ushort;
typedef const char* String;

#include <wx/wx.h>