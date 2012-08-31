#pragma once

#import <Cocoa/Cocoa.h>

class CocoaApp;

@interface CocoaIdleApplication : NSApplication
{
    CocoaApp* cocoaApp;
}

+(id)sharedApplication;

-(void)setApp: (CocoaApp*)app;

-(void)startIdleCallbacks;

-(void)doStopApplication;
@end