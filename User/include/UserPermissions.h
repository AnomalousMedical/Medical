#include "hasp_api.h"

namespace Medical
{

public ref class UserPermissions
{
private:
	hasp_handle_t handle;
	static UserPermissions^ instance;

	void logout();

public:
	UserPermissions();

	~UserPermissions();

	bool allowFeature(int featureId);

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