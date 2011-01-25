#pragma once

#include "WxOSWindow.h"

class MainWindow : public wxFrame
{
public:
	typedef void (*DeleteDelegate)();

	MainWindow(String caption, int width, int height, DeleteDelegate deleteCB);

	virtual ~MainWindow(void);

	WxOSWindow* getOSWindow()
	{
		return osWindow;
	}

private:
	wxWindow* mainDrawControl;
	WxOSWindow* osWindow;
	DeleteDelegate deleteCB;
};
