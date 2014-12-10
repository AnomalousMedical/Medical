//
//  CocoaView.m
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#include "StdAfx.h"
#import "CocoaView.h"
#include "CocoaWindow.h"
#include "MultiTouch.h"

@implementation CocoaView

- (id)initWithFrame:(NSRect)frame andWindow:(CocoaWindow *)win
{
    self = [super initWithFrame:frame];
    if (self)
    {
        cocoaWindow = win;
        multiTouch = 0;
        [self updateTrackingAreas];
        [self buildKeyConverter];
    }
    
    return self;
}

- (void)dealloc
{
    if(trackingArea)
    {
        [trackingArea release];
    }
    [super dealloc];
}

//Cursor
-(void)resetCursorRects
{
    [self addCursorRect:[self visibleRect] cursor:cocoaWindow->getCursor()];
}

//Multi Touch
-(void)setupMultitouch: (MultiTouch*) touch
{
    multiTouch = touch;
    [self setAcceptsTouchEvents:true];
}

- (void)touchesBeganWithEvent:(NSEvent *)event
{
    TouchInfo touchInfo;
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseTouching inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		touchInfo.normalizedX = point.x;
		touchInfo.normalizedY = 1.0 - point.y;
		touchInfo.id = (int)touch.identity;
		multiTouch->fireTouchStarted(touchInfo);
	}
}

- (void)touchesMovedWithEvent:(NSEvent *)event
{
    TouchInfo touchInfo;
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseTouching inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		touchInfo.normalizedX = point.x;
		touchInfo.normalizedY = 1.0 - point.y;
		touchInfo.id = (int)touch.identity;
		multiTouch->fireTouchMoved(touchInfo);
	}
}

- (void)touchesEndedWithEvent:(NSEvent *)event
{
    TouchInfo touchInfo;
	NSSet *touches = [event touchesMatchingPhase:NSTouchPhaseEnded inView:self];
	for(NSTouch *touch in touches)
	{
		NSPoint point = touch.normalizedPosition;
		touchInfo.normalizedX = point.x;
		touchInfo.normalizedY = 1.0 - point.y;
		touchInfo.id = (int)touch.identity;
		multiTouch->fireTouchEnded(touchInfo);
	}
}

- (void)touchesCancelledWithEvent:(NSEvent *)event
{
	multiTouch->fireAllTouchesCanceled();
}

//Mouse
-(void)updateTrackingAreas
{
    if(trackingArea != nil)
    {
        [self removeTrackingArea:trackingArea];
        [trackingArea release];
    }
    int opts = (NSTrackingMouseMoved | NSTrackingActiveAlways);
    trackingArea = [[NSTrackingArea alloc] initWithRect:[self convertRectToBacking:[self bounds]] options: opts owner:self userInfo:nil];
    [self addTrackingArea:trackingArea];
}

-(void)mouseMoved:(NSEvent *)theEvent
{
    NSPoint mouseLoc = [self convertPointToBacking:[theEvent locationInWindow]];
    [self fireMouseMoved:mouseLoc.x y:mouseLoc.y];
}

-(void)mouseDragged:(NSEvent *)theEvent
{
    NSPoint mouseLoc = [self convertPointToBacking:[theEvent locationInWindow]];
    [self fireMouseMoved:mouseLoc.x y:mouseLoc.y];
}

-(void)rightMouseDragged:(NSEvent *)theEvent
{
    NSPoint mouseLoc = [self convertPointToBacking:[theEvent locationInWindow]];
    [self fireMouseMoved:mouseLoc.x y:mouseLoc.y];
}

-(void)otherMouseDragged:(NSEvent *)theEvent
{
    NSPoint mouseLoc = [self convertPointToBacking:[theEvent locationInWindow]];
    [self fireMouseMoved:mouseLoc.x y:mouseLoc.y];
}

- (void)mouseDown:(NSEvent *)theEvent
{
    [self fireMouseDown:0];
}

- (void)mouseUp:(NSEvent *)theEvent
{
    [self fireMouseUp:0];
}

-(void)rightMouseDown:(NSEvent *)theEvent
{
    [self fireMouseDown:1];
}

-(void)rightMouseUp:(NSEvent *)theEvent
{
    [self fireMouseUp:1];
}

-(void)otherMouseDown:(NSEvent *)theEvent
{
    [self fireMouseDown:theEvent.buttonNumber];
}

-(void)otherMouseUp:(NSEvent *)theEvent
{
    [self fireMouseUp:theEvent.buttonNumber];
}

- (void)scrollWheel:(NSEvent *)theEvent
{
    [self fireMouseWheel:theEvent.deltaX deltaY:theEvent.deltaY deltaZ:theEvent.deltaZ];
}

-(void)fireMouseDown: (int)buttonCode
{
    cocoaWindow->fireMouseButtonDown((MouseButtonCode)buttonCode);
}

-(void)fireMouseUp: (int)buttonCode
{
    cocoaWindow->fireMouseButtonUp((MouseButtonCode)buttonCode);
}

