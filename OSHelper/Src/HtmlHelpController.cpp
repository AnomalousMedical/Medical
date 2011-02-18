#include "StdAfx.h"
#include "HtmlHelpController.h"

HtmlHelpController::HtmlHelpController(wxWindow* parentWindow)
:helpController(wxHF_DEFAULT_STYLE, parentWindow)
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
extern "C" _AnomalousExport HtmlHelpController* HtmlHelpController_new(wxWindow* parentWindow)
{
	return new HtmlHelpController(parentWindow);
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