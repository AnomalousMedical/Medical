//
//  CocoaApp.h
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#ifndef __TestOpenGL__CocoaApp__
#define __TestOpenGL__CocoaApp__

#include <Cocoa/Cocoa.h>
#include "App.h"
#import "IdleCallbackApplication.h"

class CocoaApp : public App
{
public:
    CocoaApp();
    
    virtual ~CocoaApp();
    
    virtual void run();
    
    virtual void exit();
    
private:
    IdleCallbackApplication *app;
};

#endif /* defined(__TestOpenGL__CocoaApp__) */
