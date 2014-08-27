//
//  IdleCallbackApplication.c
//  OSHelper
//
//  Created by AndrewPiper on 8/31/12.
//
//

#include "StdAfx.h"
#include "CocoaIdleApplication.h"
#include "CocoaApp.h"

enum EventSubtypes
{
    UNDEFINED_EVENT_SUBTYPE = 0,
    EXIT_EVENT_SUBTYPE = 1,
    IDLE_EVENT_SUBTYPE = 2
};

@implementation CocoaIdleApplication

+(id)sharedApplication
{
    return [super sharedApplication];
}

-(void)setApp: (CocoaApp*)app
{
    cocoaApp = app;
}

- (void)run
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    NSEvent *event;
    
    [self finishLaunching];
    
    shouldKeepRunning = cocoaApp->fireInit();
    
    while(shouldKeepRunning)
    {
        [pool release];
        pool = [[NSAutoreleasePool alloc] init];
        
        //Grab an event, if one is found process it otherwise tell the program to idle.
        event = [self nextEventMatchingMask:NSAnyEventMask untilDate:[NSDate distantPast] inMode:NSDefaultRunLoopMode dequeue:YES];
        if(event != nil)
        {
            [self sendEvent:event];
        }
        else
        {
            cocoaApp->fireIdle();
        }
        
        [self updateWindows];
        
    }
    
    cocoaApp->fireExit();
    
    [pool release];
}

-(void)doStopApplication
{
    shouldKeepRunning = NO;
}

@end