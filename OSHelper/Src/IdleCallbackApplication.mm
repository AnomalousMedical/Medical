//
//  IdleCallbackApplication.c
//  OSHelper
//
//  Created by AndrewPiper on 8/31/12.
//
//

#include "StdAfx.h"
#include "IdleCallbackApplication.h"
#include "CocoaApp.h"

enum EventSubtypes
{
    UNDEFINED_EVENT_SUBTYPE = 0,
    EXIT_EVENT_SUBTYPE = 1,
    IDLE_EVENT_SUBTYPE = 2
};

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
}

@end