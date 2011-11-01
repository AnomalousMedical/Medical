#include "StdAfx.h"
#include "WxMouse.h"
#include "NativeOSWindow.h"

WxMouse::WxMouse(NativeOSWindow* osWindow, MouseButtonDownDelegate mouseButtonDownCB, MouseButtonUpDelegate mouseButtonUpCB, MouseMoveDelegate mouseMoveCB, MouseWheelDelegate mouseWheelCB)
:osWindow(osWindow),
mouseButtonDownCB(mouseButtonDownCB),
mouseButtonUpCB(mouseButtonUpCB),
mouseMoveCB(mouseMoveCB),
mouseWheelCB(mouseWheelCB),
window(osWindow)
{
	window->Bind(wxEVT_LEFT_DOWN, &WxMouse::OnMouseLeftDown, this);
	window->Bind(wxEVT_LEFT_UP, &WxMouse::OnMouseLeftUp, this);
	window->Bind(wxEVT_LEFT_DCLICK, &WxMouse::OnMouseLeftDouble, this); //WxWidgets will block the double click, but we want to fire off the events anyway

	window->Bind(wxEVT_RIGHT_DOWN, &WxMouse::OnMouseRightDown, this);
	window->Bind(wxEVT_RIGHT_UP, &WxMouse::OnMouseRightUp, this);
	window->Bind(wxEVT_RIGHT_DCLICK, &WxMouse::OnMouseRightDouble, this); //WxWidgets will block the double click, but we want to fire off the events anyway

	window->Bind(wxEVT_MIDDLE_DOWN, &WxMouse::OnMouseMiddleDown, this);
	window->Bind(wxEVT_MIDDLE_UP, &WxMouse::OnMouseMiddleUp, this);

	window->Bind(wxEVT_MOTION, &WxMouse::OnMouseMotion, this);

	window->Bind(wxEVT_MOUSEWHEEL, &WxMouse::OnMouseWheel, this);

	//window->Bind(wxEVT_MOUSE_CAPTURE_LOST
}

WxMouse::~WxMouse(void)
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