-(void)fireMouseWheel: (float)deltaX deltaY:(float)deltaY deltaZ:(float)deltaZ
{
    cocoaWindow->fireMouseWheel((int)deltaY);
}

-(void)fireMouseMoved: (int)x y:(int)y
{
    NSRect areaRect = [trackingArea rect];
    cocoaWindow->fireMouseMove(x, areaRect.size.height - y);
}

//Keyboard
- (BOOL)acceptsFirstResponder
{
    return YES;
}

- (void)keyDown:(NSEvent *)theEvent
{
    [self fireKeyDown:theEvent.keyCode character:theEvent.characters ];
}

- (void)keyUp:(NSEvent *)theEvent
{
    [self fireKeyUp:theEvent.keyCode character:theEvent.characters];
}

- (void)flagsChanged:(NSEvent *)theEvent
{
    NSUInteger modifierFlag = theEvent.modifierFlags;
    int keyCode = theEvent.keyCode;
    switch(keyCode)
    {
        case 54: //Right Command
            if((modifierFlag & NSCommandKeyMask) > 0)
            {
                [self fireKeyDown: keyCode character:nil];
            }
            else
            {
                [self fireKeyUp: keyCode character:nil];
            }
            break;
        case 55: //Left Command
            if((modifierFlag & NSCommandKeyMask) > 0)
            {
                [self fireKeyDown: keyCode character:nil];
            }
            else
            {
                [self fireKeyUp: keyCode character:nil];
            }
            break;
        case 56: //Left Shift
            if((modifierFlag & NSShiftKeyMask) > 0)
            {
                [self fireKeyDown: keyCode character:nil];
            }
            else
            {
                [self fireKeyUp: keyCode character:nil];
            }
            break;
        case 57: //Caps Lock
            if((modifierFlag & NSAlphaShiftKeyMask) > 0)
            {
                [self fireKeyDown: keyCode character:nil];
            }
            else
            {
                [self fireKeyUp: keyCode character:nil];
            }
            break;
        case 58: //Left Alt
            if((modifierFlag & NSAlternateKeyMask) > 0)
            {
                [self fireKeyDown: keyCode character:nil];
            }
            else
            {
                [self fireKeyUp: keyCode character:nil];
            }
            break;
        case 59: //Left Control
            if((modifierFlag & NSControlKeyMask) > 0)
            {
                [self fireKeyDown: keyCode character:nil];
            }
            else
            {
                [self fireKeyUp: keyCode character:nil];
            }
            break;
        case 60: //Right Shift
            if((modifierFlag & NSShiftKeyMask) > 0)
            {
                [self fireKeyDown: keyCode character:nil];
            }
            else
            {
                [self fireKeyUp: keyCode character:nil];
            }
            break;
        case 61: //Right Alt
            if((modifierFlag & NSAlternateKeyMask) > 0)
            {
                [self fireKeyDown: keyCode character:nil];
            }
            else
            {
                [self fireKeyUp: keyCode character:nil];
            }
            break;
        case 63: //Left Fn
            if((modifierFlag & NSFunctionKeyMask) > 0)
            {
                [self fireKeyDown: keyCode character:nil];
            }
            else
            {
                [self fireKeyUp: keyCode character:nil];
            }
            break;
        default:
            break;
    }
}

-(void)fireKeyUp: (int)keyCode character:(NSString*) character
{
    cocoaWindow->fireKeyUp([self translateToKeyCode:keyCode]);
}

-(void)fireKeyDown: (int)keyCode character:(NSString*) character
{
    cocoaWindow->fireKeyDown([self translateToKeyCode:keyCode], [self getCharacterId:character]);
}

-(KeyboardButtonCode)translateToKeyCode: (int)keyCode
{
    if(keyCode < KEY_CONVERTER_MAX)
    {
        return keyConverter[keyCode];
    }
    return KC_UNASSIGNED;
}

-(uint)getCharacterId: (NSString*)characters
{
    if(characters != nil && characters.length > 0)
    {
        return [characters characterAtIndex:0];
    }
    return 0;
}

