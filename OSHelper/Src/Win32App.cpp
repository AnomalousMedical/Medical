#include "StdAfx.h"

#ifndef USE_WXWIDGETS
#include "Win32App.h"

Win32App::Win32App()
	:running(false)
{

}

Win32App::~Win32App()
{

}

void Win32App::run()
{
	running = fireInit();
	MSG  msg;
	while(running)
	{
		while( PeekMessage( &msg, NULL, 0U, 0U, PM_REMOVE ) )
		{
			TranslateMessage( &msg );
			DispatchMessage( &msg );

			if(msg.message == WM_QUIT)
			{
				running = false;
			}
		}
		fireIdle();
	}
	fireExit();
}

void Win32App::exit()
{
	running = false;
}

//PInvoke
extern "C" _AnomalousExport Win32App* App_create()
{
	return new Win32App();
}

extern "C" _AnomalousExport void App_pumpMessages()
{
	MSG  msg;
	while( PeekMessage( &msg, NULL, 0U, 0U, PM_REMOVE ) )
	{
		TranslateMessage( &msg );
		DispatchMessage( &msg );
	}
}

#endif