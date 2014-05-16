#pragma once

#include "App.h"

class WinRTApp : public App
{
public:
	WinRTApp();
    
	virtual ~WinRTApp();
    
    virtual void run();
    
    virtual void exit();
};