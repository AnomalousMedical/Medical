#include "stdafx.h"

#ifdef WINDOWS

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include "..\Resource.h"

typedef LRESULT (CALLBACK *WndFunc)(HWND hwnd,UINT uMsg,WPARAM wParam,LPARAM lParam);

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

WndFunc newWndFunc = WndProc;
WndFunc oldWndFunc;

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	return oldWndFunc(hWnd, message, wParam, lParam);
}

extern "C" _AnomalousExport void WindowFunctions_registerMultiTouchEventHandler(HWND hwnd)
{
	oldWndFunc = (WndFunc)GetWindowLong(hwnd, GWLP_WNDPROC);
	long wndProcLong = (long)newWndFunc;
	SetWindowLong(hwnd, GWLP_WNDPROC, wndProcLong);
}

extern "C" _AnomalousExport bool isMultitouchAvaliable()
{
	return false;
}

#endif

#ifdef MAC_OSX

extern "C" _AnomalousExport void WindowFunctions_registerMultiTouchEventHandler(void* hwnd)
{
	
}

extern "C" _AnomalousExport bool isMultitouchAvaliable()
{
	return false;
}

#endif