#include "StdAfx.h"
#include "NativeMouse.h"
#include "NativeOSWindow.h"

NativeMouse::NativeMouse(NativeOSWindow* osWindow, MouseButtonDownDelegate mouseButtonDownCB, MouseButtonUpDelegate mouseButtonUpCB, MouseMoveDelegate mouseMoveCB, MouseWheelDelegate mouseWheelCB)
:mouseButtonDownCB(mouseButtonDownCB),
mouseButtonUpCB(mouseButtonUpCB),
mouseMoveCB(mouseMoveCB),
mouseWheelCB(mouseWheelCB),
osWindow(osWindow)
{
    osWindow->setMouseButtonDownCallback(mouseButtonDownCB);
    osWindow->setMouseButtonUpCallback(mouseButtonUpCB);
    osWindow->setMouseMoveCallback(mouseMoveCB);
    osWindow->setMouseWheelCallback(mouseWheelCB);
}

NativeMouse::~NativeMouse()
{
    
}

//PInvoke
extern "C" _AnomalousExport NativeMouse* NativeMouse_new(NativeOSWindow* osWindow, NativeMouse::MouseButtonDownDelegate mouseButtonDownCB, NativeMouse::MouseButtonUpDelegate mouseButtonUpCB, NativeMouse::MouseMoveDelegate mouseMoveCB, NativeMouse::MouseWheelDelegate mouseWheelCB)
{
	return new NativeMouse(osWindow, mouseButtonDownCB, mouseButtonUpCB, mouseMoveCB, mouseWheelCB);
}

extern "C" _AnomalousExport void NativeMouse_delete(NativeMouse* mouse)
{
	delete mouse;
}