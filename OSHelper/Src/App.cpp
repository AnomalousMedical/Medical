#include "StdAfx.h"
#include "App.h"

App::App()
{
    
}

App::~App()
{
    
}

void App::registerDelegates(OnInitDelegate onInitCB, OnExitDelegate onExitCB, OnIdleDelegate onIdleCB)
{
    this->onInitCB = onInitCB;
    this->onExitCB = onExitCB;
    this->onIdleCB = onIdleCB;
}

//PInvoke
extern "C" _AnomalousExport void App_delete(App* app)
{
	delete app;
}

extern "C" _AnomalousExport void App_registerDelegates(App* app, OnInitDelegate onInitCB, OnExitDelegate onExitCB, OnIdleDelegate onIdleCB)
{
	app->registerDelegates(onInitCB, onExitCB, onIdleCB);
}

extern "C" _AnomalousExport void App_run(App* app)
{
	app->run();
}

extern "C" _AnomalousExport void App_exit(App* app)
{
	app->exit();
}