#include "StdAfx.h"
#include "..\Include\ImageWindow.h"
#include "NativeOSWindow.h"
#include "ImageViewer.h"

ImageWindow::ImageWindow(NativeOSWindow* parent, String windowTitle, String imageFile, String homeDir)
:wxFrame(parent, -1, wxString::FromAscii(windowTitle), wxDefaultPosition, wxDefaultSize, wxDEFAULT_FRAME_STYLE | wxFRAME_FLOAT_ON_PARENT),
homeDir(wxString::FromAscii(homeDir)),
saveLocation("")
{
	//Menu
    wxMenuBar* menuBar = new wxMenuBar();

    wxMenu* fileMenu = new wxMenu();
    fileMenu->Append(SAVE_ID, "&Save...\tCtrl+S", "Save this image to disk.");
    exploreItem = fileMenu->Append(EXPLORE_ID, "&Explore...\tCtrl+E", "Open this image's location.");
    exploreItem->Enable(false);
    fileMenu->Append(CLOSE_ID, "&Close...\tCtrl+C", "Close this window.");

    wxMenu* imageMenu = new wxMenu();
    imageMenu->Append(RESIZE_ID, "&Resize...\tCtrl+R", "Change the scaling mode of this image.");

    menuBar->Append(fileMenu, "&File");
    menuBar->Append(imageMenu, "&Image");
	SetMenuBar(menuBar);

    wxBoxSizer* formSizer = new wxBoxSizer(wxVERTICAL);

	this->Bind(wxEVT_COMMAND_MENU_SELECTED, &ImageWindow::save, this, SAVE_ID, SAVE_ID);
	this->Bind(wxEVT_COMMAND_MENU_SELECTED, &ImageWindow::explore, this, EXPLORE_ID, EXPLORE_ID);
	this->Bind(wxEVT_COMMAND_MENU_SELECTED, &ImageWindow::menuClose, this, CLOSE_ID, CLOSE_ID);
	this->Bind(wxEVT_COMMAND_MENU_SELECTED, &ImageWindow::resizeImage, this, RESIZE_ID, RESIZE_ID);

    //Image Viewer
	imageViewer = new ImageViewer(this);
    imageViewer->setBitmap(imageFile);
    formSizer->Add(imageViewer, 1, wxEXPAND);

    SetSizer(formSizer);

    this->Layout();
    this->Show();
}

ImageWindow::~ImageWindow(void)
{
}

void ImageWindow::resizeImage(wxEvent& e)
{
	imageViewer->setScaleImage(!imageViewer->getScaleImage());
}

#if WINDOWS
#include "Shellapi.h"

void ImageWindow::explore(wxEvent& e)
{
	SHELLEXECUTEINFO ExecuteInfo;
    
    memset(&ExecuteInfo, 0, sizeof(ExecuteInfo));
    
    ExecuteInfo.cbSize       = sizeof(ExecuteInfo);
    ExecuteInfo.fMask        = 0;                
    ExecuteInfo.hwnd         = 0;                
    ExecuteInfo.lpVerb       = L"open";
    ExecuteInfo.lpFile       = L"explorer.exe";
    ExecuteInfo.lpParameters = saveLocation;
    ExecuteInfo.lpDirectory  = 0;
    ExecuteInfo.nShow        = SW_SHOW;
    ExecuteInfo.hInstApp     = 0;

	ShellExecuteEx(&ExecuteInfo);
}
#elif MAC_OSX   
void ImageWindow::explore(wxEvent& e)
{
//Process.Start("open", Path.GetDirectoryName(Path.GetFullPath(this.Title)));
}
#endif

void ImageWindow::save(wxEvent& e)
{
    wxFileDialog saveFile(this, "Choose location to save image.", homeDir, "", "JPEG(*.jpg)|*.jpg;|PNG(*.png)|*.png;|TIFF(*.tiff)|*.tiff;|BMP(*.bmp)|*.bmp;", wxFD_SAVE | wxFD_OVERWRITE_PROMPT);
    {
        if (saveFile.ShowModal() == wxCANCEL)
        {
            return;
        }
        wxBitmapType format = wxBITMAP_TYPE_JPEG;
        switch (saveFile.GetFilterIndex())
        {
            case 1:
                format = wxBITMAP_TYPE_JPEG;
                break;
            case 2:
                format = wxBITMAP_TYPE_PNG;
                break;
            case 3:
                format = wxBITMAP_TYPE_TIF;
                break;
            case 4:
                format = wxBITMAP_TYPE_BMP;
                break;
        }
		saveLocation = saveFile.GetPath();
        imageViewer->saveFile(saveLocation, format);
        SetTitle(saveLocation);
        exploreItem->Enable(true);
    }
}

void ImageWindow::menuClose(wxEvent& e)
{
    Close();
}

extern "C" _AnomalousExport ImageWindow* ImageWindow_new(NativeOSWindow* parent, String windowTitle, String imageFile, String homeDir)
{
	return new ImageWindow(parent, windowTitle, imageFile, homeDir);
}

extern "C" _AnomalousExport void ImageWindow_delete(ImageWindow* window)
{
	delete window;
}