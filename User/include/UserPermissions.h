#include "hasp_api.h"
#include "Features.h"

namespace Medical
{

public enum class ConnectionResult
{
	Ok = 1,
	NoDongle = 2,
	TooManyUsers = 3,
};

#ifndef ENABLE_HASP_PROTECTION
public enum class SimulatedVersion
{
	Doppler,
	DentitionAndProfile,
	Clinical,
	Radiography,
	MRI,
	Graphics
};
#endif

public ref class UserPermissions
{
private:
	hasp_handle_t handle;
	static UserPermissions^ instance;
#ifndef ENABLE_HASP_PROTECTION
	System::Collections::Generic::LinkedList<Features> allowedFeatures;
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

	System::String^ getId();

	static property UserPermissions^ Instance
	{
		UserPermissions^ get()
		{
			return instance;
		}
	}
};

}