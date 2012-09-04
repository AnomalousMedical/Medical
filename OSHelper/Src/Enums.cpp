#include "Stdafx.h"
#include "Enums.h"

#ifdef USE_WXWIDGETS

NativeDialogResult interpretResults(int resultCode)
{
	switch(resultCode)
	{
		case wxOK:
			return OK;
		case wxYES:
			return YES;
		case wxNO:
			return NO;
		case wxID_OK:
			return OK;
		case wxID_YES:
			return YES;
		case wxID_NO:
			return NO;
		default:
			return CANCEL;
	}
}

int convertMenuItemID(CommonMenuItems id)
{
	switch(id)
	{
	case New:
		return wxID_NEW;
	case Open:
		return wxID_OPEN;
	case Save:
		return wxID_SAVE;
	case SaveAs:
		return wxID_SAVEAS;
	case Exit:
		return wxID_EXIT;
	case Preferences:
		return wxID_PREFERENCES;
	case Help:
		return wxID_HELP;
	case About:
		return wxID_ABOUT;
	default:
		return id;
	}
}

#endif

#ifndef USE_WXWIDGETS

#ifdef WINDOWS

NativeDialogResult interpretResults(int resultCode)
{
	switch(resultCode)
	{
		case IDOK:
			return OK;
		case IDYES:
			return YES;
		case IDNO:
			return NO;
		default:
			return CANCEL;
	}
}

#endif

#endif