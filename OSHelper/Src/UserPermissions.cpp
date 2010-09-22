#include "stdafx.h"
#include "UserPermissions.h"
#include <algorithm>

unsigned char vendor_code[] = "0DICIOvtfneX6gWjsqVPzFiBMCSSksScJlapNYOpqMyh0iIA6WSSTaEEJLKvgwE6YLFZhErhDMqwEAGjD0uFUPT5xCzYxr3Vk5kI2/5bMEWGNvAI924nWJQJQYY39WSOKoMElRb6AEA4O99XqJ1VGjP1/rInAVbJT4RF3X1a1OlR4q8lxYTuKSt+X2toktqI54CDVap7u3eiWld3uUmwvZ7MAaN3Ys7ZvmePzLd6BiXzM/f0JFOJO3TNDHsM48OXC0cJVfVLgj5RqMEJJr0rtZlxdeuy6cpm9UIqOS8y6nPvzNXlKlexHzro4gOpt5klMzLLmVm5SybvaGkyEm6LDVpB1LO7vz8bT8g0yrZ2xvr2II2fkWyZ9hklgRfTZVbCivSTBpHDrOvqEgJWGjo9rS0w14A+H/1QcYgsxB2uKG1aRSHFUY+j8UNGkzM3Ehl301m8wksU3Cm06yihelCHuNMFVZuTia3SGUAOEmfFzDVpiO6P+pNJnwUX2xpbTElpUWrce+qFjVNZ4mppiFHAmedAkeyK9g9EarOErp1lP+kfYJ6h3Qd4iTRKLsQHX57LgNFrXcT7rSMEjF3ur88Zgc+k8DC1vLuQ6tHhPjYCBmp+Z5ik10maaKOKgBY602LlMMMWnnvCe65WInuPFNd9I0NBglP/0tCOLjM38kgDBmdkzJkSBHJMpAk8i40cHazmq1rrCxiP6F6ehe2HNfLbePJpWGz1dwbw7nV383WFmkzdIqJ/jZjZfJC3HvbsQP1prq3LrkC/PfiDfKgT5ArMS8nJgJ3FcQX3S1B/2qvBKxJt2HIB9V/4wtxTr5eR8kr8tSwbxzKJZM+kakHA1nRPMUaZukLlCWJyOF2lZqS0d0Iw5t6uUbUTtQ1ik6Iy5ej6PioAU4kRT4abGws40+rdJ9tsTcPaEp8FYrWEiGq4rI0=";

#ifndef ENABLE_HASP_PROTECTION
UserPermissions::UserPermissions(SimulatedVersion simVersion)
:id("0000000000000")
{
	switch(simVersion)
	{
		case Doppler:
			//Simulated doppler version
			allowedFeatures.push_back(PIPER_JBO_MODULE);
			allowedFeatures.push_back(PIPER_JBO_VERSION_DOPPLER);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DOPPLER);
			break;

		case DentitionAndProfile:
			//Simulated Dentition and Profile version
			allowedFeatures.push_back(PIPER_JBO_MODULE);
			allowedFeatures.push_back(PIPER_JBO_VERSION_DENTITION_PROFILE);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DENTITION);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CEPHALOMETRIC);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION);
			break;

		case Clinical:
			allowedFeatures.push_back(PIPER_JBO_MODULE);
			allowedFeatures.push_back(PIPER_JBO_VERSION_CLINICAL);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DOPPLER);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DENTITION);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CEPHALOMETRIC);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CLINICAL_DOPPLER);
			allowedFeatures.push_back(PIPER_JBO_FEATURE_CUSTOM_LAYERS);
			allowedFeatures.push_back(PIPER_JBO_FEATURE_SIMULATION);
			break;

		case Radiography:
			allowedFeatures.push_back(PIPER_JBO_MODULE);
			allowedFeatures.push_back(PIPER_JBO_VERSION_RADIOGRAPHY_CT);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DOPPLER);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DENTITION);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CEPHALOMETRIC);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CLINICAL_DOPPLER);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_MANDIBLE);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DISC_SPACE);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_RADIOGRAPHY);
			allowedFeatures.push_back(PIPER_JBO_FEATURE_CUSTOM_LAYERS);
			allowedFeatures.push_back(PIPER_JBO_FEATURE_SIMULATION);
			break;

		case MRI:
			allowedFeatures.push_back(PIPER_JBO_MODULE);
			allowedFeatures.push_back(PIPER_JBO_VERSION_MRI);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DOPPLER);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DENTITION);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CEPHALOMETRIC);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CLINICAL_DOPPLER);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_MANDIBLE);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_RADIOGRAPHY);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DISC_SPACE);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DISC_CLOCK);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_MRI);
			allowedFeatures.push_back(PIPER_JBO_FEATURE_CUSTOM_LAYERS);
			allowedFeatures.push_back(PIPER_JBO_FEATURE_SIMULATION);
			break;

		case Graphics:
			allowedFeatures.push_back(PIPER_JBO_MODULE);
			allowedFeatures.push_back(PIPER_JBO_VERSION_GRAPHICS);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DOPPLER);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DENTITION);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CEPHALOMETRIC);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_CLINICAL_DOPPLER);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_MANDIBLE);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_RADIOGRAPHY);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DISC_SPACE);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_DISC_CLOCK);
			allowedFeatures.push_back(PIPER_JBO_WIZARD_MRI);
			allowedFeatures.push_back(PIPER_JBO_FEATURE_FULL_RENDERING);
			allowedFeatures.push_back(PIPER_JBO_FEATURE_CLONE_WINDOW);
			allowedFeatures.push_back(PIPER_JBO_FEATURE_CUSTOM_LAYERS);
			allowedFeatures.push_back(PIPER_JBO_FEATURE_SIMULATION);
			break;
	}
}

