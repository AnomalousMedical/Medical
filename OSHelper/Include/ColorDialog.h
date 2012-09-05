#pragma once

#pragma once

#include "Enums.h"
#include <string>

class NativeOSWindow;

class Color
{
public:
	float r, g, b, a;

	Color()
		:r(0.0f),
		g(0.0f),
		b(0.0f),
		a(0.0f)
	{

	}

	Color(byte r, byte g, byte b)
		:r(r / 255.0f),
		g(g / 255.0f),
		b(b / 255.0f),
		a(1.0f)
	{

	}
};

class ColorDialog
{
public:
	ColorDialog(NativeOSWindow* parent);

	virtual ~ColorDialog();

	NativeDialogResult showModal();

	void setColor(Color color);

	Color getColor();

private:
	NativeOSWindow* parent;
	Color color;
};