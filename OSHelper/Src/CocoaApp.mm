//
//  CocoaApp.cpp
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#include "CocoaApp.h"

CocoaApp::CocoaApp()
{
    app = [CocoaIdleApplication sharedApplication];
    [app setApp:this];
}

CocoaApp::~CocoaApp()
{
    if(app)
    {
        [app release];
        app = nil;
    }
}

void CocoaApp::run()
{
    if(this->fireInit())
    {
        [app startIdleCallbacks];
        [app run];
        fireExit();
    }
}

void CocoaApp::exit()
{
    [app doStopApplication];
}

//PInvoke

extern "C" _AnomalousExport CocoaApp* App_create()
{
	return new CocoaApp();
}

#include <Carbon/Carbon.h>

extern "C" _AnomalousExport void App_pumpMessages()
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