#include "StdAfx.h"
#include "Enums.h"
#include "NativeOSWindow.h"

#ifdef WINDOWS

extern "C" _AnomalousExport void MessageDialog_showErrorDialog(NativeOSWindow* parent, String msg, String cap)
{
	HWND hWnd = NULL;
	if(parent != NULL)
	{
		hWnd = (HWND)parent->getHandle();
	}
	MessageBoxA(hWnd, msg, cap, MB_OK | MB_ICONEXCLAMATION);
}

extern "C" _AnomalousExport NativeDialogResult MessageDialog_showQuestionDialog(NativeOSWindow* parent, String msg, String cap)
{
	HWND hWnd = NULL;
	if(parent != NULL)
	{
		hWnd = (HWND)parent->getHandle();
	}
	return interpretResults(MessageBoxA(hWnd, msg, cap, MB_YESNO | MB_ICONQUESTION));
}

#endif

#ifdef MAC_OSX

#include <CoreFoundation/CoreFoundation.h>

extern "C" _AnomalousExport void MessageDialog_showErrorDialog(NativeOSWindow* parent, String msg, String cap)
{
	CFStringRef cfCap = CFStringCreateWithCString(NULL, cap, kCFStringEncodingUTF8);
	CFStringRef cfMsg = CFStringCreateWithCString(NULL, msg, kCFStringEncodingUTF8);
	
	CFOptionFlags response;
	CFUserNotificationDisplayAlert(0, kCFUserNotificationCautionAlertLevel, NULL, NULL, NULL, cfCap, cfMsg, CFSTR("Ok"), NULL, NULL, &response);
	
	CFRelease(cfCap);
	CFRelease(cfMsg);
}

extern "C" _AnomalousExport NativeDialogResult MessageDialog_showQuestionDialog(NativeOSWindow* parent, String msg, String cap)
{
	CFStringRef cfCap = CFStringCreateWithCString(NULL, cap, kCFStringEncodingUTF8);
	CFStringRef cfMsg = CFStringCreateWithCString(NULL, msg, kCFStringEncodingUTF8);
	
	CFOptionFlags response;
	CFUserNotificationDisplayAlert(0, kCFUserNotificationCautionAlertLevel, NULL, NULL, NULL, cfCap, cfMsg, CFSTR("Yes"), CFSTR("No"), NULL, &response);
	
	CFRelease(cfCap);
	CFRelease(cfMsg);
	
	switch (response) {
		case kCFUserNotificationDefaultResponse:
			return YES;
			break;
		case kCFUserNotificationAlternateResponse:
			return NO;
			break;
		default:
			return CANCEL;
			break;
	}
}

#endif