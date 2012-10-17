#include "StdAfx.h"
#include "NativeDialog.h"
#include "NativeOSWindow.h"
#include <Cocoa/Cocoa.h>

#include <CoreFoundation/CoreFoundation.h>

extern "C" _AnomalousExport void MessageDialog_showErrorDialog(NativeOSWindow* parent, String msg, String cap)
{
	CFOptionFlags response;
	CFUserNotificationDisplayAlert(0, kCFUserNotificationCautionAlertLevel, NULL, NULL, NULL, (CFStringRef)[NSString stringWithFormat:@"%S", cap], (CFStringRef)[NSString stringWithFormat:@"%S", msg], CFSTR("Ok"), NULL, NULL, &response);
}

extern "C" _AnomalousExport NativeDialogResult MessageDialog_showQuestionDialog(NativeOSWindow* parent, String msg, String cap)
{	
	CFOptionFlags response;
	CFUserNotificationDisplayAlert(0, kCFUserNotificationCautionAlertLevel, NULL, NULL, NULL, (CFStringRef)[NSString stringWithFormat:@"%S", cap], (CFStringRef)[NSString stringWithFormat:@"%S", msg], CFSTR("Yes"), CFSTR("No"), NULL, &response);
	
	switch (response) {
		case kCFUserNotificationDefaultResponse:
            return NDYES;
			break;
		case kCFUserNotificationAlternateResponse:
            return NDNO;
			break;
		default:
			return CANCEL;
			break;
	}
}