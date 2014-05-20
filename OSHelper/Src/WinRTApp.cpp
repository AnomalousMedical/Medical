#include "StdAfx.h"
#include "WinRTApp.h"
#include "AnomalousRTFramework.h"

using namespace Windows::ApplicationModel::Core;

WinRTApp::WinRTApp()
{

}

WinRTApp::~WinRTApp()
{

}

void WinRTApp::run()
{
	auto source = ref new AnomalousFrameworkSource(this);
	CoreApplication::Run(source);
}

void WinRTApp::exit()
{
	AnomalousRTFramework::getSingleton()->stopRunLoop();
}

//PInvoke
extern "C" _AnomalousExport WinRTApp* App_create()
{
	return new WinRTApp();
}