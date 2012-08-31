//
//  CocoaApp.cpp
//  TestOpenGL
//
//  Created by AndrewPiper on 8/30/12.
//  Copyright (c) 2012 AndrewPiper. All rights reserved.
//

#include "CocoaApp.h"

CocoaApp::CocoaApp()
{
    app = [CocoaIdleApplication sharedApplication];
    [app setApp:this];
}

CocoaApp::~CocoaApp()
{
    if(app)
    {
        [app release];
        app = nil;
    }
}

void CocoaApp::run()
{
    if(this->fireInit())
    {
        [app startIdleCallbacks];
        [app run];
        fireExit();
    }
}

void CocoaApp::exit()
{
    [app doStopApplication];
}

//PInvoke

extern "C" _AnomalousExport CocoaApp* App_create()
{
	return new CocoaApp();
}