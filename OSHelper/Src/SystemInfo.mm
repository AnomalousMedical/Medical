#include "StdAfx.h"
#import <Cocoa/Cocoa.h>

extern "C" _AnomalousExport uint SystemInfo_getDisplayCount()
{
	NSArray *screenArray = [NSScreen screens];
    return [screenArray count];
}

extern "C" _AnomalousExport void SystemInfo_getDisplayLocation(int displayIndex, int& x, int& y)
{
    NSArray *screenArray = [NSScreen screens];
    if(displayIndex < [screenArray count])
    {
        NSScreen *screen = [screenArray objectAtIndex:displayIndex];
        NSRect screenRect = [screen visibleFrame];
        x = screenRect.origin.x;
        y = screenRect.origin.y;
    }
    else
    {
        x = 0;
        y = 0;
    }
}