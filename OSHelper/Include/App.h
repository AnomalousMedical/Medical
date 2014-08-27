#pragma once

typedef bool (*OnInitDelegate)();
typedef int (*OnExitDelegate)();
typedef void (*OnIdleDelegate)();

class App
{
public:
	App(void);
    
	virtual ~App(void);
    
	void registerDelegates(OnInitDelegate onInitCB, OnExitDelegate onExitCB, OnIdleDelegate onIdleCB);
    
    virtual void run() = 0;
    
    virtual void exit() = 0;
    
	void fireIdle()
    {
        onIdleCB();
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