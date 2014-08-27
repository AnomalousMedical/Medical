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
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    app = [CocoaIdleApplication sharedApplication];
    [app setApp:this];
    
    appDelegate = [[CocoaIdleApplicationDelegate alloc] initWithApp:app andCocoaApp:this];
    [app setDelegate:appDelegate];
    
    id menubar = [[NSMenu new] autorelease];
    id appMenuItem = [[NSMenuItem new] autorelease];
    [menubar addItem:appMenuItem];
    [app setMainMenu:menubar];
    id appMenu = [[NSMenu new] autorelease];
    id appName = @"Anomalous Medical";//[[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleName"];
    id quitTitle = [@"Quit " stringByAppendingString:appName];
    id quitMenuItem = [[[NSMenuItem alloc] initWithTitle:quitTitle action:@selector(terminate:) keyEquivalent:@"q"] autorelease];
    [appMenu addItem:quitMenuItem];
    [appMenuItem setSubmenu:appMenu];
    
    [pool drain];
}

CocoaApp::~CocoaApp()
{
    if(app)
    {
        [app release];
        app = nil;
    }
    if(appDelegate)
    {
        [appDelegate release];
        appDelegate = nil;
    }
}

void CocoaApp::run()
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    [app run];
    
    [pool drain];
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