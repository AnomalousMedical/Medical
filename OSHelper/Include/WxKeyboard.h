#pragma once

#include "KeyboardButtonCode.h"

class NativeOSWindow;

#ifdef USE_WXWIDGETS

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
	int downKeyCode;

	void OnKeyDown(wxKeyEvent& kevt);
	void OnChar(wxKeyEvent& kevt);
	void OnKeyUp(wxKeyEvent& kevt);

	static KeyboardButtonCode keyConverter[397];
	static void createConverterTable();
};

#endif

#ifndef USE_WXWIDGETS

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

#endif