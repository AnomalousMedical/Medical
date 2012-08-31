//
//  IdleCallbackApplication.h
//  OSHelper
//
//  Created by AndrewPiper on 8/31/12.
//
//

#pragma once

#import <Cocoa/Cocoa.h>

class CocoaApp;

@interface IdleCallbackApplication : NSApplication
{
    CocoaApp* cocoaApp;
}

+(id)sharedApplication;

-(void)setApp: (CocoaApp*)app;

-(void)startIdleCallbacks;

-(void)doStopApplication;
@end