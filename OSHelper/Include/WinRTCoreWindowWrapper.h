#pragma once

#include "NativeOSWindow.h"
#include <agile.h>

class WinRTWindow;

ref class WinRTCoreWindowWrapper sealed
{
public:    
	virtual ~WinRTCoreWindowWrapper();

	Windows::UI::Core::CoreWindow^ getWindow()
	{
		return coreWindow;
	}

internal:
	WinRTCoreWindowWrapper(Windows::UI::Core::CoreWindow^ coreWindow);

	void setNativeWindow(WinRTWindow* window);

protected:
	// Window event handlers.
	void OnWindowSizeChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args);
	void OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args);
	void OnWindowClosed(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::CoreWindowEventArgs^ args);
	void OnActivated(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::WindowActivatedEventArgs ^args);

private:
	Windows::UI::Core::CoreWindow^ coreWindow;
	WinRTWindow* nativeWindow;
};