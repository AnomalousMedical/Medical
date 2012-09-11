#include "StdAfx.h"
#include "CocoaIdleApplicationDelegate.h"
#include "CocoaApp.h"

@implementation CocoaIdleApplicationDelegate

-(id) initWithApp:(CocoaIdleApplication*) theApp andCocoaApp:(CocoaApp*) theCocoaApp
{
    self = [super init];
    app = theApp;
    cocoaApp = theCocoaApp;
    return self;
}

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    if(cocoaApp->fireInit())
    {
        [app startIdleCallbacks];
    }
    else
    {
        [app doStopApplication];
    }
}

- (NSApplicationTerminateReply)applicationShouldTerminate:(NSApplication *)sender
{
    //We need the stack to unwind, stop termination and run our stop
    [app doStopApplication];
    return NO;
}

@end
