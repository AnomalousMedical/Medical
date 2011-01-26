#include "StdAfx.h"
#include "..\Include\ImageViewer.h"

ImageViewer::ImageViewer(wxWindow* parent)
:wxScrolledWindow(parent),
scaleImage(true),
masterBitmap(0),
scaledBitmap(0)
{
	this->Bind(wxEVT_SIZE, &ImageViewer::OnSize, this);
	SetBackgroundColour(*wxLIGHT_GREY);
}

ImageViewer::~ImageViewer(void)
{
	if(masterBitmap != 0)
	{
		delete masterBitmap;
	}
	if(scaledBitmap != 0)
	{
		delete scaledBitmap;
	}
}

void ImageViewer::setBitmap(String path)
{
	if(masterBitmap != 0)
	{
		delete masterBitmap;
	}
	if(scaledBitmap != 0)
	{
		delete scaledBitmap;
	}

	masterBitmap = new wxBitmap(path, wxBITMAP_TYPE_ANY);

	if (scaleImage)
    {
        scaleMasterImage();
    }

    configureScrollBars();

    Refresh();
}

void ImageViewer::setScaleImage(bool value)
{
	if (scaleImage != value)
	{
		scaleImage = value;
		configureScrollBars();
		Refresh();
	}
}

bool ImageViewer::getScaleImage()
{
	return scaleImage;
}

void ImageViewer::OnDraw(wxDC& dc)
{
	if (masterBitmap != 0)
	{
		if (scaleImage)
		{
			int width, height;
			GetClientSize(&width, &height);
			dc.DrawBitmap(*scaledBitmap, (width - scaledBitmap->GetWidth()) / 2, (height - scaledBitmap->GetHeight()) / 2, false);
		}
		else
		{
			dc.DrawBitmap(*masterBitmap, 0, 0, false);
		}
	}
}

void ImageViewer::scaleMasterImage()
{
    if (scaledBitmap != 0)
    {
        delete scaledBitmap;
    }
    wxImage& image = masterBitmap->ConvertToImage();
    int clientWidth, clientHeight;
	GetClientSize(&clientWidth, &clientHeight);

    double sx = (double)clientWidth / image.GetWidth();
    double sy = (double)clientHeight / image.GetHeight();
    double scale = sx;
	if(sy < scale)
	{
		scale = sy;
	}

    image.Rescale((int)(image.GetWidth() * scale), (int)(image.GetHeight() * scale));
    scaledBitmap = new wxBitmap(image);
}

void ImageViewer::configureScrollBars()
{
    if (scaleImage)
    {
        SetScrollbars(1, 1, 0, 0, 0, 0, true);
    }
    else
    {
        SetScrollbars(1, 1, masterBitmap->GetWidth(), masterBitmap->GetHeight(), 0, 0, true);
    }
}

void ImageViewer::OnSize(wxEvent& evt)
{
	evt.Skip();
    if (scaleImage)
    {
        scaleMasterImage();
        Refresh();
    }
}

void ImageViewer::saveFile(String path, wxBitmapType type)
{
	masterBitmap->SaveFile(path, type);
}