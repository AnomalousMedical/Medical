#pragma once

typedef bool (*OnInitDelegate)();
typedef int (*OnExitDelegate)();
typedef bool (*OnIdleDelegate)();

#include <wx/fs_zip.h>

class App : public wxApp
{
public:
	App(void);

	virtual ~App(void);

	virtual bool OnInit();

	virtual int OnExit();

	virtual void OnIdle(wxIdleEvent& event);

	void registerDelegates(OnInitDelegate onInitCB, OnExitDelegate onExitCB, OnIdleDelegate onIdleCB);

	void run(int argc, char* argv[]);

	void exit();

private:
	OnInitDelegate onInitCB;
	OnExitDelegate onExitCB;
	OnIdleDelegate onIdleCB;
	wxZipFSHandler zipHandler;

	DECLARE_EVENT_TABLE()
};