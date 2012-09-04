#pragma once

#include "App.h"

class Win32App : public App
{
public:
    Win32App();
    
    virtual ~Win32App();
    
    virtual void run();
    
    virtual void exit();

private:
	bool running;
};