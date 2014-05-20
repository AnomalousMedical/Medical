#pragma once

#include "NativeOSWindow.h"
#include "WinRTCoreWindowWrapper.h"
#include <agile.h>

class WinRTWindow : public NativeOSWindow
{
public:
	WinRTWindow(WinRTCoreWindowWrapper^ windowWrapper, String title, int x, int y, int width, int height, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosingDelegate closingCB, ClosedDelegate closedCB, ActivateDelegate activateCB);
    
	virtual ~WinRTWindow();

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

	virtual float getWindowScaling();

	virtual void setupMultitouch(MultiTouch* multiTouch);

private:
	Platform::Agile<WinRTCoreWindowWrapper^> windowWrapper;
};