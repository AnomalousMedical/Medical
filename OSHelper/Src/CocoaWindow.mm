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

CocoaWindow::CocoaWindow(String title, int x, int y, int width, int height, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activateCB)
:NativeOSWindow(deleteCB, sizedCB, closedCB, activateCB)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSRect frame = NSMakeRect(x, y, width, height);
    NSUInteger styleMask =    NSClosableWindowMask | NSTitledWindowMask | NSMiniaturizableWindowMask | NSResizableWindowMask;
    NSRect rect = [NSWindow contentRectForFrameRect:frame styleMask:styleMask];
    window =  [[NSWindow alloc] initWithContentRect:rect styleMask:styleMask backing: NSBackingStoreBuffered    defer:false];
    [window setBackgroundColor:[NSColor blackColor]];
    [window setTitle: [NSString stringWithUTF8String:title]];
    
    view = [[CocoaView alloc] initWithFrame:frame andWindow:this];
    //[view setAutoresizingMask: NSViewWidthSizable | NSViewHeightSizable];
    
    [window setContentView:view];
    
    winDelegate = [[CocoaWindowDelegate alloc] initWithWindow:this];
    [window setDelegate:winDelegate];
    [window setReleasedWhenClosed:false];
    
    [pool release];
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
    [window setTitle: [NSString stringWithUTF8String:title]];
}

void CocoaWindow::showFullScreen()
{
    //NOT IMPLEMENTED
    //Only 10.7+
    //[window toggleFullScreen:nil];
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
    NSSize size = [[window contentView] frame].size;
    return size.width;
}

int CocoaWindow::getHeight()
{
    NSSize size = [[window contentView] frame].size;
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
    //NOT IMPLEMENTED
    //will be trickier, but window.mm in wxwidgets has an implementation that should work
}

//createMenu()

//PInvoke
extern "C" _AnomalousExport NativeOSWindow* NativeOSWindow_create(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, NativeOSWindow::DeleteDelegate deleteCB, NativeOSWindow::SizedDelegate sizedCB, NativeOSWindow::ClosedDelegate closedCB, NativeOSWindow::ActivateDelegate activateCB)
{
	return new CocoaWindow(caption, x, y, width, height, deleteCB, sizedCB, closedCB, activateCB);
}