// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#ifndef STDAFX_H
#define STDAFX_H

#ifdef WINDOWS
#define _AnomalousExport __declspec(dllexport)
#endif

#ifdef MAC_OSX
#define _AnomalousExport __attribute__ ((visibility("default")))
#endif

//#define ENABLE_HASP_PROTECTION

typedef unsigned int uint;
typedef unsigned char byte;
typedef unsigned short ushort;
typedef const char* String;

#include <wx/wx.h>

class Color
{
public:
	float r, g, b, a;

	Color(const wxColour wxColor)
		:r(wxColor.Red() / 255.0f),
		g(wxColor.Green() / 255.0f),
		b(wxColor.Blue() / 255.0f),
		a(wxColor.Alpha() / 255.0f)
	{

	}

	wxColour toWx() const
	{
		return wxColour(r * 255, g * 255, b * 255, a * 255);
	}
};

#endif