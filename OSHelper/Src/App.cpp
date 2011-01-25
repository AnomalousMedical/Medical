#include "StdAfx.h"
#include "..\Include\App.h"

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

void App::OnIdle(wxIdleEvent& event)
{
	onIdleCB();
	event.RequestMore();
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

	//void wxNetEntry(HINSTANCE hInstance, HINSTANCE WXUNUSED(hPrevInstance), char * WXUNUSED(pCmdLine), int nCmdShow)
	//{
	//  // remember the parameters Windows gave us
	//	wxSetInstance(hInstance);
	//	wxApp::m_nCmdShow = nCmdShow;

	//	// parse the command line: we can't use pCmdLine in Unicode build so it is
	//	// simpler to never use it at all (this also results in a more correct
	//	// argv[0])

	//	// break the command line in words
	//	wxArrayString args;
	//	const wxChar *cmdLine = ::GetCommandLine();
	//	if ( cmdLine )
	//	{
	//		args = wxCmdLineParser::ConvertStringToArgs(cmdLine);
	//	}

	//	int argc = args.GetCount();

	//	// +1 here for the terminating NULL
	//	wxChar **argv = new wxChar *[argc + 1];
	//	for ( int i = 0; i < argc; i++ )
	//	{
	//		argv[i] = wxStrdup(args[i]);
	//	}

	//	// argv[] must be NULL-terminated
	//	argv[argc] = NULL;

	//	wxEntry(argc, argv);
	//}

	void App::run(int argc, char* argv[])
	{
		//LPWSTR
		//wxNetEntry((HINSTANCE)thisModule, NULL, (char*)GetCommandLineW(), 0);
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