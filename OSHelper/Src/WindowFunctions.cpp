#include "Stdafx.h"

enum WindowIcons
{
	ICON_SKULL
};

#ifdef WINDOWS

extern "C" _AnomalousExport void WindowFunctions_pumpMessages()
{
	MSG  msg;
	while( PeekMessage( &msg, NULL, 0U, 0U, PM_REMOVE ) )
	{
		TranslateMessage( &msg );
		DispatchMessage( &msg );
	}
}

#endif

#ifdef MAC_OSX

#include <Carbon/Carbon.h>

extern "C" _AnomalousExport void WindowFunctions_pumpMessages()
{
	// OSX Message Pump
	EventRef event = NULL;
	EventTargetRef targetWindow;
	targetWindow = GetEventDispatcherTarget();
	
	// If we are unable to get the target then we no longer care about events.
	if( targetWindow )
	{
		// Grab the next event, process it if it is a window event
		if( ReceiveNextEvent( 0, NULL, kEventDurationNoWait, true, &event ) == noErr )
		{
			// Dispatch the event
			SendEventToEventTarget( event, targetWindow );
			ReleaseEvent( event );
		}
	}
}

#endif