//
//  OSXMultiTouch.mm
//  OSHelper
//
//  Created by Andrew Piper on 1/27/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "OSXMultiTouch.h"
#import <objc/objc-class.h>

#include <iostream>


@implementation NSView (TouchExtension)

- (void)touchesBeganWithEvent:(NSEvent *)event
{
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseTouching inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		point.y = 1.0 - point.y;
		std::cout << "Objective C touch begin " << touch.identity << " x " << point.x << " y " << point.y << std::endl;
	}
}

- (void)touchesMovedWithEvent:(NSEvent *)event
{
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseTouching inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		point.y = 1.0 - point.y;
		std::cout << "Objective C touch moved " << touch.identity << " x " << point.x << " y " << point.y << std::endl;
	}
}

- (void)touchesEndedWithEvent:(NSEvent *)event
{
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseEnded inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		point.y = 1.0 - point.y;
		std::cout << "Objective C touch ended " << touch.identity << " x " << point.x << " y " << point.y << std::endl;
	}
}

- (void)touchesCancelledWithEvent:(NSEvent *)event
{
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseCancelled inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		point.y = 1.0 - point.y;
		std::cout << "Objective C touch canceled " << touch.identity << " x " << point.x << " y " << point.y << std::endl;
	}
}

@end

void registerWithObjectiveC(void* windowHandle)
{
	NSView* nsView = (NSView*)windowHandle;
	[nsView setAcceptsTouchEvents:true];
	bool touchActivated = [nsView acceptsTouchEvents] != 0;
	std::cout << "Objective C got " << windowHandle << " if true touch events activated " << touchActivated << std::endl;
}



/*@implementation NSView (GestureExtenstion)
 
 - (void)magnifyWithEvent:(NSEvent *)event 
 {
 std::cout << "Objective C Magnify" << std::endl;
 }
 
 - (void)rotateWithEvent:(NSEvent *)event
 {
 std::cout << "Objective C Rotate" << std::endl;
 }
 
 - (void)swipeWithEvent:(NSEvent *)event 
 {
 std::cout << "Objective C Swipe" << std::endl;
 }
 
 @end
 
 void registerWithObjectiveC(void* windowHandle)
 {
 NSView* nsView = (NSView*)windowHandle;
 bool touchActivated = [nsView acceptsTouchEvents] != 0;
 std::cout << "Objective C got " << windowHandle << " if true touch events activated " << touchActivated << std::endl;
 }*/