#pragma once

class NativeOSWindow : public wxFrame
{
public:
	typedef void (*DeleteDelegate)();
	typedef void (*SizedDelegate)();
	typedef void (*ClosedDelegate)();
	typedef void (*ActivateDelegate)(bool active);

	NativeOSWindow(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activateCB);

	virtual ~NativeOSWindow(void);

private:
	DeleteDelegate deleteCB;
	SizedDelegate sizedCB;
	ClosedDelegate closedCB;
	ActivateDelegate activateCB;

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

	void OnActivate(wxActivateEvent& event)
	{
		event.Skip();
		activateCB(event.GetActive());
	}
};
