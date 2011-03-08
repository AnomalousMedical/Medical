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

#include <IOKit/IOKitLib.h>

void get_platform_uuid(char * buf, int bufSize) 
{
    io_registry_entry_t ioRegistryRoot = IORegistryEntryFromPath(kIOMasterPortDefault, "IOService:/");
    CFStringRef uuidCf = (CFStringRef) IORegistryEntryCreateCFProperty(ioRegistryRoot, CFSTR(kIOPlatformUUIDKey), kCFAllocatorDefault, 0);
    IOObjectRelease(ioRegistryRoot);
    CFStringGetCString(uuidCf, buf, bufSize, kCFStringEncodingMacRoman);
    CFRelease(uuidCf);    
}


extern "C" _AnomalousExport void LicenseManager_getMachineID(MachineIDCallback callback)
{
	char* platUUID = new char[256];
	get_platform_uuid(platUUID, 256);
	callback(platUUID);
}

#endif