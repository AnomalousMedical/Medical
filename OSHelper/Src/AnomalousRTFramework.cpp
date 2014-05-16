#include "StdAfx.h"
#include "WinRTApp.h"
#include "AnomalousRTFramework.h"

using namespace Windows::ApplicationModel::Core;

AnomalousRTFramework^ AnomalousRTFramework::instance;

AnomalousRTFramework::AnomalousRTFramework(WinRTApp* anomalousApp)
:anomalousApp(anomalousApp)
{
	instance = this;
}

AnomalousRTFramework::~AnomalousRTFramework()
{

}

void AnomalousRTFramework::Initialize(Windows::ApplicationModel::Core::CoreApplicationView^ applicationView)
{

}

void AnomalousRTFramework::SetWindow(Windows::UI::Core::CoreWindow^ window)
{

}

void AnomalousRTFramework::Load(Platform::String^ entryPoint)
{

}

void AnomalousRTFramework::Run()
{
	if (anomalousApp->fireInit())
	{
		while (true)
		{
			anomalousApp->fireIdle();
		}
	}
	anomalousApp->fireExit();
}

void AnomalousRTFramework::Uninitialize()
{

}

AnomalousFrameworkSource::AnomalousFrameworkSource(WinRTApp* anomalousApp)
:anomalousApp(anomalousApp)
{

}

IFrameworkView^ AnomalousFrameworkSource::CreateView()
{
	return ref new AnomalousRTFramework(anomalousApp);
}