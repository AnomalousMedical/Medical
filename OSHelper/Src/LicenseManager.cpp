#include "stdafx.h"

typedef void(*MachineIDCallback)(String value);

#if WINDOWS

#include <sstream>
extern "C" _AnomalousExport void LicenseManager_getMachineID(MachineIDCallback callback)
{
	HKEY hKey = 0;
	DWORD dwDisposition;
	WCHAR buf[255] = {0};
	DWORD dwBufSize = sizeof(buf);
	DWORD type = REG_SZ;

	if(RegCreateKeyEx(HKEY_LOCAL_MACHINE, TEXT("Software\\AnomalousMedical\\General"), 0, NULL, 0, KEY_ALL_ACCESS, NULL, &hKey, &dwDisposition) == ERROR_SUCCESS)
	{
		LONG result = RegQueryValueEx(hKey, L"MachineGuid", NULL, &type, (LPBYTE)buf, &dwBufSize);
		if(result == ERROR_FILE_NOT_FOUND)
		{
			UUID uuid;
			RPC_STATUS rpcResult = UuidCreate(&uuid);
			if(rpcResult == RPC_S_OK)
			{
				RPC_WSTR uuidStr;
				UuidToString(&uuid, &uuidStr);
				int size = 0;
				for(size = 0; uuidStr[size] != 0 && size < 255; ++size)
				{
					buf[size] = uuidStr[size];
				}
				size = (size + 1) * sizeof(LPBYTE);
				RegSetValueEx(hKey, L"MachineGuid", 0, REG_SZ, (LPBYTE)uuidStr, size);
				RpcStringFree(&uuidStr);
			}
		}
	}

	char cbBuff[258] = {'W', 'I', 'N', 0};
	for(int i = 0; buf[i] != 0; ++i)
	{
		cbBuff[i+3] = (char)buf[i];
	}
	callback((String)&cbBuff);
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