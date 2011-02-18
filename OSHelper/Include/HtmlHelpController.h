#pragma once

#include <wx/html/helpctrl.h>

class HtmlHelpController
{
public:
	HtmlHelpController(wxWindow* parentWindow);

	virtual ~HtmlHelpController(void);

	void AddBook(String path);

    void Display(int index);

private:
	wxHtmlHelpController helpController;
};
