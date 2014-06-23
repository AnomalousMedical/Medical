#pragma once


#if defined(WINDOWS) || defined(WINRT)
#define Int64 LONGLONG
#endif

#ifdef MAC_OSX
#include <sys/time.h>
#include <stdint.h>
#define Int64 int64_t
#endif

/// <summary>
/// This class provides access to the QueryPerformanceCounter functions as a
/// timer that returns delta times between calls to getDelta. First call
/// initialize to get the frequency and see if the counter is valid. Then call
/// prime to set the start time followed by calls to getDelta to get the delta
/// between the call to prime, or between calls to getDelta. Prime would be
/// called outside of the loop.
/// </summary>
class PerformanceCounter
{
private:	
#if defined(WINDOWS) || defined(WINRT)
	DWORD startTick;
	LONGLONG lastTime;
	LARGE_INTEGER startTime;
	LARGE_INTEGER frequency;

#endif

#ifdef MAC_OSX
	struct timeval start;
#endif

    bool accurate;

public:
	/// <summary>
	/// Constructor.
	/// </summary>
	PerformanceCounter();

	/// <summary>
	/// Destructor.
	/// </summary>
	virtual ~PerformanceCounter();

	/// <summary>
	/// Initialize the counter. Will return true if the counter can be used.
	/// </summary>
	/// <returns>True if the counter can be used.</returns>
	bool initialize();

	/// <summary>
	/// Get the current time in microseconds.
	/// </summary>
	/// <returns>The current time in microseconds.</returns>
	Int64 getCurrentTime();

	void setAccurate(bool accurate);

	bool isAccurate();

private:
#ifdef WINRT
	DWORD GetTickCount() { return (DWORD)GetTickCount64(); }
#endif
};