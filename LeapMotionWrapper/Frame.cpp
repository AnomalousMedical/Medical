#include "StdAfx.h"

extern "C" _AnomalousExport Int64 Frame_id(Leap::Frame* frame)
{
	return frame->id();
}

extern "C" _AnomalousExport Int64 Frame_timestamp(Leap::Frame* frame)
{
	return frame->timestamp();
}