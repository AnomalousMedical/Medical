#include "StdAfx.h"
#include "FileOpenDialog.h"
#include "FileSaveDialog.h"
#include "DirDialog.h"
#include "ColorDialog.h"
#include "NativeOSWindow.h"

#include <Cocoa/Cocoa.h>

//Wildcard, these are in the format description|extension|description|extension
void convertWildcards(const std::string& wildcard, std::string& filterBuffer)
{
	size_t pos = 0;
	pos = wildcard.find('|');
	if(pos == std::string::npos)
	{
		//Consider the whole string the filter
		filterBuffer = wildcard + '\0' + wildcard + '\0';
	}
	else
	{
		int pipeCount = 0;
		filterBuffer.assign(wildcard);
		do
		{
			++pipeCount;
			filterBuffer[pos] = '\0';
			pos = wildcard.find('|', pos + 1);
		}
		while(pos != std::string::npos);
		//If the last character was not a pipe make sure to add a trailing null
		if(wildcard.rfind('|') + 1 != wildcard.length())
		{
			filterBuffer += '\0';
			++pipeCount;
		}
		//Make sure we have an even set of pairs
		if(pipeCount % 2 != 0)
		{
			filterBuffer += "*.*";
			filterBuffer += '\0';
		}
	}
}

NativeDialogResult FileOpenDialog::showModal()
{
    NSOpenPanel *oPanel = [NSOpenPanel openPanel];
    
    [oPanel setAllowsMultipleSelection:selectMultiple];
    [oPanel setCanChooseDirectories: NO];
    [oPanel setCanChooseFiles: YES];
    
    if(parent != 0)
    {
        NSView* view = (NSView*)parent->getHandle();
        NSWindow* window = [view window];
        [oPanel setParentWindow: window];
    }
    
    if([oPanel runModal] == NSOKButton)
    {
        for(NSURL *url in [oPanel URLs])
        {
            NSURL *file = [url filePathURL];
            NSString* absoluteFile = [file path];
            paths.push_back([absoluteFile cStringUsingEncoding:NSASCIIStringEncoding]);
        }
        return OK;
    }
	return CANCEL;
}

NativeDialogResult FileSaveDialog::showModal()
{
    //Use NSSavePanel=======
    return CANCEL;
}

NativeDialogResult DirDialog::showModal()
{
    //Use NSOpenPanel with setCanChooseDirectories on and canChooseFiles off=======
	return CANCEL;
}

NativeDialogResult ColorDialog::showModal()
{
    //Use NSColorPanel=======
	return CANCEL;
}