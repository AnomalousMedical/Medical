#include "StdAfx.h"
#include "WxMouse.h"
#include "NativeOSWindow.h"

WxMouse::WxMouse(NativeOSWindow* osWindow, MouseButtonDownDelegate mouseButtonDownCB, MouseButtonUpDelegate mouseButtonUpCB, MouseMoveDelegate mouseMoveCB, MouseWheelDelegate mouseWheelCB)
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

WxMouse::~WxMouse()
{
    
}

//PInvoke
extern "C" _AnomalousExport WxMouse* WxMouse_new(NativeOSWindow* osWindow, WxMouse::MouseButtonDownDelegate mouseButtonDownCB, WxMouse::MouseButtonUpDelegate mouseButtonUpCB, WxMouse::MouseMoveDelegate mouseMoveCB, WxMouse::MouseWheelDelegate mouseWheelCB)
{
	return new WxMouse(osWindow, mouseButtonDownCB, mouseButtonUpCB, mouseMoveCB, mouseWheelCB);
}

extern "C" _AnomalousExport void WxMouse_delete(WxMouse* mouse)
{
	delete mouse;
}