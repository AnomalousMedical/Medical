#pragma once

typedef bool (*OnInitDelegate)();
typedef int (*OnExitDelegate)();
typedef bool (*OnIdleDelegate)();

#ifdef USE_WXWIDGETS

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

#endif

#ifndef USE_WXWIDGETS

class App
{
public:
	App(void);
    
	virtual ~App(void);
    
	void registerDelegates(OnInitDelegate onInitCB, OnExitDelegate onExitCB, OnIdleDelegate onIdleCB);
    
    virtual void run() = 0;
    
    virtual void exit() = 0;
    
	bool fireIdle()
    {
        //return true;
        return onIdleCB();
    }
    
    bool fireInit()
    {
        return onInitCB();
    }
    
    int fireExit()
    {
        return onExitCB();
    }
    
private:
	OnInitDelegate onInitCB;
	OnExitDelegate onExitCB;
	OnIdleDelegate onIdleCB;
};

#endif