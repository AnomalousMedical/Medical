#include "StdAfx.h"
#include "WinRTApp.h"
#include "AnomalousRTFramework.h"

using namespace Windows::ApplicationModel::Core;
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

}

void AnomalousRTFramework::SetWindow(CoreWindow^ window)
{
	window->SizeChanged += ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &AnomalousRTFramework::OnWindowSizeChanged);
	window->VisibilityChanged += ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &AnomalousRTFramework::OnVisibilityChanged);
	window->Closed += ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &AnomalousRTFramework::OnWindowClosed);
}

void AnomalousRTFramework::Load(Platform::String^ entryPoint)
{

}

void AnomalousRTFramework::Run()
{
	if (anomalousApp->fireInit())
	{
		while (runningLoop)
		{
			anomalousApp->fireIdle();
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

void AnomalousRTFramework::OnWindowSizeChanged(CoreWindow^ sender, WindowSizeChangedEventArgs^ args)
{
	
}

void AnomalousRTFramework::OnVisibilityChanged(CoreWindow^ sender, VisibilityChangedEventArgs^ args)
{
	windowVisible = args->Visible;
}

void AnomalousRTFramework::OnWindowClosed(CoreWindow^ sender, CoreWindowEventArgs^ args)
{
	runningLoop = false;
}

AnomalousFrameworkSource::AnomalousFrameworkSource(WinRTApp* anomalousApp)
:anomalousApp(anomalousApp)
{

}

IFrameworkView^ AnomalousFrameworkSource::CreateView()
{
	return ref new AnomalousRTFramework(anomalousApp);
}