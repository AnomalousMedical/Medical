//
//  CocoaWindow.h
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#ifndef __TestOpenGL__CocoaWindow__
#define __TestOpenGL__CocoaWindow__

#include <Cocoa/Cocoa.h>
#include "NativeOSWindow.h"

@class CocoaView;
@class CocoaWindowDelegate;

class CocoaWindow : public NativeOSWindow
{
public:
    CocoaWindow(CocoaWindow* parent, String title, int x, int y, int width, int height, bool floatOnParent, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosingDelegate closingCB, ClosedDelegate closedCB, ActivateDelegate activateCB);
    
    virtual ~CocoaWindow();
    
    virtual void setTitle(String title);
    
    virtual void setSize(int width, int height);
    
    virtual int getWidth();
    
    virtual int getHeight();
    
    virtual void* getHandle();
    
    virtual void show();
    
    virtual void close();
    
    virtual void setMaximized(bool maximized);
    
    virtual bool getMaximized();
    
    virtual void setCursor(CursorType cursor);
    
    NSCursor* getCursor()
    {
        return currentCursor;
    }
    
    virtual void setupMultitouch(MultiTouch* multiTouch);
    
    virtual float getWindowScaling();
    
    virtual void toggleFullscreen();
    
private:
    NSWindow* window;
    CocoaView* view;
    CocoaWindowDelegate* winDelegate;
    NSCursor* currentCursor;
};

#endif /* defined(__TestOpenGL__CocoaWindow__) */
