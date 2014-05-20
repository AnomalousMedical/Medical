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
	mainWindow = window;
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

CoreWindow^ AnomalousRTFramework::getWindow()
{
	return mainWindow;
}

AnomalousFrameworkSource::AnomalousFrameworkSource(WinRTApp* anomalousApp)
:anomalousApp(anomalousApp)
{

}

IFrameworkView^ AnomalousFrameworkSource::CreateView()
{
	return ref new AnomalousRTFramework(anomalousApp);
}