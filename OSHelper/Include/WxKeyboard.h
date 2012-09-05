#pragma once

#include "KeyboardButtonCode.h"

class NativeOSWindow;

class WxKeyboard
{
public:
	typedef void (*KeyDownDelegate)(KeyboardButtonCode keyCode, uint character);
	typedef void (*KeyUpDelegate)(KeyboardButtonCode keyCode);
    
	WxKeyboard(NativeOSWindow* osWindow, KeyDownDelegate keyDownCB, KeyUpDelegate keyUpCB);
    
	~WxKeyboard(void);
    
private:
	KeyDownDelegate keyDownCB;
	KeyUpDelegate keyUpCB;
	NativeOSWindow* osWindow;
};