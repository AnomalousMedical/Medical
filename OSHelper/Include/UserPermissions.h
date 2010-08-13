#pragma once

#include "Features.h"
#include <vector>
#include <string>

enum ConnectionResult
{
	Ok = 1,
	NoDongle = 2,
	TooManyUsers = 3,
};

#ifndef ENABLE_HASP_PROTECTION
enum SimulatedVersion
{
	Doppler,
	DentitionAndProfile,
	Clinical,
	Radiography,
	MRI,
	Graphics
};

#else
#include "hasp_api.h"
#endif

class UserPermissions
{
private:
	std::string id;

#ifndef ENABLE_HASP_PROTECTION
	std::vector<Features> allowedFeatures;
#else
	hasp_handle_t handle;
#endif

	void logout();

public:
#ifndef ENABLE_HASP_PROTECTION
	UserPermissions(SimulatedVersion simVersion);
#else 
	UserPermissions();
#endif

	~UserPermissions();

	bool allowFeature(Features featureId);

	ConnectionResult checkConnection();

	const std::string& getId();
};