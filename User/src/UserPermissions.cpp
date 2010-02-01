#include "stdafx.h"
#include "UserPermissions.h"
#include "Settings.h"

namespace Medical
{

unsigned char vendor_code[] =
	"AzIceaqfA1hX5wS+M8cGnYh5ceevUnOZIzJBbXFD6dgf3tBkb9cvUF/Tkd/iKu2fsg9wAysYKw7RMAsV"
	"vIp4KcXle/v1RaXrLVnNBJ2H2DmrbUMOZbQUFXe698qmJsqNpLXRA367xpZ54i8kC5DTXwDhfxWTOZrB"
	"rh5sRKHcoVLumztIQjgWh37AzmSd1bLOfUGI0xjAL9zJWO3fRaeB0NS2KlmoKaVT5Y04zZEc06waU2r6"
	"AU2Dc4uipJqJmObqKM+tfNKAS0rZr5IudRiC7pUwnmtaHRe5fgSI8M7yvypvm+13Wm4Gwd4VnYiZvSxf"
	"8ImN3ZOG9wEzfyMIlH2+rKPUVHI+igsqla0Wd9m7ZUR9vFotj1uYV0OzG7hX0+huN2E/IdgLDjbiapj1"
	"e2fKHrMmGFaIvI6xzzJIQJF9GiRZ7+0jNFLKSyzX/K3JAyFrIPObfwM+y+zAgE1sWcZ1YnuBhICyRHBh"
	"aJDKIZL8MywrEfB2yF+R3k9wFG1oN48gSLyfrfEKuB/qgNp+BeTruWUk0AwRE9XVMUuRbjpxa4YA67SK"
	"unFEgFGgUfHBeHJTivvUl0u4Dki1UKAT973P+nXy2O0u239If/kRpNUVhMg8kpk7s8i6Arp7l/705/bL"
	"Cx4kN5hHHSXIqkiG9tHdeNV8VYo5+72hgaCx3/uVoVLmtvxbOIvo120uTJbuLVTvT8KtsOlb3DxwUrwL"
	"zaEMoAQAFk6Q9bNipHxfkRQER4kR7IYTMzSoW5mxh3H9O8Ge5BqVeYMEW36q9wnOYfxOLNw6yQMf8f9s"
	"JN4KhZty02xm707S7VEfJJ1KNq7b5pP/3RjE0IKtB2gE6vAPRvRLzEohu0m7q1aUp8wAvSiqjZy7FLaT"
	"tLEApXYvLvz6PEJdj4TegCZugj7c8bIOEqLXmloZ6EgVnjQ7/ttys7VFITB3mazzFiyQuKf4J6+b/a/Y";

UserPermissions::UserPermissions()
:handle(HASP_INVALID_HANDLE_VALUE)
{
	if(instance != nullptr)
	{
		throw gcnew System::Exception("Only one UserPermissions instance can be created at a time.");
	}
	instance = this;
#ifndef ENABLE_HASP_PROTECTION
	#ifdef SIM_LITE
	//Simulated lite version
	allowedFeatures.AddLast(Features::PIPER_JBO_LITE);
	#endif

	#ifdef SIM_STANDARD
	//Simulated standard version
	allowedFeatures.AddLast(Features::PIPER_JBO_STANDARD);
	#endif
	
	#ifdef SIM_GRAPHICS
	//Simulated graphics version
	allowedFeatures.AddLast(Features::PIPER_JBO_GRAPHICS);
	#endif

	#ifdef SIM_DOPPLER
	//Simulated doppler version
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_DOPPLER);
	#endif

	#ifdef SIM_DENTITION_AND_PROFILE
	//Simulated Dentition and Profile version
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_TEETH);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_PROFILE);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_PROFILE_TEETH);
	#endif

	#ifdef SIM_CLINICAL
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_DOPPLER);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_TEETH);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_PROFILE);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_PROFILE_TEETH);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_CLINICAL);
	#endif

	#ifdef SIM_RADIOGRAPHY
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_DOPPLER);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_TEETH);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_PROFILE);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_PROFILE_TEETH);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_CLINICAL);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_BONE);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_CT);
	#endif

	#ifdef SIM_MRI
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_DOPPLER);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_TEETH);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_PROFILE);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_PROFILE_TEETH);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_CLINICAL);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_BONE);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_CT);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_DISC);
	allowedFeatures.AddLast(Features::WIZARD_PIPER_JBO_MRI);
	#endif

#endif
}

UserPermissions::~UserPermissions()
{
	if(handle != HASP_INVALID_HANDLE_VALUE)
	{
		logout();
	}
	instance = nullptr;
}

bool UserPermissions::allowFeature(Features featureId)
{
#ifdef ENABLE_HASP_PROTECTION
	const hasp_feature_t feature = static_cast<hasp_feature_t>(featureId);

	hasp_handle_t handle = HASP_INVALID_HANDLE_VALUE;
	hasp_status_t status;

	status = hasp_login(feature, vendor_code, &handle);

	/* check if operation was successful */
	if (status == HASP_STATUS_OK)
	{
		hasp_logout(handle);
		return true;
	}
	else
	{
		return false;
	}
#else
	return allowedFeatures.Contains(featureId);
#endif
}

bool UserPermissions::checkConnection()
{
#ifdef ENABLE_HASP_PROTECTION
	hasp_status_t status = HASP_STATUS_OK;
	if(handle == HASP_INVALID_HANDLE_VALUE)
	{
		hasp_handle_t localHandle;
		status = hasp_login(0, vendor_code, &localHandle);
		handle = localHandle;
	}
	if(status == HASP_STATUS_OK)
	{
		unsigned char data[1];
		status = hasp_read(handle, HASP_FILEID_RO, 0, 1, data);
		if(status == HASP_STATUS_OK)
		{
			return true;
		}
		else
		{
			logout();
		}
	}
	return false;
#else
	return true;
#endif
}

void UserPermissions::logout()
{
#ifdef ENABLE_HASP_PROTECTION
	hasp_logout(handle);
	handle = HASP_INVALID_HANDLE_VALUE;
#endif
}

}