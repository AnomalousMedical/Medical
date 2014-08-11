#pragma once

#include "NativeOSWindow.h"

#define WIN32_WINDOW_CLASS L"Win32WindowClass"

class Win32Window : public NativeOSWindow
{
public:
	Win32Window(HWND parent, String title, int x, int y, int width, int height, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosingDelegate closingCB, ClosedDelegate closedCB, ActivateDelegate activateCB);
    
    virtual ~Win32Window();
    
    virtual void setTitle(String title);
    
    virtual void setSize(int width, int height);
    
    virtual int getWidth();
    
    virtual int getHeight();
    
    virtual void* getHandle();
    
    virtual void show();
    
    virtual void close();
    
    virtual void setMaximized(bool maximized);
    
    virtual bool getMaximized();
    
    virtual void setCursor(CursorType cursor);

	virtual float getWindowScaling();

	virtual void toggleFullscreen();

	void activateCursor()
	{
		SetCursor(hCursor);
	}
    
    virtual void setupMultitouch(MultiTouch* multiTouch)
	{
		//This does nothing since we have to do our multitouch in another dll to remain compatable with xp.
		//See MultTouch.cpp to see how it works.
	}

	void manageCapture(MouseButtonCode mouseCode);

	void manageRelease(MouseButtonCode mouseCode);

	static void createWindowClass(HANDLE hModule);

	static void destroyWindowClass();
private:
	HWND window;
	static WNDCLASSEX wndclass;
	HCURSOR hCursor;
	bool mouseDown[MouseButtonCode::NUM_BUTTONS];
	WINDOWPLACEMENT previousWindowPlacement;
};