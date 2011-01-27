#include "StdAfx.h"
#include "App.h"
#include <wx/filesys.h>

App::App(void)
{
}

App::~App(void)
{
}

void App::registerDelegates(OnInitDelegate onInitCB, OnExitDelegate onExitCB, OnIdleDelegate onIdleCB)
{
	this->onInitCB = onInitCB;
	this->onExitCB = onExitCB;
	this->onIdleCB = onIdleCB;
}

bool App::OnInit()
{
	wxInitAllImageHandlers();
	wxFileSystem::AddHandler(&zipHandler);

#ifdef MAC_OSX
	wxApp::s_macAboutMenuItemId = wxID_ABOUT;
	wxApp::s_macPreferencesMenuItemId = wxID_PREFERENCES;
#endif

	return onInitCB()!=0;
}

int App::OnExit()
{
	wxFileSystem::RemoveHandler(&zipHandler);
	return onExitCB();
}

void App::OnIdle(wxIdleEvent& event)
{
	if(onIdleCB() != 0)
	{
		event.RequestMore();
	}
}

BEGIN_EVENT_TABLE(App, wxApp)
    EVT_IDLE(App::OnIdle)
END_EVENT_TABLE()

#if defined(WINDOWS)

	#define WIN32_LEAN_AND_MEAN
	#include <windows.h>

	#include <wx/msw/private.h>
	#include <wx/cmdline.h>

	static HANDLE thisModule;

	BOOL APIENTRY DllMain(HANDLE hModule, DWORD ulReasonForCall, LPVOID lpReserved)
	{
		if (ulReasonForCall == DLL_PROCESS_ATTACH)
			thisModule = hModule;
		return TRUE;
	}

	void App::run(int argc, char* argv[])
	{
		wxEntry((HINSTANCE)thisModule, NULL, (char*)GetCommandLineW(), 0);
	}

#else

	void App::run(int argc, char* argv[])
	{
		wxEntry(argc, argv);
	}

#endif

//PInvoke

extern "C" _AnomalousExport App* App_create()
{
	return new App();
}

extern "C" _AnomalousExport void App_delete(App* app)
{
	delete app;
}

extern "C" _AnomalousExport void App_registerDelegates(App* app, OnInitDelegate onInitCB, OnExitDelegate onExitCB, OnIdleDelegate onIdleCB)
{
	app->registerDelegates(onInitCB, onExitCB, onIdleCB);
}

extern "C" _AnomalousExport void App_run(App* app, int argc, char* argv[])
{
	app->run(argc, argv);
}

DECLARE_APP(App)
IMPLEMENT_APP(App)