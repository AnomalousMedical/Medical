//
//  CocoaWindowDelegate.h
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "CocoaIdleApplication.h"

class CocoaApp;

@interface CocoaIdleApplicationDelegate : NSObject<NSApplicationDelegate>
{
@private
    CocoaIdleApplication* app;
    CocoaApp* cocoaApp;
}

-(id) initWithApp:(CocoaIdleApplication*) theApp andCocoaApp:(CocoaApp*) theCocoaApp;

@end