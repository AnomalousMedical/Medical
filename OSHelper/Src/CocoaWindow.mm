//
//  CocoaWindow.cpp
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#include "StdAfx.h"
#include "CocoaWindow.h"
#include "CocoaView.h"
#include "CocoaWindowDelegate.h"

CocoaWindow::CocoaWindow(CocoaWindow* parent, String title, int x, int y, int width, int height, bool floatOnParent, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosingDelegate closingCB, ClosedDelegate closedCB, ActivateDelegate activateCB)
:NativeOSWindow(deleteCB, sizedCB, closingCB, closedCB, activateCB)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSRect frame = NSMakeRect(x, y, width, height);
    NSUInteger styleMask =    NSClosableWindowMask | NSTitledWindowMask | NSMiniaturizableWindowMask | NSResizableWindowMask;
    NSRect rect = [NSWindow contentRectForFrameRect:frame styleMask:styleMask];
    window =  [[NSWindow alloc] initWithContentRect:rect styleMask:styleMask backing: NSBackingStoreBuffered    defer:false];
    [window setBackgroundColor:[NSColor blackColor]];
    [window setTitle: [NSString stringWithFormat:@"%S", title]];
    
    if(parent != 0)
    {
        [window setParentWindow:parent->window];
        
        if(floatOnParent)
        {
            [window setLevel:NSFloatingWindowLevel];
        }
    }
    else
    {
        [window setCollectionBehavior:NSWindowCollectionBehaviorFullScreenPrimary];
    }
    
    view = [[CocoaView alloc] initWithFrame:frame andWindow:this];
    //[view setAutoresizingMask: NSViewWidthSizable | NSViewHeightSizable];
    
    [window setContentView:view];
    
    winDelegate = [[CocoaWindowDelegate alloc] initWithWindow:this];
    [window setDelegate:winDelegate];
    [window setReleasedWhenClosed:false];
    
    setCursor(Arrow);
    
    [pool drain];
}

CocoaWindow::~CocoaWindow()
{
    if(window)
    {
        [window release];
        window = nil;
    }
    if(view)
    {
        [view release];
        view = nil;
    }
    if(winDelegate)
    {
        [winDelegate release];
    }
}

void CocoaWindow::setTitle(String title)
{
    [window setTitle: [NSString stringWithFormat:@"%S", title]];
}

void CocoaWindow::setSize(int width, int height)
{
    NSRect frame = window.frame;
    frame.size.width = width;
    frame.size.height = height;
    [window setFrame:frame display:YES];
}

int CocoaWindow::getWidth()
{
    NSSize size = [window convertRectToBacking:[[window contentView] frame]].size;
    return size.width;
}

int CocoaWindow::getHeight()
{
    NSSize size = [window convertRectToBacking:[[window contentView] frame]].size;
    return size.height;
}

void* CocoaWindow::getHandle()
{
    return view;
}

void CocoaWindow::show()
{
    [window makeKeyAndOrderFront: window];
}

void CocoaWindow::close()
{
    [window close];
}

void CocoaWindow::setMaximized(bool maximized)
{
    if(maximized != [window isZoomed])
    {
        if(maximized)
        {
            [window performZoom:nil];
        }
        else
        {
            [window zoom:nil];
        }
    }
}

bool CocoaWindow::getMaximized()
{
    return [window isZoomed];
}

void CocoaWindow::setCursor(CursorType cursor)
{
    switch(cursor)
	{
		case Arrow:
			currentCursor = [NSCursor arrowCursor];
			break;
		case Beam:
			currentCursor = [NSCursor IBeamCursor];
			break;
		case SizeLeft:
			currentCursor = [NSCursor resizeLeftCursor];
			break;
		case SizeRight:
			currentCursor = [NSCursor resizeRightCursor];
			break;
		case SizeHorz:
			currentCursor = [NSCursor resizeLeftRightCursor];
			break;
		case SizeVert:
			currentCursor = [NSCursor resizeUpDownCursor];
			break;
		case Hand:
			currentCursor = [NSCursor openHandCursor];
			break;
		case Link:
			currentCursor = [NSCursor pointingHandCursor];
			break;
		default:
			currentCursor = [NSCursor arrowCursor];
			break;
	}
    
    [view addCursorRect:[view visibleRect] cursor:currentCursor];
    [window invalidateCursorRectsForView:view];
}

float CocoaWindow::getWindowScaling()
{
    return [window backingScaleFactor];
}

void CocoaWindow::setupMultitouch(MultiTouch *multiTouch)
{
    [view setupMultitouch:multiTouch];
}

void CocoaWindow::toggleFullscreen()
{
    [window toggleFullScreen:nil];
}

//PInvoke
extern "C" _AnomalousExport NativeOSWindow* NativeOSWindow_create(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, NativeOSWindow::DeleteDelegate deleteCB, NativeOSWindow::SizedDelegate sizedCB, NativeOSWindow::ClosingDelegate closingCB, NativeOSWindow::ClosedDelegate closedCB, NativeOSWindow::ActivateDelegate activateCB)
{
	return new CocoaWindow(static_cast<CocoaWindow*>(parent), caption, x, y, width, height, floatOnParent, deleteCB, sizedCB, closingCB, closedCB, activateCB);
}