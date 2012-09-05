#include "StdAfx.h"

#ifndef USE_WXWIDGETS
#include "Win32Window.h"

LRESULT WINAPI MsgProc( HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam );

Win32Window::Win32Window(String title, int x, int y, int width, int height, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activateCB)
	:NativeOSWindow(deleteCB, sizedCB, closedCB, activateCB),
	window(0)
{	
	window = CreateWindow(WINDOW_CLASS_NAME, title, WS_OVERLAPPEDWINDOW, x, y, width, height, NULL, NULL, (HINSTANCE)GetModuleHandle(NULL), NULL);
	SetWindowLong(window, GWL_USERDATA, (LONG)this);
}
    
Win32Window::~Win32Window()
{
	if(window)
	{
		DestroyWindow(window);
		window = 0;
	}
}
    
void Win32Window::setTitle(String title)
{
	SetWindowTextA(window, title);
}
    
void Win32Window::showFullScreen()
{

}
    
void Win32Window::setSize(int width, int height)
{
	SetWindowPos(window, NULL, 0, 0, width, height, SWP_NOMOVE|SWP_NOZORDER|SWP_NOACTIVATE);
}
    
int Win32Window::getWidth()
{
	RECT windowRect;
	GetClientRect(window, &windowRect);
	return windowRect.right - windowRect.left;
}
    
int Win32Window::getHeight()
{
	RECT windowRect;
	GetClientRect(window, &windowRect);
	return windowRect.bottom - windowRect.top;
}

void* Win32Window::getHandle()
{
	return window;
}
    
void Win32Window::show()
{
	WINDOWPLACEMENT placement;
	GetWindowPlacement(window, &placement);
	ShowWindow(window, placement.showCmd);
	UpdateWindow(window);
}
    
void Win32Window::close()
{
	PostMessage(window, WM_CLOSE, 0, 0);
}
    
void Win32Window::setMaximized(bool maximized)
{
	WINDOWPLACEMENT placement;
	GetWindowPlacement(window, &placement);
	if(maximized)
	{
		placement.showCmd = SW_MAXIMIZE;
	}
	else
	{
		placement.showCmd = SW_SHOWNORMAL;
	}
	SetWindowPlacement(window, &placement);
}
    
bool Win32Window::getMaximized()
{
	WINDOWPLACEMENT placement;
	GetWindowPlacement(window, &placement);
	return placement.showCmd == SW_MAXIMIZE;
}
    
void Win32Window::setCursor(CursorType cursor)
{

}

//PInvoke
extern "C" _AnomalousExport NativeOSWindow* NativeOSWindow_create(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, NativeOSWindow::DeleteDelegate deleteCB, NativeOSWindow::SizedDelegate sizedCB, NativeOSWindow::ClosedDelegate closedCB, NativeOSWindow::ActivateDelegate activateCB)
{
	return new Win32Window(caption, x, y, width, height, deleteCB, sizedCB, closedCB, activateCB);
}

#endif