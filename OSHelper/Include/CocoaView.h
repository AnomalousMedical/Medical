//
//  CocoaView.h
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#include "KeyboardButtonCode.h"

#define KEY_CONVERTER_MAX 127

class CocoaWindow;
class MultiTouch;

@interface CocoaView : NSView
{
@private
    NSTrackingArea* trackingArea;
    CocoaWindow* cocoaWindow;
    KeyboardButtonCode keyConverter[KEY_CONVERTER_MAX];
    MultiTouch* multiTouch;
}

-(id) initWithFrame:(NSRect)frame andWindow:(CocoaWindow*) win;

-(void)fireMouseDown: (int)buttonCode;

-(void)fireMouseUp: (int)buttonCode;

-(void)fireMouseWheel: (float)deltaX deltaY:(float)deltaY deltaZ:(float)deltaZ;

-(void)fireMouseMoved: (int)x y:(int)y;

-(void)fireKeyUp: (int)keyCode character:(NSString*) character;

-(void)fireKeyDown: (int)keyCode character:(NSString*) character;

-(KeyboardButtonCode)translateToKeyCode: (int)keyCode;

-(uint)getCharacterId: (NSString*)characters;

-(void)buildKeyConverter;

-(void)setupMultitouch: (MultiTouch*) touch;

@end
