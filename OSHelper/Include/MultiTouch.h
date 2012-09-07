#pragma once

#ifdef WINDOWS
	class MultiTouch;
	typedef LRESULT (CALLBACK *WndFunc)(HWND hwnd,UINT uMsg,WPARAM wParam,LPARAM lParam);
	typedef void (*WINDOWS_REGISTRATION_FUNC)(HWND hwnd, MultiTouch* multiTouch);
#endif

class TouchInfo
{
public:
	float normalizedX;
	float normalizedY;
	int pixelX;
	int pixelY;
	int id;
};

class MultiTouch
{
public:
	virtual ~MultiTouch()
	{

	}

	virtual void fireTouchStarted(const TouchInfo& touchInfo) = 0;

	virtual void fireTouchEnded(const TouchInfo& touchInfo) = 0;

	virtual void fireTouchMoved(const TouchInfo& touchInfo) = 0;

	virtual void fireAllTouchesCanceled() = 0;

	#ifdef WINDOWS
		virtual LRESULT fireOriginalWindowFunc(HWND hwnd,UINT uMsg,WPARAM wParam,LPARAM lParam) = 0;
	#endif
};