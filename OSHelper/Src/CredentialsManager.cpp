#include "stdafx.h"
#include <string>
#include <stdio.h>

class CredentialsManager
{
private:
	std::string filename;

public:
	CredentialsManager(const std::string& filename);

	~CredentialsManager();

	void secureStoreCredentials(const std::string& user, const std::string& pass);

	void deleteSecureCredentials();

	bool getHasStoredCredentials();

	std::string getUsername();

	std::string getPassword();
};


//Implementation
CredentialsManager::CredentialsManager(const std::string& filename)
	:filename(filename)
{

}

CredentialsManager::~CredentialsManager()
{

}

void CredentialsManager::secureStoreCredentials(const std::string& user, const std::string& pass)
{
	FILE* file;
	file = fopen(filename.c_str(), "w");
	if(file != NULL)
	{
		fclose(file);
	}
}

void CredentialsManager::deleteSecureCredentials()
{
	remove(filename.c_str());
}

bool CredentialsManager::getHasStoredCredentials()
{
	FILE* file;
	file = fopen(filename.c_str(), "r");
	if(file != NULL)
	{
		fclose(file);
		return true;
	}
	return false;
}

std::string CredentialsManager::getUsername()
{
	return "testguy";
}

std::string CredentialsManager::getPassword()
{
	return "testguy";
}

//CWrapper
typedef void (*GetStringDelegate)(String data);

CredentialsManager* CredentialsManager_create(String file)
{
	return new CredentialsManager(file);
}

void CredentialsManager_delete(CredentialsManager* credentialsManager)
{
	delete credentialsManager;
}

void CredentialsManager_secureStoreCredentials(CredentialsManager* credentialsManager, String user, String pass)
{
	credentialsManager->secureStoreCredentials(user, pass);
}

void CredentialsManager_deleteSecureCredentials(CredentialsManager* credentialsManager)
{
	credentialsManager->deleteSecureCredentials();
}

bool CredentialsManager_getHasStoredCredentials(CredentialsManager* credentialsManager)
{
	return credentialsManager->getHasStoredCredentials();
}

void CredentialsManager_getUsername(CredentialsManager* credentialsManager, GetStringDelegate stringCallback)
{
	stringCallback(credentialsManager->getUsername().c_str());
}

void CredentialsManager_getPassword(CredentialsManager* credentialsManager, GetStringDelegate stringCallback)
{
	stringCallback(credentialsManager->getPassword().c_str());
}