#include "StdAfx.h"
#include "NativeOSWindow.h"
#include "NativeDialog.h"

#include <string>

#include <Cocoa/Cocoa.h>

//Process an entry for wildcards, returns true if the wildcard matches everything
bool processEntry(NSString* entry, NSMutableArray* fileTypeVector)
{
    NSUInteger dotPos = [entry rangeOfString:@"."].location;
    if(dotPos != NSNotFound)
    {
        //Consider the whole string the filter, from the . to the end
        NSString* card = [entry substringFromIndex:dotPos + 1];
        if([card isEqualToString: @"*"])
        {
            return true;
        }
        [fileTypeVector addObject:card];
    }
    return false;
}

bool processEntries(NSString* entry, NSMutableArray* fileTypeVector)
{
    NSArray* fileTypes = [entry componentsSeparatedByString:@";"];
    for(NSUInteger i = 0; i < [fileTypes count]; ++i)
    {
        if(processEntry([fileTypes objectAtIndex:i], fileTypeVector))
        {
            return true;
        }
    }
    return false;
}

//Wildcard, these are in the format description|extension|description|extension
//Will return true if the filters should be used and false if not.
bool convertWildcards(String utf16Wildcard, NSMutableArray* fileTypeVector)
{
    NSString* wildcard = [NSString stringWithFormat:@"%S", utf16Wildcard];
    if([wildcard length] == 0)
    {
        return false;
    }
    NSArray* fileTypes = [wildcard componentsSeparatedByString:@"|"];
	if([fileTypes count] == 1)
	{
        return !processEntries(wildcard, fileTypeVector);
	}
	else
	{
		for(NSUInteger i = 1; i < [fileTypes count]; i += 2)
        {
            if(processEntries([fileTypes objectAtIndex:i], fileTypeVector))
            {
                return false;
            }
        }
	}
    
    return fileTypeVector.count > 0;
}

extern "C" _AnomalousExport void FileOpenDialog_showModal(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple, FileOpenDialogSetPathString setPathString, FileOpenDialogResultCallback resultCallback)
{    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSOpenPanel *oPanel = [NSOpenPanel openPanel];
    
    [oPanel setAllowsMultipleSelection:selectMultiple];
    [oPanel setCanChooseDirectories: NO];
    [oPanel setCanChooseFiles: YES];
    
    NSWindow* parentWindow = nil;
    if(parent != 0)
    {
        NSView* view = (NSView*)parent->getHandle();
        parentWindow = [view window];
    }
    
    NSMutableArray* allowedFileTypes = [[NSMutableArray alloc] init];
    
    //Allowed file types
    if(wildcard != 0)
    {
        if(convertWildcards(wildcard, allowedFileTypes))
        {
            [oPanel setAllowedFileTypes:allowedFileTypes];
        }
    }
    
    [oPanel beginSheetModalForWindow:parentWindow completionHandler:^(NSInteger returnCode)
     {
         NativeDialogResult result = CANCEL;
         if(returnCode == NSOKButton)
         {
             for(NSURL *url in [oPanel URLs])
             {
                 NSURL *file = [url filePathURL];
                 NSString* absoluteFile = [file path];
                 String pathString = (String)[absoluteFile cStringUsingEncoding:NSUTF16StringEncoding];
                 setPathString(pathString);
             }
             result = OK;
         }
         resultCallback(result);
     }];
    
    [allowedFileTypes release];
    
    [pool release];
}

extern "C" _AnomalousExport void FileSaveDialog_showModal(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, FileSaveDialogResultCallback resultCallback)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSSavePanel *oPanel = [NSSavePanel savePanel];
    
    NSWindow* parentWindow = nil;
    if(parent != 0)
    {
        NSView* view = (NSView*)parent->getHandle();
        parentWindow = [view window];
    }
    
    NSMutableArray* allowedFileTypes = [[NSMutableArray alloc] init];
    
    //Allowed file types
    if(wildcard != 0)
    {
        if(convertWildcards(wildcard, allowedFileTypes))
        {
            [oPanel setAllowedFileTypes:allowedFileTypes];
        }
    }
    
    [oPanel beginSheetModalForWindow:parentWindow completionHandler:^(NSInteger returnCode)
     {
         String resPath;
         NativeDialogResult result = CANCEL;
         if(returnCode == NSOKButton)
         {
             NSURL *file = [oPanel URL];
             NSString* absoluteFile = [file path];
             resPath = (String)[absoluteFile cStringUsingEncoding:NSUTF16StringEncoding];
             result = OK;
         }
         resultCallback(result, resPath);
     }];
    
    [allowedFileTypes release];
    
    [pool release];
}

extern "C" _AnomalousExport void DirDialog_showModal(NativeOSWindow* parent, String message, String startPath, DirDialogResultCallback resultCallback)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSOpenPanel *oPanel = [NSOpenPanel openPanel];
    
    [oPanel setAllowsMultipleSelection:NO];
    [oPanel setCanChooseDirectories: YES];
    [oPanel setCanChooseFiles: NO];
    
    NSWindow* parentWindow = nil;
    if(parent != 0)
    {
        NSView* view = (NSView*)parent->getHandle();
        parentWindow = [view window];
    }
    
    [oPanel beginSheetModalForWindow:parentWindow completionHandler:^(NSInteger returnCode)
     {
         String resPath;
         NativeDialogResult result = CANCEL;
         if(returnCode == NSOKButton)
         {
             NSURL *file = [oPanel URL];
             NSString* absoluteFile = [file path];
             resPath = (String)[absoluteFile cStringUsingEncoding:NSUTF16StringEncoding];
             result = OK;
         }
         resultCallback(result, resPath);
     }];
    
    [pool release];
}


@interface CallbackNotification : NSObject
{
    @private
    ColorDialogResultCallback resultCallback;
}

-(id) initWithCallback: (ColorDialogResultCallback)cb;

-(void) colorPickerClosed:(NSNotification *) notification;

@end

@implementation CallbackNotification

-(id) initWithCallback: (ColorDialogResultCallback)cb
{
    self = [super init];
    resultCallback = cb;
    return self;
}

-(void) colorPickerClosed:(NSNotification *) notification
{
    NSColorPanel* cPanel = [NSColorPanel sharedColorPanel];
    NSColor* theColor = [[cPanel color] colorUsingColorSpaceName:NSCalibratedRGBColorSpace];
    
    NativeDialogResult result = OK;
    Color resColor([theColor redComponent], [theColor greenComponent], [theColor blueComponent]);
 
    [[NSNotificationCenter defaultCenter] removeObserver:self name: NSWindowWillCloseNotification object: cPanel];
    
    resultCallback(result, resColor);
    
    [self release];
}

@end
 
extern "C" _AnomalousExport void ColorDialog_showModal(NativeOSWindow* parent, Color color, ColorDialogResultCallback resultCallback)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSColorPanel* cPanel = [NSColorPanel sharedColorPanel];
    CallbackNotification* cbNotification = [[CallbackNotification alloc] initWithCallback:resultCallback];
    
    [[NSNotificationCenter defaultCenter] addObserver: cbNotification selector: @selector( colorPickerClosed: ) name: NSWindowWillCloseNotification object: cPanel];
    
    [cPanel makeKeyAndOrderFront: cPanel]; //Leak complaint, but it is released in colorPickerClosed
    
    [pool release];
}