#pragma once

#ifdef USE_WXWIDGETS

class NativeOSWindow : public wxFrame
{
public:
	typedef void (*DeleteDelegate)();
	typedef void (*SizedDelegate)();
	typedef void (*ClosedDelegate)();
	typedef void (*ActivateDelegate)(bool active);

	NativeOSWindow(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activateCB);

	virtual ~NativeOSWindow(void);

private:
	DeleteDelegate deleteCB;
	SizedDelegate sizedCB;
	ClosedDelegate closedCB;
	ActivateDelegate activateCB;

	void OnSize(wxEvent& event)
	{
		event.Skip();
		sizedCB();
	}

	void OnClose(wxEvent& event)
	{
		event.Skip();
		closedCB();
	}

	void OnActivate(wxActivateEvent& event)
	{
		event.Skip();
		activateCB(event.GetActive());
	}
};

#endif

#ifndef USE_WXWIDGETS

#include "KeyboardButtonCode.h"
#include "MouseButtonCode.h"

class NativeOSWindow
{
public:
	typedef void (*DeleteDelegate)();
	typedef void (*SizedDelegate)();
	typedef void (*ClosedDelegate)();
	typedef void (*ActivateDelegate)(bool active);
    
    typedef void (*KeyDownDelegate)(KeyboardButtonCode keyCode, uint character);
	typedef void (*KeyUpDelegate)(KeyboardButtonCode keyCode);
    
    typedef void (*MouseButtonDownDelegate)(MouseButtonCode id);
	typedef void (*MouseButtonUpDelegate)(MouseButtonCode id);
	typedef void (*MouseMoveDelegate)(int absX, int absY);
	typedef void (*MouseWheelDelegate)(int relZ);
    
	NativeOSWindow(DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activateCB);
    
	virtual ~NativeOSWindow(void);
    
    virtual void setTitle(String title) = 0;
    
    virtual void showFullScreen() = 0;
    
    virtual void setSize(int width, int height) = 0;
    
    virtual int getWidth() = 0;
    
    virtual int getHeight() = 0;
    
    virtual void* getHandle() = 0;
    
    virtual void show() = 0;
    
    virtual void close() = 0;
    
    virtual void setMaximized(bool maximized) = 0;
    
    virtual bool getMaximized() = 0;
    
    virtual void setCursor(/*CursorType cursor*/) = 0;
    
    void fireSized()
	{
		sizedCB();
	}
    
	void fireClosed()
	{
		closedCB();
	}
    
	void fireActivate(bool active)
	{
		activateCB(active);
	}
    
    void setKeyDownCallback(KeyDownDelegate keyDown)
    {
        keyDownCB = keyDown;
    }
    
    void fireKeyDown(KeyboardButtonCode keyCode, uint character)
    {
        if(keyDownCB != 0)
        {
            keyDownCB(keyCode, character);
        }
    }
    
    void setKeyUpCallback(KeyUpDelegate keyUp)
    {
        keyUpCB = keyUp;
    }
    
	void fireKeyUp(KeyboardButtonCode keyCode)
    {
        if(keyUpCB != 0)
        {
            keyUpCB(keyCode);
        }
    }
    
    void setMouseButtonDownCallback(MouseButtonDownDelegate mouseButtonDown)
    {
        this->mouseButtonDownCB = mouseButtonDown;
    }
    
    void fireMouseButtonDown(MouseButtonCode id)
    {
        if(mouseButtonDownCB != 0)
        {
            mouseButtonDownCB(id);
        }
    }
    
    void setMouseButtonUpCallback(MouseButtonUpDelegate mouseButtonUp)
    {
        this->mouseButtonUpCB = mouseButtonUp;
    }
    
	void fireMouseButtonUp(MouseButtonCode id)
    {
        if(mouseButtonUpCB != 0)
        {
            mouseButtonUpCB(id);
        }
    }
    
    void setMouseMoveCallback(MouseMoveDelegate mouseMove)
    {
        this->mouseMoveCB = mouseMove;
    }
    
	void fireMouseMove(int absX, int absY)
    {
        if(mouseMoveCB != 0)
        {
            mouseMoveCB(absX, absY);
        }
    }
    
    void setMouseWheelCallback(MouseWheelDelegate mouseWheel)
    {
        this->mouseWheelCB = mouseWheel;
    }
    
	void fireMouseWheel(int relZ)
    {
        if(mouseWheelCB != 0)
        {
            mouseWheelCB(relZ);
        }
    }
    
private:
	DeleteDelegate deleteCB;
	SizedDelegate sizedCB;
	ClosedDelegate closedCB;
	ActivateDelegate activateCB;
    
    KeyDownDelegate keyDownCB;
	KeyUpDelegate keyUpCB;
    
    MouseButtonDownDelegate mouseButtonDownCB;
	MouseButtonUpDelegate mouseButtonUpCB;
	MouseMoveDelegate mouseMoveCB;
	MouseWheelDelegate mouseWheelCB;
};

#endif