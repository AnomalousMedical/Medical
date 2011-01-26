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