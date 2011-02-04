#include "stdafx.h"

enum OperatingSystem
{
    Windows,
    Mac,
};

extern "C" _AnomalousExport OperatingSystem PlatformConfig_getPlatform()
{
#if WINDOWS
	return Windows;
#elif MAC_OSX
	return Mac;
#endif
}