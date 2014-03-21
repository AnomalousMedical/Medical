#include "StdAfx.h"
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
	if (fireInit())
	{
		MSG  msg;
		while (true)
		{
			if(PeekMessage(&msg, NULL, 0U, 0U, PM_REMOVE))
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);

				if (msg.message == WM_QUIT)
				{
					break;
				}
			}
			else
			{
				fireIdle();
			}
		}
	}
	fireExit();
}

void Win32App::exit()
{
	PostQuitMessage(0);
}

//PInvoke
extern "C" _AnomalousExport Win32App* App_create()
{
	return new Win32App();
}