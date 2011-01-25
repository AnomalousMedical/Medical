#include "StdAfx.h"
#include "..\Include\WxOSWindow.h"

WxOSWindow::WxOSWindow(wxWindow* window)
:window(window),
closedCallback(NULL),
sizedCallback(NULL)
{
	window->Bind(wxEVT_SIZE, &WxOSWindow::OnSize, this);
	window->Bind(wxEVT_CLOSE_WINDOW, &WxOSWindow::OnClose, this);
}

WxOSWindow::~WxOSWindow(void)
{
}

WXWidget WxOSWindow::getHandle()
{
	return window->GetHandle();
}

int WxOSWindow::getWidth()
{
	int w, h;
	window->GetClientSize(&w, &h);
	return w;
}

int WxOSWindow::getHeight()
{
	int w, h;
	window->GetClientSize(&w, &h);
	return h;
}

void WxOSWindow::registerCallbacks(NativeCallback closedCallback, NativeCallback sizedCallback)
{
	this->closedCallback = closedCallback;
	this->sizedCallback = sizedCallback;
}

//PInvoke interface

extern "C" _AnomalousExport WXWidget WxOSWindow_getHandle(WxOSWindow* window)
{
	return window->getHandle();
}

extern "C" _AnomalousExport int WxOSWindow_getWidth(WxOSWindow* window)
{
	return window->getWidth();
}

extern "C" _AnomalousExport int WxOSWindow_getHeight(WxOSWindow* window)
{
	return window->getHeight();
}

extern "C" _AnomalousExport void WxOSWindow_registerCallbacks(WxOSWindow* window, WxOSWindow::NativeCallback closedCallback, WxOSWindow::NativeCallback sizedCallback)
{
	window->registerCallbacks(closedCallback, sizedCallback);
}