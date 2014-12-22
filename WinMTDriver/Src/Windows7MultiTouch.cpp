#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <windows.h>
#include <windowsx.h>   // included for point conversion
#include "Windows7MultiTouch.h"
#include "MultiTouch.h"
#include "..\Resource.h"
#include <hash_map>

UINT cInputs;
PTOUCHINPUT pInputs;
TouchInfo touchInfo;
stdext::hash_map<HWND, MultiTouch*> windowToTouchMap;
MONITORINFO monitorInfo;

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	MultiTouch* multiTouch = windowToTouchMap[hWnd];
	switch(message)
	{
	case WM_TOUCH:
		cInputs = LOWORD(wParam);
		pInputs = new TOUCHINPUT[cInputs];
		if (pInputs)
		{
			if (GetTouchInputInfo((HTOUCHINPUT)lParam, cInputs, pInputs, sizeof(TOUCHINPUT)))
			{
				HMONITOR monitor = MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST);
				monitorInfo.cbSize = sizeof(MONITORINFO);
				GetMonitorInfo(monitor, &monitorInfo);
				float width = (monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left) * 100.0f;
				float height = (monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top) * 100.0f;
				POINT ptConvert;

				for (int i=0; i < static_cast<INT>(cInputs); i++)
				{
					TOUCHINPUT ti = pInputs[i];
					touchInfo.id = ti.dwID;

					//Convert point to window coord
					ptConvert.x = TOUCH_COORD_TO_PIXEL(ti.x);
					ptConvert.y = TOUCH_COORD_TO_PIXEL(ti.y);
					ScreenToClient(hWnd, &ptConvert);
					touchInfo.pixelX = ptConvert.x;
					touchInfo.pixelY = ptConvert.y;

					//Set the rest of the info
					touchInfo.normalizedX = ti.x / width;
					touchInfo.normalizedY = ti.y / height;
					if(ti.dwFlags & TOUCHEVENTF_MOVE)
					{
						multiTouch->fireTouchMoved(touchInfo);
					}
					else if(ti.dwFlags & TOUCHEVENTF_DOWN)
					{
						multiTouch->fireTouchStarted(touchInfo);
					}
					else if(ti.dwFlags & TOUCHEVENTF_UP)
					{
						multiTouch->fireTouchEnded(touchInfo);
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
		return 0;
		break;
	}
	return multiTouch->fireOriginalWindowFunc(hWnd, message, wParam, lParam);
}

extern "C" _declspec(dllexport) void registerWithWindows(HWND hwnd, MultiTouch* multiTouch)
{
	windowToTouchMap[hwnd] = multiTouch;
	RegisterTouchWindow(hwnd, 0);
	LONG_PTR wndProcLong = (LONG_PTR)WndProc;
	SetWindowLongPtr(hwnd, GWLP_WNDPROC, wndProcLong);
}