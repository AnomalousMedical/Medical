#pragma once

typedef bool (*OnInitDelegate)();
typedef int (*OnExitDelegate)();
typedef bool (*OnIdleDelegate)();

class App : public wxApp
{
public:
	App(void);

	virtual ~App(void);

	virtual bool OnInit()
	{
		wxInitAllImageHandlers();
		return onInitCB()!=0;
	}

	virtual int OnExit()
	{
		return onExitCB();
	}

	virtual void OnIdle(wxIdleEvent& event);

	void registerDelegates(OnInitDelegate onInitCB, OnExitDelegate onExitCB, OnIdleDelegate onIdleCB);

	void run(int argc, char* argv[]);

private:
	OnInitDelegate onInitCB;
	OnExitDelegate onExitCB;
	OnIdleDelegate onIdleCB;

	DECLARE_EVENT_TABLE()
};