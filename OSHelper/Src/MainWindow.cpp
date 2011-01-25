#include "StdAfx.h"
#include "..\Include\MainWindow.h"

enum CursorType
{
    Arrow = 0,
    Beam = 1,
    SizeLeft = 2,
    SizeRight = 3,
    SizeHorz = 4,
    SizeVert = 5,
    Hand = 6,
    Link = 7,
};

wxCursor cursors[] = 
{ 
	wxCursor(wxCURSOR_ARROW),
    wxCursor(wxCURSOR_IBEAM),
    wxCursor(wxCURSOR_SIZENWSE),
    wxCursor(wxCURSOR_SIZENESW),
    wxCursor(wxCURSOR_SIZEWE),
    wxCursor(wxCURSOR_SIZENS),
    wxCursor(wxCURSOR_SIZING),
    wxCursor(wxCURSOR_HAND)
};

MainWindow::MainWindow(String caption, int width, int height, DeleteDelegate deleteCB)
:wxFrame(NULL, -1, wxString::FromAscii(caption), wxPoint(-1, -1), wxSize(width, height)),
deleteCB(deleteCB)
{
	SetBackgroundColour(wxColour(0, 0, 0));

#if MAC_OSX
    //OSX needs a panel to change mouse cursors.
    Panel panel = new Panel(this);
    mainDrawControl = panel;
#else
    mainDrawControl = this;
#endif

    Center();
	osWindow = new WxOSWindow(mainDrawControl);
}

MainWindow::~MainWindow(void)
{
	deleteCB();
	delete osWindow;
}

extern "C" _AnomalousExport MainWindow* MainWindow_create(String caption, int width, int height, MainWindow::DeleteDelegate deleteCB)
{
	return new MainWindow(caption, width, height, deleteCB);
}

extern "C" _AnomalousExport void MainWindow_destroy(MainWindow* mainWindow)
{
	delete mainWindow;
}

extern "C" _AnomalousExport void MainWindow_setTitle(MainWindow* mainWindow, String title)
{
	mainWindow->SetLabel(wxString::FromAscii(title));
}

extern "C" _AnomalousExport void MainWindow_showFullScreen(MainWindow* mainWindow)
{
    mainWindow->ShowFullScreen(true, wxFULLSCREEN_ALL);
}

extern "C" _AnomalousExport void MainWindow_setSize(MainWindow* mainWindow, int width, int height)
{
    mainWindow->SetSize(wxSize(width, height));
}

extern "C" _AnomalousExport void MainWindow_show(MainWindow* mainWindow)
{
    mainWindow->Show(true);
}

extern "C" _AnomalousExport void MainWindow_close(MainWindow* mainWindow)
{
    mainWindow->Close();
}

extern "C" _AnomalousExport void MainWindow_setMaximized(MainWindow* mainWindow, bool maximize)
{
	mainWindow->Maximize(maximize);
}

extern "C" _AnomalousExport bool MainWindow_getMaximized(MainWindow* mainWindow)
{
	return mainWindow->IsMaximized();
}

extern "C" _AnomalousExport void MainWindow_setCursor(MainWindow* mainWindow, CursorType cursor)
{
	mainWindow->SetCursor(cursors[(int)cursor]);
}

extern "C" _AnomalousExport WxOSWindow* MainWindow_getOSWindow(MainWindow* mainWindow)
{
	return mainWindow->getOSWindow();
}