#include "StdAfx.h"
#include "NativeLog.h"

const std::string NativeLog::BLANK;

NativeLog::NativeLog(std::string& subsystem)
:subsystem(subsystem)
{
}

NativeLog::NativeLog(const char* subsystem)
:subsystem(subsystem)
{

}

NativeLog::~NativeLog(void)
{
}

void NativeLog::sendMessage(const std::string& message, LogLevel logLevel)
{
	std::vector<NativeLogListener*>::iterator end = listeners.end();
	for(std::vector<NativeLogListener*>::iterator iter = listeners.begin(); iter != end; ++iter)
	{
		(*iter)->logMessage(message, logLevel, subsystem);
	}
}

void NativeLog::addListener(NativeLogListener* listener)
{
	listeners.push_back(listener);
}