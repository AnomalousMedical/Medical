#include "hasp_api.h"
#include "Features.h"

namespace Medical
{

public ref class UserPermissions
{
private:
	hasp_handle_t handle;
	static UserPermissions^ instance;
#ifndef ENABLE_HASP_PROTECTION
	System::Collections::Generic::LinkedList<Features> blockedFeatures;
#endif

	void logout();

public:
	UserPermissions();

	~UserPermissions();

	bool allowFeature(Features featureId);

	bool checkConnection();

	static property UserPermissions^ Instance
	{
		UserPermissions^ get()
		{
			return instance;
		}
	}
};

}