#pragma once

#include "MouseButtonCode.h"

class NativeOSWindow;

class NativeMouse
{
public:
	typedef void (*MouseButtonDownDelegate)(MouseButtonCode id);
	typedef void (*MouseButtonUpDelegate)(MouseButtonCode id);
	typedef void (*MouseMoveDelegate)(int absX, int absY);
	typedef void (*MouseWheelDelegate)(int relZ);
    
	NativeMouse(NativeOSWindow* osWindow, MouseButtonDownDelegate mouseButtonDownCB, MouseButtonUpDelegate mouseButtonUpCB, MouseMoveDelegate mouseMoveCB, MouseWheelDelegate mouseWheelCB);
    
	virtual ~NativeMouse();
    
private:
	MouseButtonDownDelegate mouseButtonDownCB;
	MouseButtonUpDelegate mouseButtonUpCB;
	MouseMoveDelegate mouseMoveCB;
	MouseWheelDelegate mouseWheelCB;
	NativeOSWindow* osWindow;
};
