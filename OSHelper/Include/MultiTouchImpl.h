#pragma once

#include "MultiTouch.h"

typedef void (*TouchEventDelegate)(TouchInfo touchInfo);
typedef void (*TouchEventCanceledDelegate)();

class MultiTouchImpl : public MultiTouch
{
public:
	MultiTouchImpl(WindowType window, TouchEventDelegate touchStartedCB, TouchEventDelegate touchEndedCB, TouchEventDelegate touchMovedCB, TouchEventCanceledDelegate touchCanceledCB)
		:window(window),
		touchStartedCB(touchStartedCB),
		touchEndedCB(touchEndedCB),
		touchMovedCB(touchMovedCB),
		touchCanceledCB(touchCanceledCB)
#ifdef WINDOWS
		,originalWindowFunction((WndFunc)GetWindowLong(window, GWLP_WNDPROC))
#endif
	{

	}

	virtual ~MultiTouchImpl()
	{

	}

	virtual void fireTouchStarted(const TouchInfo& touchInfo)
	{
		touchStartedCB(touchInfo);
	}

	virtual void fireTouchEnded(const TouchInfo& touchInfo)
	{
		touchEndedCB(touchInfo);
	}

	virtual void fireTouchMoved(const TouchInfo& touchInfo)
	{
		touchMovedCB(touchInfo);
	}

	virtual void fireAllTouchesCanceled()
	{
		touchCanceledCB();
	}

	#ifdef WINDOWS
		virtual LRESULT fireOriginalWindowFunc(HWND hwnd,UINT uMsg,WPARAM wParam,LPARAM lParam)
		{
			return originalWindowFunction(hwnd, uMsg, wParam, lParam);
		}
	#endif

private:
	WindowType window;
	TouchEventDelegate touchStartedCB;
	TouchEventDelegate touchEndedCB;
	TouchEventDelegate touchMovedCB;
	TouchEventCanceledDelegate touchCanceledCB;
	#ifdef WINDOWS
		WndFunc originalWindowFunction;
	#endif
};
