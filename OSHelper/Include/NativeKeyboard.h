#pragma once

#include "KeyboardButtonCode.h"

class NativeOSWindow;

class NativeKeyboard
{
public:
	typedef void (*KeyDownDelegate)(KeyboardButtonCode keyCode, uint character);
	typedef void (*KeyUpDelegate)(KeyboardButtonCode keyCode);
    
	NativeKeyboard(NativeOSWindow* osWindow, KeyDownDelegate keyDownCB, KeyUpDelegate keyUpCB);
    
	virtual ~NativeKeyboard(void);
    
private:
	KeyDownDelegate keyDownCB;
	KeyUpDelegate keyUpCB;
	NativeOSWindow* osWindow;
};