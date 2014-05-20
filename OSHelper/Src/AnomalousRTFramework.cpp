#include "StdAfx.h"
#include "WinRTApp.h"
#include "AnomalousRTFramework.h"

using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;

AnomalousRTFramework^ AnomalousRTFramework::instance;

AnomalousRTFramework::AnomalousRTFramework(WinRTApp* anomalousApp)
:anomalousApp(anomalousApp),
runningLoop(true),
windowVisible(false)
{
	instance = this;
}

AnomalousRTFramework::~AnomalousRTFramework()
{

}

void AnomalousRTFramework::Initialize(CoreApplicationView^ applicationView)
{
	applicationView->Activated +=
		ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &AnomalousRTFramework::OnActivated);

	CoreApplication::Suspending +=
		ref new EventHandler<SuspendingEventArgs^>(this, &AnomalousRTFramework::OnSuspending);

	CoreApplication::Resuming +=
		ref new EventHandler<Platform::Object^>(this, &AnomalousRTFramework::OnResuming);
}

void AnomalousRTFramework::SetWindow(CoreWindow^ window)
{
	mainWindow = window;

	window->VisibilityChanged += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Core::CoreWindow ^, Windows::UI::Core::VisibilityChangedEventArgs ^>(this, &AnomalousRTFramework::OnVisibilityChanged);
}

void AnomalousRTFramework::Load(Platform::String^ entryPoint)
{

}

void AnomalousRTFramework::Run()
{
	CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
	if (anomalousApp->fireInit())
	{
		CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
		while (runningLoop)
		{
			if (windowVisible)
			{
				CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
				runningLoop = anomalousApp->fireIdle();
			}
			else
			{
				CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
			}
		}
	}
	anomalousApp->fireExit();
}

void AnomalousRTFramework::Uninitialize()
{

}

void AnomalousRTFramework::stopRunLoop()
{
	runningLoop = false;
}

CoreWindow^ AnomalousRTFramework::getWindow()
{
	return mainWindow;
}

void AnomalousRTFramework::OnActivated(CoreApplicationView^ applicationView, IActivatedEventArgs^ args)
{
	// Run() won't start until the CoreWindow is activated.
	CoreWindow::GetForCurrentThread()->Activate();
}

void AnomalousRTFramework::OnSuspending(Platform::Object^ sender, SuspendingEventArgs^ args)
{
	// Save app state asynchronously after requesting a deferral. Holding a deferral
	// indicates that the application is busy performing suspending operations. Be
	// aware that a deferral may not be held indefinitely. After about five seconds,
	// the app will be forced to exit.
	//SuspendingDeferral^ deferral = args->SuspendingOperation->GetDeferral();

	//create_task([this, deferral]()
	//{
	//	m_deviceResources->Trim();

	//	// Insert your code here.

	//	deferral->Complete();
	//});
}

void AnomalousRTFramework::OnVisibilityChanged(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::VisibilityChangedEventArgs ^args)
{
	windowVisible = args->Visible;
}

void AnomalousRTFramework::OnResuming(Platform::Object^ sender, Platform::Object^ args)
{
	// Restore any data or state that was unloaded on suspend. By default, data
	// and state are persisted when resuming from suspend. Note that this event
	// does not occur if the app was previously terminated.

	// Insert your code here.
}

AnomalousFrameworkSource::AnomalousFrameworkSource(WinRTApp* anomalousApp)
:anomalousApp(anomalousApp)
{

}

IFrameworkView^ AnomalousFrameworkSource::CreateView()
{
	return ref new AnomalousRTFramework(anomalousApp);
}
