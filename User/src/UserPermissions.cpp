#include "stdafx.h"
#include "UserPermissions.h"
#include "Settings.h"

namespace Medical
{

unsigned char vendor_code[] = "0DICIOvtfneX6gWjsqVPzFiBMCSSksScJlapNYOpqMyh0iIA6WSSTaEEJLKvgwE6YLFZhErhDMqwEAGjD0uFUPT5xCzYxr3Vk5kI2/5bMEWGNvAI924nWJQJQYY39WSOKoMElRb6AEA4O99XqJ1VGjP1/rInAVbJT4RF3X1a1OlR4q8lxYTuKSt+X2toktqI54CDVap7u3eiWld3uUmwvZ7MAaN3Ys7ZvmePzLd6BiXzM/f0JFOJO3TNDHsM48OXC0cJVfVLgj5RqMEJJr0rtZlxdeuy6cpm9UIqOS8y6nPvzNXlKlexHzro4gOpt5klMzLLmVm5SybvaGkyEm6LDVpB1LO7vz8bT8g0yrZ2xvr2II2fkWyZ9hklgRfTZVbCivSTBpHDrOvqEgJWGjo9rS0w14A+H/1QcYgsxB2uKG1aRSHFUY+j8UNGkzM3Ehl301m8wksU3Cm06yihelCHuNMFVZuTia3SGUAOEmfFzDVpiO6P+pNJnwUX2xpbTElpUWrce+qFjVNZ4mppiFHAmedAkeyK9g9EarOErp1lP+kfYJ6h3Qd4iTRKLsQHX57LgNFrXcT7rSMEjF3ur88Zgc+k8DC1vLuQ6tHhPjYCBmp+Z5ik10maaKOKgBY602LlMMMWnnvCe65WInuPFNd9I0NBglP/0tCOLjM38kgDBmdkzJkSBHJMpAk8i40cHazmq1rrCxiP6F6ehe2HNfLbePJpWGz1dwbw7nV383WFmkzdIqJ/jZjZfJC3HvbsQP1prq3LrkC/PfiDfKgT5ArMS8nJgJ3FcQX3S1B/2qvBKxJt2HIB9V/4wtxTr5eR8kr8tSwbxzKJZM+kakHA1nRPMUaZukLlCWJyOF2lZqS0d0Iw5t6uUbUTtQ1ik6Iy5ej6PioAU4kRT4abGws40+rdJ9tsTcPaEp8FYrWEiGq4rI0=";

UserPermissions::UserPermissions()
:handle(HASP_INVALID_HANDLE_VALUE)
{
	if(instance != nullptr)
	{
		throw gcnew System::Exception("Only one UserPermissions instance can be created at a time.");
	}
	instance = this;
#ifndef ENABLE_HASP_PROTECTION
	#ifdef SIM_DOPPLER
	//Simulated doppler version
	allowedFeatures.AddLast(Features::PIPER_JBO_MODULE);
	allowedFeatures.AddLast(Features::PIPER_JBO_VERSION_DOPPLER);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DOPPLER);
	#endif

	#ifdef SIM_DENTITION_AND_PROFILE
	//Simulated Dentition and Profile version
	allowedFeatures.AddLast(Features::PIPER_JBO_MODULE);
	allowedFeatures.AddLast(Features::PIPER_JBO_VERSION_DENTITION_PROFILE);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DENTITION);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CEPHALOMETRIC);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION);
	#endif

	#ifdef SIM_CLINICAL
	allowedFeatures.AddLast(Features::PIPER_JBO_MODULE);
	allowedFeatures.AddLast(Features::PIPER_JBO_VERSION_CLINICAL);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DOPPLER);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DENTITION);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CEPHALOMETRIC);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CLINICAL_DOPPLER);
	allowedFeatures.AddLast(Features::PIPER_JBO_FEATURE_CUSTOM_LAYERS);
	allowedFeatures.AddLast(Features::PIPER_JBO_FEATURE_SIMULATION);
	#endif

	#ifdef SIM_RADIOGRAPHY
	allowedFeatures.AddLast(Features::PIPER_JBO_MODULE);
	allowedFeatures.AddLast(Features::PIPER_JBO_VERSION_RADIOGRAPHY_CT);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DOPPLER);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DENTITION);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CEPHALOMETRIC);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CLINICAL_DOPPLER);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_MANDIBLE);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DISC_SPACE);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_RADIOGRAPHY);
	allowedFeatures.AddLast(Features::PIPER_JBO_FEATURE_CUSTOM_LAYERS);
	allowedFeatures.AddLast(Features::PIPER_JBO_FEATURE_SIMULATION);
	#endif

	#ifdef SIM_MRI
	allowedFeatures.AddLast(Features::PIPER_JBO_MODULE);
	allowedFeatures.AddLast(Features::PIPER_JBO_VERSION_MRI);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DOPPLER);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DENTITION);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CEPHALOMETRIC);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CLINICAL_DOPPLER);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_MANDIBLE);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_RADIOGRAPHY);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DISC_SPACE);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DISC_CLOCK);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_MRI);
	allowedFeatures.AddLast(Features::PIPER_JBO_FEATURE_CUSTOM_LAYERS);
	allowedFeatures.AddLast(Features::PIPER_JBO_FEATURE_SIMULATION);
	#endif

	#ifdef SIM_GRAPHICS
	allowedFeatures.AddLast(Features::PIPER_JBO_MODULE);
	allowedFeatures.AddLast(Features::PIPER_JBO_VERSION_GRAPHICS);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DOPPLER);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DENTITION);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CEPHALOMETRIC);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_CLINICAL_DOPPLER);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_MANDIBLE);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_RADIOGRAPHY);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DISC_SPACE);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_DISC_CLOCK);
	allowedFeatures.AddLast(Features::PIPER_JBO_WIZARD_MRI);
	allowedFeatures.AddLast(Features::PIPER_JBO_FEATURE_FULL_RENDERING);
	allowedFeatures.AddLast(Features::PIPER_JBO_FEATURE_CLONE_WINDOW);
	allowedFeatures.AddLast(Features::PIPER_JBO_FEATURE_CUSTOM_LAYERS);
	allowedFeatures.AddLast(Features::PIPER_JBO_FEATURE_SIMULATION);
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

System::String^ UserPermissions::getId()
{
#ifdef ENABLE_HASP_PROTECTION
	System::String^ returnVal = "Error";
	if(checkConnection())
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
			System::String^ managedInfo = gcnew System::String(info);
			int idIndex = managedInfo->IndexOf("id=\"") + 4;
			int endQuoteIndex = managedInfo->IndexOf("\"", idIndex);
			returnVal = managedInfo->Substring(idIndex, endQuoteIndex - idIndex);
		}

		hasp_free(info);
	}
	return returnVal;
#else
	return "0000000000000";
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