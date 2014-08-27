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

- (NSApplicationTerminateReply)applicationShouldTerminate:(NSApplication *)sender
{
    //We need the stack to unwind, cancel termination and run our stop, this will cause the loop in IdleApplication.run to stop and will
    //unwind back up to the c# main.
    [app doStopApplication];
    return NSTerminateCancel;
}

@end
