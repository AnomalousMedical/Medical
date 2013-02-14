// stdafx.cpp : source file that includes just the standard includes
// OSHelper.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"

NativeLog logger("OSHelperNative");

extern "C" _AnomalousExport void NativeLog_addLogListener(NativeLogListener* logListener)
{
	logger.addListener(logListener);
}