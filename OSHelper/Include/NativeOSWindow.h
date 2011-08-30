#pragma once

class NativeOSWindow : public wxFrame
{
public:
	typedef void (*DeleteDelegate)();
	typedef void (*SizedDelegate)();
	typedef void (*ClosedDelegate)();

	NativeOSWindow(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB);

	virtual ~NativeOSWindow(void);

private:
	DeleteDelegate deleteCB;
	SizedDelegate sizedCB;
	ClosedDelegate closedCB;

	void OnSize(wxEvent& event)
	{
		event.Skip();
		sizedCB();
	}

	void OnClose(wxEvent& event)
	{
		event.Skip();
		closedCB();
	}
};
