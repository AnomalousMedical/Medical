#include "StdAfx.h"
#include "..\Include\HtmlHelpController.h"

HtmlHelpController::HtmlHelpController(void)
{
}

HtmlHelpController::~HtmlHelpController(void)
{
}

void HtmlHelpController::AddBook(String path)
{
	helpController.AddBook(wxString::FromAscii(path));
}

void HtmlHelpController::Display(int index)
{
	helpController.Display(index);
}

//PInvoke
extern "C" _AnomalousExport HtmlHelpController* HtmlHelpController_new(void)
{
	return new HtmlHelpController();
}

extern "C" _AnomalousExport void HtmlHelpController_delete(HtmlHelpController* controller)
{
	delete controller;
}

extern "C" _AnomalousExport void HtmlHelpController_AddBook(HtmlHelpController* controller, String path)
{
	controller->AddBook(path);
}

extern "C" _AnomalousExport void HtmlHelpController_Display(HtmlHelpController* controller, int index)
{
	controller->Display(index);
}