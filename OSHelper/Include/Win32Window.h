#pragma once

#include "NativeOSWindow.h"

class Win32Window : public NativeOSWindow
{
public:
	Win32Window(String title, int x, int y, int width, int height, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activateCB);
    
    virtual ~Win32Window();
    
    virtual void setTitle(String title);
    
    virtual void showFullScreen();
    
    virtual void setSize(int width, int height);
    
    virtual int getWidth();
    
    virtual int getHeight();
    
    virtual void* getHandle();
    
    virtual void show();
    
    virtual void close();
    
    virtual void setMaximized(bool maximized);
    
    virtual bool getMaximized();
    
    virtual void setCursor(CursorType cursor);
    
    //createMenu()
private:
	HWND window;
	WNDCLASSEX wndclass;
};