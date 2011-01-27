#include "stdafx.h"

//#define MULTITOUCH

#ifdef MULTITOUCH

#ifdef WINDOWS

#ifndef WINVER                  // Specifies that the minimum required platform is Windows 7.
#define WINVER 0x0601           // Change this to the appropriate value to target other versions of Windows.
#endif

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <windowsx.h>   // included for point conversion
#include "..\Resource.h"
#include <iostream>

typedef LRESULT (CALLBACK *WndFunc)(HWND hwnd,UINT uMsg,WPARAM wParam,LPARAM lParam);

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

WndFunc newWndFunc = WndProc;
WndFunc oldWndFunc;

int wmId, wmEvent, i, x, y;

UINT cInputs;
PTOUCHINPUT pInputs;
POINT ptInput;  

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch(message)
	{
	case WM_TOUCH:
		cInputs = LOWORD(wParam);
		pInputs = new TOUCHINPUT[cInputs];
		if (pInputs)
		{
			if (GetTouchInputInfo((HTOUCHINPUT)lParam, cInputs, pInputs, sizeof(TOUCHINPUT)))
			{
				for (int i=0; i < static_cast<INT>(cInputs); i++)
				{
					TOUCHINPUT ti = pInputs[i];
					if(ti.dwFlags & TOUCHEVENTF_MOVE)
					{
						std::cout << "Moved " << ti.dwID << " x " << ti.x << " y " << ti.y << " contactX " << ti.cxContact << " contactY " << ti.cyContact << std::endl;
					}
					else if(ti.dwFlags & TOUCHEVENTF_DOWN)
					{
						std::cout << "Touched down" << ti.dwID  << std::endl;
					}
					else if(ti.dwFlags & TOUCHEVENTF_UP)
					{
						std::cout << "Touched up " << ti.dwID << std::endl;
					}
					//Don't process these, but leave them for reference
					/*else if(ti.dwFlags & TOUCHEVENTF_INRANGE)
					{
						std::cout << "Touched in range " << ti.dwID << std::endl;
					}
					else if(ti.dwFlags & TOUCHEVENTF_PRIMARY)
					{
						std::cout << "Touched primary " << ti.dwID << std::endl;
					}
					else if(ti.dwFlags & TOUCHEVENTF_NOCOALESCE)
					{
						std::cout << "Touched nocoalesce " << ti.dwID << std::endl;
					}
					else if(ti.dwFlags & TOUCHEVENTF_PEN)
					{
						std::cout << "Touched pen " << ti.dwID << std::endl;
					}
					else if(ti.dwFlags & TOUCHEVENTF_PALM)
					{
						std::cout << "Touched palm " << ti.dwID << std::endl;
					}*/
				}
			}
			// If you handled the message and don't want anything else done with it, you can close it
			CloseTouchInputHandle((HTOUCHINPUT)lParam);
			delete [] pInputs;
		}else{
			// Handle the error here 
		}  


		break;
	}

	return oldWndFunc(hWnd, message, wParam, lParam);
}

extern "C" _AnomalousExport void MultiTouch_registerMultiTouchEventHandler(HWND hwnd)
{
	RegisterTouchWindow(hwnd, 0);

	oldWndFunc = (WndFunc)GetWindowLong(hwnd, GWLP_WNDPROC);
	long wndProcLong = (long)newWndFunc;
	SetWindowLong(hwnd, GWLP_WNDPROC, wndProcLong);
}

extern "C" _AnomalousExport bool MultiTouch_isMultitouchAvailable()
{
	return true;
}

#endif //WINDOWS

#ifdef MAC_OSX

extern "C" _AnomalousExport void MultiTouch_registerMultiTouchEventHandler(void* hwnd)
{

}

extern "C" _AnomalousExport bool MultiTouch_isMultitouchAvailable()
{
	return false;
}

#endif //MAC_OSX

#else //Else from MULTITOUCH

extern "C" _AnomalousExport void MultiTouch_registerMultiTouchEventHandler(void* hwnd)
{

}

extern "C" _AnomalousExport bool MultiTouch_isMultitouchAvailable()
{
	return false;
}

#endif //MULTITOUCH Else