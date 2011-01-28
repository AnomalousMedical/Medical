#pragma once

#ifdef WINDOWS
	typedef LRESULT (CALLBACK *WndFunc)(HWND hwnd,UINT uMsg,WPARAM wParam,LPARAM lParam);
	typedef HWND WindowType;
#endif

class TouchInfo
{
public:
	float normalizedX;
	float normalizedY;
	int id;
};

typedef void (*TouchEventDelegate)(TouchInfo touchInfo);

class MultiTouch
{
public:
	MultiTouch(WindowType window, TouchEventDelegate touchStartedCB, TouchEventDelegate touchEndedCB, TouchEventDelegate touchMovedCB)
		:window(window),
		touchStartedCB(touchStartedCB),
		touchEndedCB(touchEndedCB),
		touchMovedCB(touchMovedCB)
#ifdef WINDOWS
		,originalWindowFunction((WndFunc)GetWindowLong(window, GWLP_WNDPROC))
#endif
	{

	}

	virtual ~MultiTouch()
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