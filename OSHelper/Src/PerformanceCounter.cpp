#include "stdafx.h"
#include "PerformanceCounter.h"
#if defined(WINDOWS) || defined(WINRT)
#include "winbase.h"
#include "mmsystem.h"
#include <algorithm>
#endif

PerformanceCounter::PerformanceCounter()
:accurate(false)
{
	
}

PerformanceCounter::~PerformanceCounter()
{
	setAccurate(false);
}

bool PerformanceCounter::initialize()
{
#if defined(WINDOWS) || defined(WINRT)
#if defined(WINDOWS) && !defined(_WIN64)
	DWORD procMask;
	DWORD sysMask;
	DWORD timerMask;

	//Find the lowest used core
	GetProcessAffinityMask(GetCurrentProcess(), &procMask, &sysMask);
	if(procMask == 0)
	{
		procMask = 1;
	}
	if(timerMask == 0)
	{
		timerMask = 1;
		while( ( timerMask & procMask ) == 0 )
		{
			timerMask <<= 1;
		}
	}

	//Change affinity and read counter values
	HANDLE thread = GetCurrentThread();
	DWORD oldMask = SetThreadAffinityMask(thread, timerMask);
#endif

	bool valid = QueryPerformanceFrequency(&frequency);
	if(valid)
	{
		QueryPerformanceCounter(&startTime);
		startTick = GetTickCount();
	}

	//Finish and return
	lastTime = 0;
	return valid;
#endif

#ifdef MAC_OSX
	gettimeofday(&start, NULL);	
	return true;
#endif
}

Int64 PerformanceCounter::getCurrentTime()
{

#if defined(WINDOWS) || defined(WINRT)
	LARGE_INTEGER currentTime;
	QueryPerformanceCounter(&currentTime);

	//Compute the number of ticks in milliseconds since initialize was called.
	LONGLONG time = currentTime.QuadPart - startTime.QuadPart;
	LONGLONG ticks = 1000 * time / frequency.QuadPart;

	//Check for performance counter leaps
	DWORD check = GetTickCount() - startTick;
	signed long off = (signed long)(ticks - check);
	if(off < -100 || off > 100)
	{
		//Adjust timer
		LONGLONG adjust = (std::min)(off * frequency.QuadPart / 1000, time - lastTime);
        startTime.QuadPart += adjust;
        time -= adjust;
	}

	lastTime = time;

	return 1000000 * time / frequency.QuadPart;
#endif

#ifdef MAC_OSX
	struct timeval now;
	gettimeofday(&now, NULL);
	Int64 intNow = (Int64)(now.tv_sec - start.tv_sec)*1000000LL+(Int64)(now.tv_usec - start.tv_usec);
	return intNow;
#endif
}

void PerformanceCounter::setAccurate(bool accurate)
{
#ifdef WINDOWS //Not sure about windows rt and timeBeginPeriod
	if (this->accurate != accurate)
	{
		this->accurate = accurate;
		if (this->accurate)
		{
			timeBeginPeriod(1);
		}
		else
		{
			timeEndPeriod(1);
		}
	}
#endif
}

bool PerformanceCounter::isAccurate()
{
	return accurate;
}

extern "C" _AnomalousExport PerformanceCounter* PerformanceCounter_Create()
{
	return new PerformanceCounter();
}

extern "C" _AnomalousExport void PerformanceCounter_Delete(PerformanceCounter* counter)
{
	delete counter;
}

extern "C" _AnomalousExport bool PerformanceCounter_initialize(PerformanceCounter* counter)
{
	return counter->initialize();
}

extern "C" _AnomalousExport Int64 PerformanceCounter_getCurrentTime(PerformanceCounter* counter)
{
	return counter->getCurrentTime();
}

extern "C" _AnomalousExport void PerformanceCounter_setAccurate(PerformanceCounter* counter, bool accurate)
{
	counter->setAccurate(accurate);
}

extern "C" _AnomalousExport bool PerformanceCounter_isAccurate(PerformanceCounter* counter)
{
	return counter->isAccurate();
}