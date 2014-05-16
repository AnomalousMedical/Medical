#include "stdafx.h"

extern "C" _AnomalousExport uint SystemInfo_getDisplayCount()
{
#ifdef WINDOWS
	return GetSystemMetrics(SM_CMONITORS);
#endif
#ifdef WINRT
	return 1;
#endif
}

class MonitorFinder
{
public:
	MonitorFinder(int index)
		:x(0),
		y(0),
		index(index),
		currentIndex(0)
	{

	}

	bool processMonitor(LPRECT lprcMonitor)
	{
		if(currentIndex == index)
		{
			x = lprcMonitor->left;
			y = lprcMonitor->top;
			return false;
		}
		else
		{
			++currentIndex;
			return true;
		}
	}

	int getX()
	{
		return x;
	}

	int getY()
	{
		return y;
	}

private:
	int x;
	int y;
	int index;
	int currentIndex;
};

BOOL CALLBACK FindMonitorsCallBack(HMONITOR hMonitor, HDC hdcMonitor, LPRECT lprcMonitor, LPARAM dwData)
{
	MonitorFinder* monitorFinder = (MonitorFinder*)dwData;
	return monitorFinder->processMonitor(lprcMonitor);
}

extern "C" _AnomalousExport void SystemInfo_getDisplayLocation(int displayIndex, int& x, int& y)
{
#ifdef WINDOWS
	MonitorFinder monitorFinder(displayIndex);
	EnumDisplayMonitors(NULL, NULL, FindMonitorsCallBack, (LPARAM)&monitorFinder);
	x = monitorFinder.getX();
	y = monitorFinder.getY();
#endif

#ifdef WINRT
	x = 0;
	y = 0;
#endif
}