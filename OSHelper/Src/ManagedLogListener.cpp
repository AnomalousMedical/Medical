#include "StdAfx.h"
#include "NativeLog.h"

typedef void (*LogMessageDelegate)(const char* message, LogLevel logLevel, const char* subsystem);

class ManagedLogListener : NativeLogListener
{
private:
	LogMessageDelegate logDelegate;

public:
	ManagedLogListener(LogMessageDelegate logDelegate)
		:logDelegate(logDelegate)
	{

	}

	virtual ~ManagedLogListener(void)
	{

	}

	virtual void logMessage(const std::string& message, LogLevel logLevel, const std::string& subsystem)
	{
		logDelegate(message.c_str(), logLevel, subsystem.c_str());
	}
};

extern "C" _AnomalousExport ManagedLogListener* ManagedLogListener_create(LogMessageDelegate logDelegate)
{
	return new ManagedLogListener(logDelegate);
}

extern "C" _AnomalousExport void ManagedLogListener_destroy(ManagedLogListener* managedLogListener)
{
	delete managedLogListener;
}