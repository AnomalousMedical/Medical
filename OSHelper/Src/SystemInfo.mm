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

extern "C" _AnomalousExport void SystemInfo_GetOSXVersion(int &major, int &minor, int &bugfix)
{
    // sensible default
	static int mMajor = -1;
	static int mMinor = 0;
	static int mBugfix = 0;
    
	static dispatch_once_t onceToken;
	dispatch_once(&onceToken, ^{
        NSDictionary *systemVersionPlist = [NSDictionary dictionaryWithContentsOfFile:@"/System/Library/CoreServices/SystemVersion.plist"];
        if(systemVersionPlist != nil)
        {
            NSString* versionString = [systemVersionPlist objectForKey:@"ProductVersion"];
            NSArray* versions = [versionString componentsSeparatedByString:@"."];
            check( versions.count >= 2 );
            if ( versions.count >= 1 )
            {
                mMajor = [[versions objectAtIndex:0] integerValue];
            }
            if ( versions.count >= 2 )
            {
                mMinor = [[versions objectAtIndex:1] integerValue];
            }
            if ( versions.count >= 3 )
            {
                mBugfix = [[versions objectAtIndex:2] integerValue];
            }
        }
	});
    
	major = mMajor;
	minor = mMinor;
	bugfix = mBugfix;
}