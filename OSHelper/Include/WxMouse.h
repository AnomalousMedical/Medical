#pragma once

#include "MouseButtonCode.h"

class NativeOSWindow;

#ifdef USE_WXWIDGETS

class WxMouse
{
public:
	typedef void (*MouseButtonDownDelegate)(MouseButtonCode id);
	typedef void (*MouseButtonUpDelegate)(MouseButtonCode id);
	typedef void (*MouseMoveDelegate)(int absX, int absY);
	typedef void (*MouseWheelDelegate)(int relZ);

	WxMouse(NativeOSWindow* osWindow, MouseButtonDownDelegate mouseButtonDownCB, MouseButtonUpDelegate mouseButtonUpCB, MouseMoveDelegate mouseMoveCB, MouseWheelDelegate mouseWheelCB);

	virtual ~WxMouse();

private:
	void OnMouseLeftDown(wxEvent& evt)
    {
		window->CaptureMouse();
        mouseButtonDownCB(MB_BUTTON0);
    }

    void OnMouseLeftUp(wxEvent& evt)
    {
        mouseButtonUpCB(MB_BUTTON0);
		window->ReleaseMouse();
    }

    void OnMouseLeftDouble(wxEvent& evt)
    {
        OnMouseLeftDown(evt);
        OnMouseLeftUp(evt);
    }

    void OnMouseRightDown(wxEvent& evt)
    {
		window->CaptureMouse();
        mouseButtonDownCB(MB_BUTTON1);
    }

    void OnMouseRightUp(wxEvent& evt)
    {
        mouseButtonUpCB(MB_BUTTON1);
		window->ReleaseMouse();
    }

    void OnMouseRightDouble(wxEvent& evt)
    {
        OnMouseRightDown(evt);
        OnMouseRightUp(evt);
    }

    void OnMouseMiddleDown(wxEvent& evt)
    {
		window->CaptureMouse();
        mouseButtonDownCB(MB_BUTTON2);
    }

    void OnMouseMiddleUp(wxEvent& evt)
    {
        mouseButtonUpCB(MB_BUTTON2);
		window->ReleaseMouse();
    }

    void OnMouseMotion(wxMouseEvent& evt)
    {
		wxPoint pos = evt.GetPosition();
		mouseMoveCB(pos.x, pos.y);
    }

    void OnMouseWheel(wxMouseEvent& evt)
    {
		mouseWheelCB(evt.GetWheelRotation());
    }

	MouseButtonDownDelegate mouseButtonDownCB;
	MouseButtonUpDelegate mouseButtonUpCB;
	MouseMoveDelegate mouseMoveCB;
	MouseWheelDelegate mouseWheelCB;
	NativeOSWindow* osWindow;
	wxWindow* window;
};

#endif

#ifndef USE_WXWIDGETS

class WxMouse
{
public:
	typedef void (*MouseButtonDownDelegate)(MouseButtonCode id);
	typedef void (*MouseButtonUpDelegate)(MouseButtonCode id);
	typedef void (*MouseMoveDelegate)(int absX, int absY);
	typedef void (*MouseWheelDelegate)(int relZ);
    
	WxMouse(NativeOSWindow* osWindow, MouseButtonDownDelegate mouseButtonDownCB, MouseButtonUpDelegate mouseButtonUpCB, MouseMoveDelegate mouseMoveCB, MouseWheelDelegate mouseWheelCB);
    
	virtual ~WxMouse();
    
private:
	MouseButtonDownDelegate mouseButtonDownCB;
	MouseButtonUpDelegate mouseButtonUpCB;
	MouseMoveDelegate mouseMoveCB;
	MouseWheelDelegate mouseWheelCB;
	NativeOSWindow* osWindow;
};

#endif
