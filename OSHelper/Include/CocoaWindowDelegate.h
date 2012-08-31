//
//  CocoaWindowDelegate.h
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#import <Cocoa/Cocoa.h>

class CocoaWindow;

@interface CocoaWindowDelegate : NSObject<NSWindowDelegate>
{
@private
    CocoaWindow* window;
}

-(id) initWithWindow:(CocoaWindow*) window;

@end
