#pragma once

#include "MultiTouch.h"

typedef void (*TouchEventDelegate)(TouchInfo touchInfo);

class MultiTouchImpl : public MultiTouch
{
public:
	MultiTouchImpl(WindowType window, TouchEventDelegate touchStartedCB, TouchEventDelegate touchEndedCB, TouchEventDelegate touchMovedCB)
		:window(window),
		touchStartedCB(touchStartedCB),
		touchEndedCB(touchEndedCB),
		touchMovedCB(touchMovedCB)
#ifdef WINDOWS
		,originalWindowFunction((WndFunc)GetWindowLong(window, GWLP_WNDPROC))
#endif
	{

	}

	virtual ~MultiTouchImpl()
	{

	}

	void fireTouchStarted(const TouchInfo& touchInfo)
	{
		touchStartedCB(touchInfo);
	}

	void fireTouchEnded(const TouchInfo& touchInfo)
	{
		touchEndedCB(touchInfo);
	}

	void fireTouchMoved(const TouchInfo& touchInfo)
	{
		touchMovedCB(touchInfo);
	}

	#ifdef WINDOWS
		LRESULT fireOriginalWindowFunc(HWND hwnd,UINT uMsg,WPARAM wParam,LPARAM lParam)
		{
			return originalWindowFunction(hwnd, uMsg, wParam, lParam);
		}
	#endif

private:
	WindowType window;
	TouchEventDelegate touchStartedCB;
	TouchEventDelegate touchEndedCB;
	TouchEventDelegate touchMovedCB;
	#ifdef WINDOWS
		WndFunc originalWindowFunction;
	#endif
};
