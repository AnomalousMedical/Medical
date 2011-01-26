#pragma once

class ImageViewer : public wxScrolledWindow
{
public:
	ImageViewer(wxWindow* parent);

	virtual ~ImageViewer(void);

	void setBitmap(String path);

	void setScaleImage(bool value);

	bool getScaleImage();

	virtual void OnDraw(wxDC& dc);

	void scaleMasterImage();

	void configureScrollBars();

	void OnSize(wxEvent& evt);

	void saveFile(String path, wxBitmapType type);

private:
	wxBitmap* masterBitmap;
	wxBitmap* scaledBitmap;
	bool scaleImage;
};
