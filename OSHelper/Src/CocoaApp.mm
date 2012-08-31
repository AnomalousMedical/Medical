//
//  CocoaApp.cpp
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#include "CocoaApp.h"

enum EventSubtypes
{
    UNDEFINED_EVENT_SUBTYPE = 0,
    EXIT_EVENT_SUBTYPE = 1,
    IDLE_EVENT_SUBTYPE = 2
};

CocoaApp::CocoaApp()
{
    app = [IdleCallbackApplication sharedApplication];
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
    }
}

void CocoaApp::exit()
{
    [app doStopApplication];
}

@implementation IdleCallbackApplication

+(id)sharedApplication
{
    return [super sharedApplication];
}

-(void)setApp: (CocoaApp*)app
{
    cocoaApp = app;
}

-(void)sendEvent:(NSEvent*) event
{
    if([event type] == NSApplicationDefined && event.subtype == IDLE_EVENT_SUBTYPE)
    {
        if(cocoaApp->fireIdle())
        {
            [self postEvent:[NSEvent otherEventWithType:NSApplicationDefined location:NSZeroPoint modifierFlags:NSAnyEventMask timestamp:0 windowNumber:0 context:nil subtype:IDLE_EVENT_SUBTYPE data1:0 data2:0] atStart:NO];
        }
        else
        {
            [self doStopApplication];
        }
    }
    else
    {
        [super sendEvent:event];
    }
}

-(void)startIdleCallbacks
{
    [self postEvent:[NSEvent otherEventWithType:NSApplicationDefined location:NSZeroPoint modifierFlags:NSAnyEventMask timestamp:0 windowNumber:0 context:nil subtype:IDLE_EVENT_SUBTYPE data1:0 data2:0] atStart:NO];
}

-(void)doStopApplication
{
    [self stop:nil];
    [self postEvent:[NSEvent otherEventWithType:NSApplicationDefined location:NSZeroPoint modifierFlags:NSAnyEventMask timestamp:0 windowNumber:0 context:nil subtype:EXIT_EVENT_SUBTYPE data1:0 data2:0] atStart:NO];
    cocoaApp->fireExit();
}

@end

//PInvoke

extern "C" _AnomalousExport CocoaApp* App_create()
{
	return new CocoaApp();
}