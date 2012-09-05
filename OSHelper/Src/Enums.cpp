#include "Stdafx.h"
#include "Enums.h"

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