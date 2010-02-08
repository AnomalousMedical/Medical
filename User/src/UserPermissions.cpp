#include "stdafx.h"
#include "UserPermissions.h"
#include "Settings.h"

namespace Medical
{

unsigned char vendor_code[] = "HzHNnjCSAMvslqiZJmPbxgmsh2X1uyq8f+LFQ34pxMlTpE95mLL+10vdGX9OgpPcJ0TgkH+A3N18f1imAtxHDTfiRwr46WJOthewOiaoJxF2b7B7G5cEzSBrUtIHsXzEnhRFz2rcndCT8HnMgN5kp43+hDn8vw9qE0uQ8F7MfVd6cmAcKDL8xSGpc0gF2LIJDzphhNEGsFRDq2lP7zSfYtQ0WH3S7DTOE7RhGKn2ap6ciUvxctFyE5LrOmNJ4ji5r0s/zuBTY4egxNqfD9AzagKHs/1Bz4yfGy94GOR7T8fOIW5V0DAd/gevgLMZ1VgRQTv4mL8EnX4Avf3rHWnKqmHZS91bdR2kLMQALN9PoTzmHGQAGsQLBDlSLVZCPZpeV/VRDnTLacAJLLzgqs2G4Ko3SMYJlTDM/p6PoEJlgjRhu8S/vGu1OqQ8ZTAqBL+ABrM1irJ96FT4bsnmTCMyI3JXQQ5VcaY6WewWI7dlftryg/1+1HwZ+72pnzt8iwrJ1nlt3rJqTYpAJrW1JNSITFUNBB84s1TLakxlrXlic2Zc5bPqU71R4IncLTsprHLOyvxqbk6MYaoL03HagJRbsnYz37+90W1NfGjB7412m01wUSpAAboeCvko3KegF1sjKiAFA6w24xf0ik0v8ccxfTJCTGnggZFM4d5GAxviktyoX0pHesigQbrrZesaZXvN6bQPmtOLJKfm094u4PIBhdUG55uuz2n4tCVhRLbEX6z45rYajxxDpEIoz0LQJSZ0CgJi0GWgtQjF1ZSdMOnsGUCpxNmimKB4Z31Ods6ql6oNQgc+Re6SYL94RrB41bC8hnExIdkx5eqNMHihaNhNN3s3IXJV0jPBxR/8HlEeVXrC4k9OH2IOWjjUy56QypYn4bQkt1Fnbr3STibSWTVUJeWoEZlYcMZfVBnfKAFVSzU=";

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