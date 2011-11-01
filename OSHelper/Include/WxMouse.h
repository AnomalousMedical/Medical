#pragma once

enum MouseButtonCode
{
    MB_BUTTON0 = 0,
    MB_BUTTON1 = 1,
    MB_BUTTON2 = 2,
    MB_BUTTON3 = 3,
    MB_BUTTON4 = 4,
    MB_BUTTON5 = 5,
    MB_BUTTON6 = 6,
    MB_BUTTON7 = 7,
    NUM_BUTTONS = 8,
};

class NativeOSWindow;

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
