#include "StdAfx.h"
#include "NativeOSWindow.h"
#include "NativeDialog.h"

#include <string>
#include <vector>

#include <Cocoa/Cocoa.h>

//Wildcard, these are in the format description|extension|description|extension
void convertWildcards(const std::string& wildcard, std::vector<std::string>& fileTypeVector)
{
	size_t pos = 0;
	pos = wildcard.find('|');
	if(pos == std::string::npos)
	{
		//Consider the whole string the filter
		fileTypeVector.push_back(wildcard);
	}
	else
	{
		int pipeCount = 0;
		do
		{
			++pipeCount;
            if(pipeCount % 2 == 0)
            {
                fileTypeVector.push_back(wildcard.substr());//filterBuffer[pos] = '\0';
            }
			pos = wildcard.find('|', pos + 1);
		}
		while(pos != std::string::npos);
		//If the last character was not a pipe make sure to add the last extension
		if(wildcard.rfind('|') + 1 != wildcard.length())
		{
			++pipeCount;
            if(pipeCount % 2 == 0)
            {
                fileTypeVector.push_back(wildcard.substr());//filterBuffer[pos] = '\0';
            }
		}
	}
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
    
    [oPanel beginSheetModalForWindow:parentWindow completionHandler:^(NSInteger returnCode)
     {
         NativeDialogResult result = CANCEL;
         if(returnCode == NSOKButton)
         {
             for(NSURL *url in [oPanel URLs])
             {
                 NSURL *file = [url filePathURL];
                 NSString* absoluteFile = [file path];
                 setPathString([absoluteFile cStringUsingEncoding:NSASCIIStringEncoding]);
             }
             result = OK;
         }
         resultCallback(result);
     }];
    
    [pool release];
}

extern "C" _AnomalousExport void FileSaveDialog_showModal(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, FileSaveDialogResultCallback resultCallback)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    //Use NSSavePanel=======
    NativeDialogResult result = CANCEL;
    std::string path;
    resultCallback(result, path.c_str());
    
    [pool release];
}

extern "C" _AnomalousExport void DirDialog_showModal(NativeOSWindow* parent, String message, String startPath, DirDialogResultCallback resultCallback)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    //Use NSOpenPanel with setCanChooseDirectories on and canChooseFiles off=======
    NativeDialogResult result = CANCEL;
    std::string path;
    resultCallback(result, path.c_str());
    
    [pool release];
}

extern "C" _AnomalousExport void ColorDialog_showModal(NativeOSWindow* parent, Color color, ColorDialogResultCallback resultCallback)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    //Use NSColorPanel=======
    NativeDialogResult result = CANCEL;
    Color resColor(0.0f, 0.0f, 0.0f);
    resultCallback(result, resColor);
    
    [pool release];
}