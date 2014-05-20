#include "StdAfx.h"
#include "WinRTWindow.h"
#include "AnomalousRTFramework.h"

using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;

WinRTWindow::WinRTWindow(WinRTCoreWindowWrapper^ windowWrapper, String title, int x, int y, int width, int height, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosingDelegate closingCB, ClosedDelegate closedCB, ActivateDelegate activateCB)
:NativeOSWindow(deleteCB, sizedCB, closingCB, closedCB, activateCB),
windowWrapper(windowWrapper)
{
	windowWrapper->setNativeWindow(this);
}

WinRTWindow::~WinRTWindow()
{

}

void WinRTWindow::setTitle(String title)
{

}

void WinRTWindow::showFullScreen()
{

}

void WinRTWindow::setSize(int width, int height)
{

}

int WinRTWindow::getWidth()
{
	return windowWrapper->getWindow()->Bounds.Width;
}

int WinRTWindow::getHeight()
{
	return windowWrapper->getWindow()->Bounds.Height;
}

void* WinRTWindow::getHandle()
{
	return reinterpret_cast<void*>(windowWrapper->getWindow());
}

void WinRTWindow::show()
{

}

void WinRTWindow::close()
{

}

void WinRTWindow::setMaximized(bool maximized)
{

}

bool WinRTWindow::getMaximized()
{
	return true;
}

void WinRTWindow::setCursor(CursorType cursor)
{

}

float WinRTWindow::getWindowScaling()
{
	return 1.0f;
}

void WinRTWindow::setupMultitouch(MultiTouch* multiTouch)
{
	//This does nothing since we have to do our multitouch in another dll to remain compatable with xp.
	//See MultTouch.cpp to see how it works.
}

//PInvoke
extern "C" _AnomalousExport NativeOSWindow* NativeOSWindow_create(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, NativeOSWindow::DeleteDelegate deleteCB, NativeOSWindow::SizedDelegate sizedCB, NativeOSWindow::ClosingDelegate closingCB, NativeOSWindow::ClosedDelegate closedCB, NativeOSWindow::ActivateDelegate activateCB)
{
	return new WinRTWindow(ref new WinRTCoreWindowWrapper(AnomalousRTFramework::getSingleton()->getWindow()), caption, x, y, width, height, deleteCB, sizedCB, closingCB, closedCB, activateCB);
}