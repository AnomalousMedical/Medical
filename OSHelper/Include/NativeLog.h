#pragma once

#include <string>
#include <vector>
#include <sstream>

enum LogLevel
{
	Info = 1 << 0,
	ImportantInfo = 1 << 1,
	Warning = 1 << 2,
	Error = 1 << 3,
	Debug = 1 << 4,
	All = Info | ImportantInfo | Warning | Error | Debug,
};

class NativeLogListener
{
public:
	virtual ~NativeLogListener(void){}

	virtual void logMessage(const std::string& message, LogLevel logLevel, const std::string& subsystem) = 0;
};

class NativeLog
{
private:
	static const std::string BLANK;

	std::string subsystem;
	std::vector<NativeLogListener*> listeners;
	std::stringstream stringStream;

public:
	NativeLog(std::string& subsystem);

	NativeLog(const char* subsystem);

	virtual ~NativeLog(void);

	void sendMessage(const std::string& message, LogLevel logLevel);

	void addListener(NativeLogListener* listener);

	template <typename T>
	NativeLog& operator << (const T& v)
	{
		stringStream << v;
		return *this;
	}

	class InfoFlush {};
	class ImportantInfoFlush {};
	class WarningFlush {};
	class ErrorFlush {};
	class DebugFlush {};

	NativeLog& operator << (const InfoFlush& v)
	{
        (void)v;
		sendMessage(stringStream.str(), Info);
		stringStream.str(BLANK);
		return *this;
	}

	NativeLog& operator << (const ImportantInfoFlush& v)
	{
        (void)v;
		sendMessage(stringStream.str(), ImportantInfo);
		stringStream.str(BLANK);
		return *this;
	}

	NativeLog& operator << (const WarningFlush& v)
	{
        (void)v;
		sendMessage(stringStream.str(), Warning);
		stringStream.str(BLANK);
		return *this;
	}

	NativeLog& operator << (const ErrorFlush& v)
	{
        (void)v;
		sendMessage(stringStream.str(), Error);
		stringStream.str(BLANK);
		return *this;
	}

	NativeLog& operator << (const DebugFlush& v)
	{
        (void)v;
		sendMessage(stringStream.str(), Debug);
		stringStream.str(BLANK);
		return *this;
	}
};

static NativeLog::InfoFlush info;
static NativeLog::ImportantInfoFlush importantInfo;
static NativeLog::WarningFlush warning;
static NativeLog::ErrorFlush error;
static NativeLog::DebugFlush debug;