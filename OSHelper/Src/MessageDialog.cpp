#include "StdAfx.h"
#include "Enums.h"
#include "NativeOSWindow.h"

#ifdef WINDOWS

extern "C" _AnomalousExport void MessageDialog_showErrorDialog(NativeOSWindow* parent, String msg, String cap)
{
	wxMessageBox(wxString::FromAscii(msg), wxString::FromAscii(cap), wxOK | wxICON_ERROR, parent);
}

extern "C" _AnomalousExport NativeDialogResult MessageDialog_showQuestionDialog(NativeOSWindow* parent, String msg, String cap)
{
	return interpretResults(wxMessageBox(wxString::FromAscii(msg), wxString::FromAscii(cap), wxYES_NO | wxICON_QUESTION, parent));
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