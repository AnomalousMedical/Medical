#pragma once

class NativeOSWindow;
class ImageViewer;

class ImageWindow : public wxFrame
{
public:
	ImageWindow(NativeOSWindow* parent, String windowTitle, String imageFile, String homeDir, bool allowSaving, bool startMaximized);

	~ImageWindow(void);

private:
	ImageViewer* imageViewer;

	wxMenuItem* exploreItem;

	wxString homeDir;
	wxString saveLocation;

	void resizeImage(wxEvent& e);

    void explore(wxEvent& e);

    void save(wxEvent& e);

    void menuClose(wxEvent& e);

	static const int SAVE_ID = 1000;
	static const int EXPLORE_ID = 1001;
	static const int CLOSE_ID = 1002;
	static const int RESIZE_ID = 1003;
};
