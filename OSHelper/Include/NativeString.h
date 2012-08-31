#pragma once

#ifdef USE_WXWIDGETS

#include <string>

class NativeString
{
public:
	NativeString(const wxString& string)
		:string(string)
	{

	}

	virtual ~NativeString(void)
	{

	}

	String c_str()
	{
		return string.c_str();
	}

private:
	std::string string;
};

#endif