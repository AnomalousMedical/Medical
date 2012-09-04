#include "StdAfx.h"

#ifndef USE_WXWIDGETS
#include "Win32Window.h"
#include "Windowsx.h"

LRESULT WINAPI MsgProc( HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam );

Win32Window::Win32Window(String title, int x, int y, int width, int height, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activateCB)
	:NativeOSWindow(deleteCB, sizedCB, closedCB, activateCB),
	window(0),
	downKeyCode(0)
{	
	//Need to limit the wndclass to only be created once
	wndclass.cbSize			=	sizeof( wndclass );
	wndclass.style			=	CS_HREDRAW | CS_VREDRAW | CS_OWNDC;// | CS_DBLCLKS;
	wndclass.lpfnWndProc	=	&MsgProc;
	wndclass.cbClsExtra		=	0;
	wndclass.cbWndExtra		=	0;
	wndclass.hInstance		=	(HINSTANCE)GetModuleHandle(NULL);
	wndclass.hIcon			=	LoadIcon( NULL, IDI_APPLICATION );
	wndclass.hCursor		=	LoadCursor( NULL, IDC_ARROW );
	wndclass.hbrBackground	=	( HBRUSH ) ( COLOR_WINDOW );
	wndclass.lpszMenuName	=	NULL;
	wndclass.lpszClassName	=	L"Application"; // Registered class name
	wndclass.hIconSm		=	LoadIcon( NULL, IDI_APPLICATION );
	wndclass.hbrBackground = CreateSolidBrush(RGB(0, 0, 0));

	if(RegisterClassEx(&wndclass))
	{
		window = CreateWindowA("Application", title, WS_OVERLAPPEDWINDOW, x, y, width, height, NULL, NULL, wndclass.hInstance, NULL);
		SetWindowLong(window, GWL_USERDATA, (LONG)this);
	}
}
    
Win32Window::~Win32Window()
{
	UnregisterClass(L"Application", wndclass.hInstance);
	if(window)
	{
		DestroyWindow(window);
		window = 0;
	}
}
    
void Win32Window::setTitle(String title)
{
	SetWindowTextA(window, title);
}
    
void Win32Window::showFullScreen()
{

}
    
void Win32Window::setSize(int width, int height)
{
	SetWindowPos(window, NULL, 0, 0, width, height, SWP_NOMOVE|SWP_NOZORDER|SWP_NOACTIVATE);
}
    
int Win32Window::getWidth()
{
	RECT windowRect;
	GetClientRect(window, &windowRect);
	return windowRect.right - windowRect.left;
}
    
int Win32Window::getHeight()
{
	RECT windowRect;
	GetClientRect(window, &windowRect);
	return windowRect.bottom - windowRect.top;
}

void* Win32Window::getHandle()
{
	return window;
}
    
void Win32Window::show()
{
	WINDOWPLACEMENT placement;
	GetWindowPlacement(window, &placement);
	ShowWindow(window, placement.showCmd);
	UpdateWindow(window);
}
    
void Win32Window::close()
{
	PostMessage(window, WM_CLOSE, 0, 0);
}
    
void Win32Window::setMaximized(bool maximized)
{
	WINDOWPLACEMENT placement;
	GetWindowPlacement(window, &placement);
	if(maximized)
	{
		placement.showCmd = SW_MAXIMIZE;
	}
	else
	{
		placement.showCmd = SW_SHOWNORMAL;
	}
	SetWindowPlacement(window, &placement);
}
    
bool Win32Window::getMaximized()
{
	WINDOWPLACEMENT placement;
	GetWindowPlacement(window, &placement);
	return placement.showCmd == SW_MAXIMIZE;
}
    
void Win32Window::setCursor(CursorType cursor)
{

}

//Win32 Message Proc
uint keycodeToUTF32( unsigned int scanCode);

KeyboardButtonCode virtualKeyToKeyboardButtonCode(WPARAM wParam);