#else
UserPermissions::UserPermissions()
:handle(HASP_INVALID_HANDLE_VALUE)
{
	
}
#endif

UserPermissions::~UserPermissions()
{
#ifdef ENABLE_HASP_PROTECTION
	if(handle != HASP_INVALID_HANDLE_VALUE)
	{
		logout();
	}
#endif
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
	return std::find(allowedFeatures.begin(), allowedFeatures.end(), featureId) != allowedFeatures.end();
#endif
}

ConnectionResult UserPermissions::checkConnection()
{
#ifdef ENABLE_HASP_PROTECTION
	hasp_status_t status = HASP_STATUS_OK;
	if(handle == HASP_INVALID_HANDLE_VALUE)
	{
		hasp_handle_t localHandle;
		status = hasp_login(static_cast<hasp_feature_t>(PIPER_JBO_MODULE), vendor_code, &localHandle);
		handle = localHandle;
	}
	if(status == HASP_STATUS_OK)
	{
		unsigned char data[1];
		status = hasp_read(handle, HASP_FILEID_RO, 0, 1, data);
		if(status == HASP_STATUS_OK)
		{
			return ConnectionResult::Ok;
		}
		else
		{
			logout();
		}
	}
	else if(status == HASP_TOO_MANY_USERS)
	{
		return TooManyUsers;
	}
	return NoDongle;
#else
	return Ok;
#endif
}

const std::string& UserPermissions::getId()
{
#ifdef ENABLE_HASP_PROTECTION
	id = "Error";
	if(checkConnection() == ConnectionResult::Ok)
	{
		char *info = 0;

		const char* format = 
		"<?xml version=\"1.0\" encoding=\"UTF-8\" ?>"
		"<haspformat root=\"haspscope\">"
		"    <hasp>"
		"        <attribute name=\"id\" />"
		"    </hasp>"
		"</haspformat>";

		hasp_status_t status = hasp_get_sessioninfo(handle, format, &info);

		/* check if operation was successful */
		if (status == HASP_STATUS_OK)
		{
			/*System::String^ managedInfo = gcnew System::String(info);
			int idIndex = managedInfo->IndexOf("id=\"") + 4;
			int endQuoteIndex = managedInfo->IndexOf("\"", idIndex);
			returnVal = managedInfo->Substring(idIndex, endQuoteIndex - idIndex);*/
			id = info;
		}

		hasp_free(info);
	}
	return id;
#else
	return id;
#endif
}

void UserPermissions::logout()
{
#ifdef ENABLE_HASP_PROTECTION
	hasp_logout(handle);
	handle = HASP_INVALID_HANDLE_VALUE;
#endif
}

#ifndef ENABLE_HASP_PROTECTION
extern "C" _AnomalousExport UserPermissions* UserPermissions_create(SimulatedVersion simVersion)
{
	return new UserPermissions(simVersion);
}
#else 
extern "C" _AnomalousExport UserPermissions* UserPermissions_create()
{
	return new UserPermissions();
}
#endif

extern "C" _AnomalousExport void UserPermissions_destroy(UserPermissions* permissions)
{
	delete permissions;
}

extern "C" _AnomalousExport bool UserPermissions_allowFeature(UserPermissions* permissions, Features featureId)
{
	return permissions->allowFeature(featureId);
}

extern "C" _AnomalousExport ConnectionResult UserPermissions_checkConnection(UserPermissions* permissions)
{
	return permissions->checkConnection();
}

extern "C" _AnomalousExport String UserPermissions_getId(UserPermissions* permissions)
{
	return permissions->getId().c_str();
}