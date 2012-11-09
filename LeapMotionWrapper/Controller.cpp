#include "StdAfx.h"

extern "C" _AnomalousExport Leap::Controller* Controller_Create(Leap::Listener* listener)
{
	return new Leap::Controller(listener);
}

extern "C" _AnomalousExport void Controller_Delete(Leap::Controller* controller)
{
	delete controller;
}