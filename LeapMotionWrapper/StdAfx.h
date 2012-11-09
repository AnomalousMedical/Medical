#ifndef STDAFX_H
#define STDAFX_H

#ifdef WINDOWS
#define _AnomalousExport __declspec(dllexport)
typedef const wchar_t* String; //UTF16 Needed, On windows this is ok
#endif

#ifdef MAC_OSX
#define _AnomalousExport __attribute__ ((visibility("default")))
typedef const unsigned short* String; //UTF16 Needed
#endif

#define _HAS_ITERATOR_DEBUGGING 0
#include "Leap.h"

typedef int64_t Int64;

#endif