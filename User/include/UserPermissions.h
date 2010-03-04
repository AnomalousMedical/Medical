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
	UserPermissions();

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