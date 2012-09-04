// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#ifndef STDAFX_H
#define STDAFX_H

#ifdef WINDOWS
#ifndef USE_WXWIDGETS
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#endif

#define _AnomalousExport __declspec(dllexport)
#endif

#ifdef MAC_OSX
#define _AnomalousExport __attribute__ ((visibility("default")))
#endif

typedef unsigned int uint;
typedef unsigned char byte;
typedef unsigned short ushort;
typedef const char* String;

#ifdef USE_WXWIDGETS

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

#endif