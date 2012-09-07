#include "StdAfx.h"
#include "NativeKeyboard.h"
#include "NativeOSWindow.h"

NativeKeyboard::NativeKeyboard(NativeOSWindow* osWindow, KeyDownDelegate keyDownCB, KeyUpDelegate keyUpCB)
:osWindow(osWindow),
keyDownCB(keyDownCB),
keyUpCB(keyUpCB)
{
    osWindow->setKeyDownCallback(keyDownCB);
    osWindow->setKeyUpCallback(keyUpCB);
}

NativeKeyboard::~NativeKeyboard(void)
{
    
}

//PInvoke
extern "C" _AnomalousExport NativeKeyboard* NativeKeyboard_new(NativeOSWindow* osWindow, NativeKeyboard::KeyDownDelegate keyDownCB, NativeKeyboard::KeyUpDelegate keyUpCB)
{
	return new NativeKeyboard(osWindow, keyDownCB, keyUpCB);
}

extern "C" _AnomalousExport void NativeKeyboard_delete(NativeKeyboard* keyboard)
{
	delete keyboard;
}