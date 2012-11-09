#include "StdAfx.h"

extern "C" _AnomalousExport Leap::Controller* Controller_Create(Leap::Listener* listener)
{
	return new Leap::Controller(listener);
}

extern "C" _AnomalousExport void Controller_Delete(Leap::Controller* controller)
{
	delete controller;
}

extern "C" _AnomalousExport Leap::Frame* Controller_Frame(Leap::Controller* controller)
{
	return &controller->frame();
}

extern "C" _AnomalousExport Leap::Frame* Controller_Frame_History(Leap::Controller* controller, int history)
{
	return &controller->frame(history);
}