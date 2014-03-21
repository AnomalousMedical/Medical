//
//  CocoaWindowDelegate.m
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#include "StdAfx.h"
#import "CocoaWindowDelegate.h"
#include "CocoaWindow.h"

@implementation CocoaWindowDelegate

-(id)initWithWindow:(CocoaWindow*) win
{
    self = [super init];
    window = win;
    return self;
}

//- (void)windowDidMove:(NSNotification *)notification
//{
    //NSLog(@"Window Moved");
//}

- (void)windowDidResize:(NSNotification *)notification
{
    window->fireSized();
}

- (void)windowWillClose:(NSNotification *)notification
{
    window->fireClosing();
    window->fireClosed();
}

- (void)windowDidBecomeKey:(NSNotification *)notification
{
    window->fireActivate(true);
}

- (void)windowDidResignKey:(NSNotification *)notification
{
    window->fireActivate(false);
}

@end
