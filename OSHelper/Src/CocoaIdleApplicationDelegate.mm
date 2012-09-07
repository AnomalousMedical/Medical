#include "StdAfx.h"
#include "CocoaIdleApplicationDelegate.h"

@implementation CocoaIdleApplicationDelegate

-(id) initWithApp:(CocoaIdleApplication*) theApp
{
    self = [super init];
    app = theApp;
    return self;
}

- (NSApplicationTerminateReply)applicationShouldTerminate:(NSApplication *)sender
{
    //We need the stack to unwind, stop termination and run our stop
    [app doStopApplication];
    return NO;
}

@end
