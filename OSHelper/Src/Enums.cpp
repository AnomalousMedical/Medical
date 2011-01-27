#include "Stdafx.h"
#include "Enums.h"

NativeDialogResult interpretResults(int resultCode)
{
	switch(resultCode)
	{
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