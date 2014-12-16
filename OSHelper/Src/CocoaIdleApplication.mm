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
    
    //Make sure we get out of fullscreen mode by toggling all windows back to windowed.
    for(NSWindow* window in [self windows])
    {
        if([window styleMask] & NSFullScreenWindowMask)
        {
            [window toggleFullScreen:nil];
        }
    }
    
    cocoaApp->fireExit();
    
    [pool release];
}

- (void)sendEvent:(NSEvent *)event
{
    //OSX is such a unique snowflake that releasing another key while command is pressed
    //will not dispatch the keyup event, this makes sure that we get the event on the current key window.
    //Thanks to http://stackoverflow.com/questions/4001565/missing-keyup-events-on-meaningful-key-combinations-e-g-select-till-beginning?rq=1
    //When we have other awesome problems with command in OSX 10.78 Stuck in Traffic this will be why.
    if ([event type] == NSKeyUp && ([event modifierFlags] & NSCommandKeyMask))
    {
        [[self keyWindow] sendEvent:event];
    }
    else
    {
        [super sendEvent:event];
    }
}

-(void)doStopApplication
{
    shouldKeepRunning = NO;
}

@end