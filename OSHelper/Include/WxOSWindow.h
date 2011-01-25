#pragma once

class WxOSWindow
{
public:
	typedef void (*NativeCallback)();

	WxOSWindow(wxWindow* window);

	~WxOSWindow(void);

	WXWidget getHandle();

	int getWidth();

	int getHeight();

	void registerCallbacks(NativeCallback closedCallback, NativeCallback sizedCallback);

	wxWindow* getWxWindow()
	{
		return window;
	}

private:

	NativeCallback closedCallback;
	NativeCallback sizedCallback;

	wxWindow* window;

	void OnSize(wxEvent& event)
	{
		event.Skip();
		if(sizedCallback != NULL)
		{
			sizedCallback();
		}
	}

	void OnClose(wxEvent& event)
	{
		event.Skip();
		if(closedCallback != NULL)
		{
			closedCallback();
		}
	}
};
