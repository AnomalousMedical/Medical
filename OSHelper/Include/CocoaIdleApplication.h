#pragma once

#import <Cocoa/Cocoa.h>

class CocoaApp;

@interface CocoaIdleApplication : NSApplication
{
    CocoaApp* cocoaApp;
    bool shouldKeepRunning;
}

+(id)sharedApplication;

-(void)setApp: (CocoaApp*)app;

-(void)doStopApplication;
@end