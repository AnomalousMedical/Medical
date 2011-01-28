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

#include "MultiTouch.h"

#ifdef __GNUC__
#include <ext/hash_map>
#else
#include <hash_map>
#endif

namespace std
{
	using namespace __gnu_cxx;
}

std::hash_map<long, MultiTouch*> viewToMultitouch;
TouchInfo touchInfo;

@implementation NSView (TouchExtension)

- (void)touchesBeganWithEvent:(NSEvent *)event
{
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseTouching inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		touchInfo.normalizedX = point.x;
		touchInfo.normalizedY = 1.0 - point.y;
		touchInfo.id = (int)touch.identity;
		viewToMultitouch[(long)self]->fireTouchStarted(touchInfo);
	}
}

- (void)touchesMovedWithEvent:(NSEvent *)event
{
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseTouching inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		touchInfo.normalizedX = point.x;
		touchInfo.normalizedY = 1.0 - point.y;
		touchInfo.id = (int)touch.identity;
		viewToMultitouch[(long)self]->fireTouchMoved(touchInfo);
	}
}

- (void)touchesEndedWithEvent:(NSEvent *)event
{
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseEnded inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		touchInfo.normalizedX = point.x;
		touchInfo.normalizedY = 1.0 - point.y;
		touchInfo.id = (int)touch.identity;
		viewToMultitouch[(long)self]->fireTouchEnded(touchInfo);
	}
}

- (void)touchesCancelledWithEvent:(NSEvent *)event
{
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseCancelled inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		touchInfo.normalizedX = point.x;
		touchInfo.normalizedY = 1.0 - point.y;
		touchInfo.id = (int)touch.identity;
		viewToMultitouch[(long)self]->fireTouchEnded(touchInfo);
	}
}

@end

void registerWithObjectiveC(void* windowHandle, MultiTouch* multiTouch)
{
	NSView* nsView = (NSView*)windowHandle;
	[nsView setAcceptsTouchEvents:true];
	viewToMultitouch[(long)windowHandle] = multiTouch;
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