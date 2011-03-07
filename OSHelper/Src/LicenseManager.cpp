#include "stdafx.h"

typedef void(*MachineIDCallback)(String value);

#if WINDOWS

#include <sstream>
extern "C" _AnomalousExport void LicenseManager_getMachineID(MachineIDCallback callback)
{
	DWORD volSerial;
	GetVolumeInformation(LPCWSTR("C:\\"),NULL,NULL,&volSerial,NULL,NULL,NULL,NULL);
	std::stringstream ss;
	ss << "WIN" << volSerial;
	callback(ss.str().c_str());
}

#elif MAC_OSX

extern "C" _AnomalousExport void LicenseManager_getMachineID(MachineIDCallback callback)
{
	return "OfflineTest1";
}

#endif