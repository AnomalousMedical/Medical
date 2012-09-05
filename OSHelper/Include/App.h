#pragma once

typedef bool (*OnInitDelegate)();
typedef int (*OnExitDelegate)();
typedef bool (*OnIdleDelegate)();

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