-(void)buildKeyConverter
{
    for(int i = 0; i < KEY_CONVERTER_MAX; ++i)
    {
        keyConverter[i] = KC_UNASSIGNED;
    }
    
 //Function Row
 keyConverter[53] = KC_ESCAPE; //Key Esc
 keyConverter[122] = KC_F1; //Key F1
 keyConverter[120] = KC_F2; //Key F2
 keyConverter[99] = KC_F3; //Key F3
 keyConverter[118] = KC_F4; //Key F4
 keyConverter[96] = KC_F5; //Key F5
 keyConverter[97] = KC_F6; //Key F6
 keyConverter[98] = KC_F7; //Key F7
 keyConverter[100] = KC_F8; //Key F8
 keyConverter[101] = KC_F9; //Key F9
 keyConverter[109] = KC_F10; //Key F10
 
 //Number Row
 keyConverter[50] = KC_GRAVE; //Key `
 keyConverter[18] = KC_1; //Key 1
 keyConverter[19] = KC_2; //Key 2
 keyConverter[20] = KC_3; //Key 3
 keyConverter[21] = KC_4; //Key 4
 keyConverter[23] = KC_5; //Key 5
 keyConverter[22] = KC_6; //Key 6
 keyConverter[26] = KC_7; //Key 7
 keyConverter[28] = KC_8; //Key 8
 keyConverter[25] = KC_9; //Key 9
 keyConverter[29] = KC_0; //Key 0
 keyConverter[27] = KC_MINUS; //Key -
 keyConverter[24] = KC_EQUALS; //Key =
 keyConverter[51] = KC_BACK; //Key Backspace
 
 //Top Row
 keyConverter[48] = KC_TAB; //Key Tab
 keyConverter[12] = KC_Q; //Key q
 keyConverter[13] = KC_W; //Key w
 keyConverter[14] = KC_E; //Key e
 keyConverter[15] = KC_R; //Key r
 keyConverter[17] = KC_T; //Key t
 keyConverter[16] = KC_Y; //Key y
 keyConverter[32] = KC_U; //Key u
 keyConverter[34] = KC_I; //Key i
 keyConverter[31] = KC_O; //Key o
 keyConverter[35] = KC_P; //Key p
 keyConverter[33] = KC_LBRACKET; //Key [
 keyConverter[30] = KC_RBRACKET; //Key ]
 keyConverter[42] = KC_BACKSLASH; //Key \
 
 //Home Row
 keyConverter[0] = KC_A; //Key a
 keyConverter[1] = KC_S; //Key s
 keyConverter[2] = KC_D; //Key d
 keyConverter[3] = KC_F; //Key f
 keyConverter[5] = KC_G; //Key g
 keyConverter[4] = KC_H; //Key h
 keyConverter[38] = KC_J; //Key j
 keyConverter[40] = KC_K; //Key k
 keyConverter[37] = KC_L; //Key l
 keyConverter[41] = KC_SEMICOLON; //Key ;
 keyConverter[39] = KC_APOSTROPHE; //Key '
 keyConverter[36] = KC_RETURN; //Key
 
 //Bottom Row
 keyConverter[56] = KC_LSHIFT; //Key Left Shift
 keyConverter[6] = KC_Z; //Key z
 keyConverter[7] = KC_X; //Key x
 keyConverter[8] = KC_C; //Key c
 keyConverter[9] = KC_V; //Key v
 keyConverter[11] = KC_B; //Key b
 keyConverter[45] = KC_N; //Key n
 keyConverter[46] = KC_M; //Key m
 keyConverter[43] = KC_COMMA; //Key ,
 keyConverter[47] = KC_PERIOD; //Key .
 keyConverter[44] = KC_SLASH; //Key /
 keyConverter[56] = KC_LSHIFT; //Key Right Shift, but we send as left shift
 
 //Spacebar row (on laptop)
 //keyConverter[63] = KC_; //Key Fn
 keyConverter[55] = KC_LCONTROL; //Key left command, treat as control
 keyConverter[58] = KC_LMENU; //Key left Alt
 keyConverter[59] = KC_LWIN; //Key left control, treat as win
 keyConverter[49] = KC_SPACE; //Key Space
 keyConverter[54] = KC_RCONTROL; //Key Right Command, treat as control
 keyConverter[61] = KC_RMENU; //Key Right alt
 
 //Cursor Position Keys
 keyConverter[117] = KC_DELETE; //Key Delete
 keyConverter[115] = KC_HOME; //Key Home
 keyConverter[119] = KC_END; //Key End
 keyConverter[116] = KC_PGUP; //Key Page Up
 keyConverter[121] = KC_PGDOWN; //Key Page Down
 
 //Arrow Keys
 keyConverter[123] = KC_LEFT; //Key Left
 keyConverter[124] = KC_RIGHT; //Key Right
 keyConverter[126] = KC_UP; //Key Up
 keyConverter[125] = KC_DOWN; //Key Down
 
 //Numpad numlock off
 keyConverter[83] = KC_NUMPAD1; //Key 1
 keyConverter[84] = KC_NUMPAD2; //Key 2
 keyConverter[85] = KC_NUMPAD3; //Key 3
 keyConverter[86] = KC_NUMPAD4; //Key 4
 keyConverter[87] = KC_NUMPAD5; //Key 5
 keyConverter[88] = KC_NUMPAD6; //Key 6
 keyConverter[89] = KC_NUMPAD7; //Key 7
 keyConverter[91] = KC_NUMPAD8; //Key 8
 keyConverter[92] = KC_NUMPAD9; //Key 9
 keyConverter[82] = KC_NUMPAD0; //Key 0
 keyConverter[75] = KC_DIVIDE; //Key /
 keyConverter[67] = KC_MULTIPLY; //Key *
 keyConverter[78] = KC_SUBTRACT; //Key -
 keyConverter[69] = KC_ADD; //Key +
 keyConverter[76] = KC_NUMPADENTER; //Key Enter on numpad osx sends upside down ?
 keyConverter[65] = KC_DECIMAL; //Key .
}

@end
