#include "StdAfx.h"
#include "WxKeyboard.h"
#include "NativeOSWindow.h"

WxKeyboard::WxKeyboard(NativeOSWindow* osWindow, KeyDownDelegate keyDownCB, KeyUpDelegate keyUpCB)
:osWindow(osWindow),
keyDownCB(keyDownCB),
keyUpCB(keyUpCB)
{
    osWindow->setKeyDownCallback(keyDownCB);
    osWindow->setKeyUpCallback(keyUpCB);
}

WxKeyboard::~WxKeyboard(void)
{
    
}

//PInvoke
extern "C" _AnomalousExport WxKeyboard* WxKeyboard_new(NativeOSWindow* osWindow, WxKeyboard::KeyDownDelegate keyDownCB, WxKeyboard::KeyUpDelegate keyUpCB)
{
	return new WxKeyboard(osWindow, keyDownCB, keyUpCB);
}

extern "C" _AnomalousExport void WxKeyboard_delete(WxKeyboard* keyboard)
{
	delete keyboard;
}