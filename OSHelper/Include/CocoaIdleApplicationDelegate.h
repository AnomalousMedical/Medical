//
//  CocoaWindowDelegate.h
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "CocoaIdleApplication.h"

@interface CocoaIdleApplicationDelegate : NSObject<NSApplicationDelegate>
{
@private
    CocoaIdleApplication* app;
}

-(id) initWithApp:(CocoaIdleApplication*) theApp;

@end