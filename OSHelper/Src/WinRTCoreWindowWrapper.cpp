#include "StdAfx.h"
#include "WinRTWindow.h"
#include "WinRTCoreWindowWrapper.h"

using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;

WinRTCoreWindowWrapper::WinRTCoreWindowWrapper(Windows::UI::Core::CoreWindow^ coreWindow)
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
	coreWindow->Activated += ref new TypedEventHandler<CoreWindow ^, WindowActivatedEventArgs ^>(this, &WinRTCoreWindowWrapper::OnActivated);
}

void WinRTCoreWindowWrapper::OnWindowSizeChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args)
{
	nativeWindow->fireSized();
}

void WinRTCoreWindowWrapper::OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args)
{
	
}

void WinRTCoreWindowWrapper::OnWindowClosed(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::CoreWindowEventArgs^ args)
{
	nativeWindow->fireClosing();
	nativeWindow->fireClosed();
}

void WinRTCoreWindowWrapper::OnActivated(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::WindowActivatedEventArgs ^args)
{
	nativeWindow->fireActivate(args->WindowActivationState != CoreWindowActivationState::Deactivated);
}