LRESULT WINAPI MsgProc( HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam )
{
	Win32Window *win = (Win32Window*)GetWindowLong(hWnd, GWL_USERDATA);
	if(win)
	{
		switch(msg)
		{
			//Window
			case WM_SIZE:
				win->fireSized();
				break;
			case WM_CLOSE:
				win->fireClosed();
				break;
			case WM_ACTIVATEAPP:
				win->fireActivate(wParam);
				break;

			//Keyboard
			case WM_CHAR:
				//win->downKeyCode = wParam;
				//win->fireKeyDown(win->keyboardButtonCode, wParam);
				break;
			case WM_KEYDOWN:
				//win->keyboardButtonCode = (KeyboardButtonCode)wParam;
				win->fireKeyDown(virtualKeyToKeyboardButtonCode(wParam), keycodeToUTF32((lParam & 0x01FF0000) >> 16));
				break;
			case WM_KEYUP:
				win->fireKeyUp(virtualKeyToKeyboardButtonCode(wParam));
				break;

			//Mouse
			case WM_LBUTTONDOWN:
				win->fireMouseButtonDown(MB_BUTTON0);
				break;
			case WM_LBUTTONUP:
				win->fireMouseButtonUp(MB_BUTTON0);
				break;
			case WM_RBUTTONDOWN:
				win->fireMouseButtonDown(MB_BUTTON1);
				break;
			case WM_RBUTTONUP:
				win->fireMouseButtonUp(MB_BUTTON1);
				break;
			case WM_MBUTTONDOWN:
				win->fireMouseButtonDown(MB_BUTTON2);
				break;
			case WM_MBUTTONUP:
				win->fireMouseButtonUp(MB_BUTTON2);
				break;
			case WM_XBUTTONDOWN:
				switch(GET_XBUTTON_WPARAM (wParam))
				{
					case XBUTTON1:
						win->fireMouseButtonDown(MB_BUTTON3);
						break;
					case XBUTTON2:
						win->fireMouseButtonDown(MB_BUTTON4);
						break;
				}
				break;
			case WM_XBUTTONUP:
				switch(GET_XBUTTON_WPARAM (wParam))
				{
					case XBUTTON1:
						win->fireMouseButtonUp(MB_BUTTON3);
						break;
					case XBUTTON2:
						win->fireMouseButtonUp(MB_BUTTON4);
						break;
				}
				break;
			case WM_MOUSEMOVE:
				win->fireMouseMove(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
				break;
			case WM_MOUSEWHEEL:
				win->fireMouseWheel(GET_WHEEL_DELTA_WPARAM(wParam));
				break;
		}
	}
	return DefWindowProc(hWnd, msg, wParam, lParam);
}

//Taken from CEGUI wiki
//http://www.cegui.org.uk/wiki/index.php/DirectInput_to_CEGUI_utf32
uint keycodeToUTF32( unsigned int scanCode)
 {
 	uint utf = 0;
 
 	BYTE keyboardState[256];
 	unsigned char ucBuffer[3];
 	static WCHAR deadKey = '\0';
 
 	// Retrieve the keyboard layout in order to perform the necessary convertions
 	HKL hklKeyboardLayout = GetKeyboardLayout(0); // 0 means current thread 
 	// This seemingly cannot fail 
 	// If this value is cached then the application must respond to WM_INPUTLANGCHANGE 
 
 	// Retrieve the keyboard state
 	// Handles CAPS-lock and SHIFT states
 	if (GetKeyboardState(keyboardState) == FALSE)
 		return utf;
 
 	/* 0. Convert virtual-key code into a scan code
        1. Convert scan code into a virtual-key code
 	      Does not distinguish between left- and right-hand keys.
        2. Convert virtual-key code into an unshifted character value
 	      in the low order word of the return value. Dead keys (diacritics)
 		  are indicated by setting the top bit of the return value.
        3. Windows NT/2000/XP: Convert scan code into a virtual-key
 	      Distinguishes between left- and right-hand keys.*/
 	UINT virtualKey = MapVirtualKeyEx(scanCode, 3, hklKeyboardLayout);
 	if (virtualKey == 0) // No translation possible
 		return utf;
 
     /* Parameter 5:
 		0. No menu is active
 		1. A menu is active
        Return values:
 		Negative. Returned a dead key
 		0. No translation available
 		1. A translation exists 
 		2. Dead-key could not be combined with character 	*/
 	int ascii = ToAsciiEx(virtualKey, scanCode, keyboardState, (LPWORD) ucBuffer, 0, hklKeyboardLayout);
 	if(deadKey != '\0' && ascii == 1)
 	{
 		// A dead key is stored and we have just converted a character key
 		// Combine the two into a single character
 		WCHAR wcBuffer[3];
 		WCHAR out[3];
 		wcBuffer[0] = ucBuffer[0];
 		wcBuffer[1] = deadKey;
 		wcBuffer[2] = '\0';
 		if( FoldStringW(MAP_PRECOMPOSED, (LPWSTR) wcBuffer, 3, (LPWSTR) out, 3) )
 			utf = out[0];
 		else
 		{
 			// FoldStringW failed
 			DWORD dw = GetLastError();
 			switch(dw)
 			{
 			case ERROR_INSUFFICIENT_BUFFER:
 			case ERROR_INVALID_FLAGS:
 			case ERROR_INVALID_PARAMETER:
 				break;
 			}
 		}
 		deadKey = '\0';
 	}
 	else if (ascii == 1)
 	{
 		// We have a single character
 		utf = ucBuffer[0];
 		deadKey = '\0';
 	}
 	else
 	{
 		// Convert a non-combining diacritical mark into a combining diacritical mark
 		switch(ucBuffer[0])
 		{
 		case 0x5E: // Circumflex accent: â
 			deadKey = 0x302;
 			break;
 		case 0x60: // Grave accent: � 
 			deadKey = 0x300;
 			break;
 		case 0xA8: // Diaeresis: ü
 			deadKey = 0x308;
 			break;
 		case 0xB4: // Acute accent: é
 			deadKey = 0x301;
 			break;
 		case 0xB8: // Cedilla: ç
 			deadKey = 0x327;
 			break;
 		default:
 			deadKey = ucBuffer[0];
 		}
 	}
 
 	return utf;
 }

#include "Win32KeyMap.h"

KeyboardButtonCode virtualKeyToKeyboardButtonCode(WPARAM wParam)
{
	if(wParam < KEY_MAP_MAX)
	{
		return keyMap[wParam];
	}
	return KC_UNASSIGNED;
}

//PInvoke
extern "C" _AnomalousExport NativeOSWindow* NativeOSWindow_create(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, NativeOSWindow::DeleteDelegate deleteCB, NativeOSWindow::SizedDelegate sizedCB, NativeOSWindow::ClosedDelegate closedCB, NativeOSWindow::ActivateDelegate activateCB)
{
	return new Win32Window(caption, x, y, width, height, deleteCB, sizedCB, closedCB, activateCB);
}

#endif