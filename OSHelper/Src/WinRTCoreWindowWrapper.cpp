#include "StdAfx.h"
#include "WinRTWindow.h"
#include "WinRTCoreWindowWrapper.h"

using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;

WinRTCoreWindowWrapper::WinRTCoreWindowWrapper(Windows::UI::Core::CoreWindow^ window)
:coreWindow(coreWindow)
{
	
}

WinRTCoreWindowWrapper::~WinRTCoreWindowWrapper()
{

}

void WinRTCoreWindowWrapper::setNativeWindow(WinRTWindow* window)
{
	nativeWindow = window;
	coreWindow->SizeChanged += ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &WinRTCoreWindowWrapper::OnWindowSizeChanged);
	coreWindow->VisibilityChanged += ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &WinRTCoreWindowWrapper::OnVisibilityChanged);
	coreWindow->Closed += ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &WinRTCoreWindowWrapper::OnWindowClosed);
}

void WinRTCoreWindowWrapper::OnWindowSizeChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args)
{

}

void WinRTCoreWindowWrapper::OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args)
{
	//windowVisible = args->Visible;
}

void WinRTCoreWindowWrapper::OnWindowClosed(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::CoreWindowEventArgs^ args)
{
	//runningLoop = false;
}