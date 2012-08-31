#include "StdAfx.h"
#include "NativeOSWindow.h"

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

#ifdef USE_WXWIDGETS

wxCursor windowCursors[] = 
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

NativeOSWindow::NativeOSWindow(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activateCB)
:wxFrame(parent, -1, wxString::FromAscii(caption), wxPoint(x, y), wxSize(width, height), floatOnParent ? wxDEFAULT_FRAME_STYLE | wxFRAME_FLOAT_ON_PARENT : wxDEFAULT_FRAME_STYLE),
deleteCB(deleteCB),
sizedCB(sizedCB),
closedCB(closedCB),
activateCB(activateCB)
{
	SetBackgroundColour(wxColour(0, 0, 0));

	Bind(wxEVT_SIZE, &NativeOSWindow::OnSize, this);
	Bind(wxEVT_CLOSE_WINDOW, &NativeOSWindow::OnClose, this);
	Bind(wxEVT_ACTIVATE, &NativeOSWindow::OnActivate, this);
}

NativeOSWindow::~NativeOSWindow(void)
{
	deleteCB();
}

extern "C" _AnomalousExport NativeOSWindow* NativeOSWindow_create(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, NativeOSWindow::DeleteDelegate deleteCB, NativeOSWindow::SizedDelegate sizedCB, NativeOSWindow::ClosedDelegate closedCB, NativeOSWindow::ActivateDelegate activateCB)
{
	return new NativeOSWindow(parent, caption, x, y, width, height, floatOnParent, deleteCB, sizedCB, closedCB, activateCB);
}

extern "C" _AnomalousExport void NativeOSWindow_destroy(NativeOSWindow* nativeWindow)
{
	delete nativeWindow;
}

extern "C" _AnomalousExport void NativeOSWindow_setTitle(NativeOSWindow* nativeWindow, String title)
{
	nativeWindow->SetLabel(wxString::FromAscii(title));
}

extern "C" _AnomalousExport void NativeOSWindow_showFullScreen(NativeOSWindow* nativeWindow)
{
    nativeWindow->ShowFullScreen(true, wxFULLSCREEN_ALL);
}

extern "C" _AnomalousExport void NativeOSWindow_setSize(NativeOSWindow* nativeWindow, int width, int height)
{
    nativeWindow->SetSize(wxSize(width, height));
}

extern "C" _AnomalousExport int NativeOSWindow_getWidth(NativeOSWindow* nativeWindow)
{
    int w, h;
	nativeWindow->GetClientSize(&w, &h);
	return w;
}

extern "C" _AnomalousExport int NativeOSWindow_getHeight(NativeOSWindow* nativeWindow)
{
    int w, h;
	nativeWindow->GetClientSize(&w, &h);
	return h;
}

extern "C" _AnomalousExport WXWidget NativeOSWindow_getHandle(NativeOSWindow* nativeWindow)
{
    return nativeWindow->GetHandle();
}


extern "C" _AnomalousExport void NativeOSWindow_show(NativeOSWindow* nativeWindow)
{
    nativeWindow->Show(true);
}

extern "C" _AnomalousExport void NativeOSWindow_close(NativeOSWindow* nativeWindow)
{
    nativeWindow->Close();
}

extern "C" _AnomalousExport void NativeOSWindow_setMaximized(NativeOSWindow* nativeWindow, bool maximize)
{
	nativeWindow->Maximize(maximize);
}

extern "C" _AnomalousExport bool NativeOSWindow_getMaximized(NativeOSWindow* nativeWindow)
{
	return nativeWindow->IsMaximized();
}

extern "C" _AnomalousExport void NativeOSWindow_setCursor(NativeOSWindow* nativeWindow, CursorType cursor)
{
	nativeWindow->SetCursor(windowCursors[(int)cursor]);
}

extern "C" _AnomalousExport wxMenuBar* NativeOSWindow_createMenu(NativeOSWindow* nativeWindow)
{
	wxMenuBar* menuBar = new wxMenuBar();
	nativeWindow->SetMenuBar(menuBar);
	return menuBar;
}

#endif

#ifndef USE_WXWIDGETS

NativeOSWindow::NativeOSWindow(DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activateCB)
:deleteCB(deleteCB),
sizedCB(sizedCB),
closedCB(closedCB),
activateCB(activateCB),
keyDownCB(0),
keyUpCB(0),
mouseButtonDownCB(0),
mouseButtonUpCB(0),
mouseMoveCB(0),
mouseWheelCB(0)
{
    //	SetBackgroundColour(wxColour(0, 0, 0));
    
    //	Bind(wxEVT_SIZE, &NativeOSWindow::OnSize, this);
    //	Bind(wxEVT_CLOSE_WINDOW, &NativeOSWindow::OnClose, this);
    //	Bind(wxEVT_ACTIVATE, &NativeOSWindow::OnActivate, this);
}

NativeOSWindow::~NativeOSWindow(void)
{
	deleteCB();
}

//Shared Pinvoke
extern "C" _AnomalousExport void NativeOSWindow_destroy(NativeOSWindow* nativeWindow)
{
	delete nativeWindow;
}

extern "C" _AnomalousExport void NativeOSWindow_setTitle(NativeOSWindow* nativeWindow, String title)
{
	nativeWindow->setTitle(title);
}

extern "C" _AnomalousExport void NativeOSWindow_showFullScreen(NativeOSWindow* nativeWindow)
{
    nativeWindow->showFullScreen();
}

extern "C" _AnomalousExport void NativeOSWindow_setSize(NativeOSWindow* nativeWindow, int width, int height)
{
    nativeWindow->setSize(width, height);
}

extern "C" _AnomalousExport int NativeOSWindow_getWidth(NativeOSWindow* nativeWindow)
{
    return nativeWindow->getWidth();
}

extern "C" _AnomalousExport int NativeOSWindow_getHeight(NativeOSWindow* nativeWindow)
{
    return nativeWindow->getHeight();
}

extern "C" _AnomalousExport void* NativeOSWindow_getHandle(NativeOSWindow* nativeWindow)
{
    return nativeWindow->getHandle();
}

extern "C" _AnomalousExport void NativeOSWindow_show(NativeOSWindow* nativeWindow)
{
    nativeWindow->show();
}

extern "C" _AnomalousExport void NativeOSWindow_close(NativeOSWindow* nativeWindow)
{
    nativeWindow->close();
}

extern "C" _AnomalousExport void NativeOSWindow_setMaximized(NativeOSWindow* nativeWindow, bool maximize)
{
	nativeWindow->setMaximized(maximize);
}

extern "C" _AnomalousExport bool NativeOSWindow_getMaximized(NativeOSWindow* nativeWindow)
{
	return nativeWindow->getMaximized();
}

extern "C" _AnomalousExport void NativeOSWindow_setCursor(NativeOSWindow* nativeWindow, CursorType cursor)
{
	//nativeWindow->SetCursor(windowCursors[(int)cursor]);
}

extern "C" _AnomalousExport void* NativeOSWindow_createMenu(NativeOSWindow* nativeWindow)
{
	//wxMenuBar* menuBar = new wxMenuBar();
	//nativeWindow->SetMenuBar(menuBar);
	//return menuBar;
    return 0;
}

#endif