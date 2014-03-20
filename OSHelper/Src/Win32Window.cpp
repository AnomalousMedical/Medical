#include "StdAfx.h"
#include "Win32Window.h"

LRESULT WINAPI MsgProc( HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam );

Win32Window::Win32Window(HWND parent, String title, int x, int y, int width, int height, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activateCB)
	:NativeOSWindow(deleteCB, sizedCB, closedCB, activateCB),
	window(0)
{	
	window = CreateWindowEx(NULL, WIN32_WINDOW_CLASS, title, WS_OVERLAPPEDWINDOW, x, y, width, height, parent, NULL, wndclass.hInstance, NULL);
	SetWindowLong(window, GWL_USERDATA, (LONG)this);
	setCursor(Arrow);

	for(int i = 0; i < MouseButtonCode::NUM_BUTTONS; ++i)
	{
		mouseDown[i] = false;
	}
}
    
Win32Window::~Win32Window()
{
	if(window)
	{
		DestroyWindow(window);
		window = 0;
	}
}
    
void Win32Window::setTitle(String title)
{
	SetWindowText(window, title);
}
    
void Win32Window::showFullScreen()
{
	SetWindowLong(window, GWL_STYLE, WS_POPUP | WS_EX_TOPMOST);
	SetWindowPos(window, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
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
	switch(cursor)
	{
		case Arrow:
			hCursor = LoadCursor(NULL, IDC_ARROW);
			break;
		case Beam:
			hCursor = LoadCursor(NULL, IDC_IBEAM);
			break;
		case SizeLeft:
			hCursor = LoadCursor(NULL, IDC_SIZENWSE);
			break;
		case SizeRight:
			hCursor = LoadCursor(NULL, IDC_SIZENESW);
			break;
		case SizeHorz:
			hCursor = LoadCursor(NULL, IDC_SIZEWE);
			break;
		case SizeVert:
			hCursor = LoadCursor(NULL, IDC_SIZENS);
			break;
		case Hand:
			hCursor = LoadCursor(NULL, IDC_HAND);
			break;
		case Link:
			hCursor = LoadCursor(NULL, IDC_ARROW);
			break;
		default:
			hCursor = LoadCursor(NULL, IDC_ARROW);
			break;
	}
}

float Win32Window::getWindowScaling()
{
	HDC hdc = GetDC(window);
	if (hdc)
	{
		int dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
		ReleaseDC(NULL, hdc);

		return dpiX / 96.0f; //Windows base dpi is 96
	}
	return 1.0f;
}

void Win32Window::manageCapture(MouseButtonCode mouseCode)
{
	bool shouldCapture = true;
	for(int i = 0; i < MouseButtonCode::NUM_BUTTONS; ++i)
	{
		if(mouseDown[i])
		{
			shouldCapture = false;
		}
	}
	mouseDown[mouseCode] = true;
	if(shouldCapture)
	{
		SetCapture(window);
	}
}

void Win32Window::manageRelease(MouseButtonCode mouseCode)
{
	mouseDown[mouseCode] = false;
	bool release = true;
	for(int i = 0; i < MouseButtonCode::NUM_BUTTONS; ++i)
	{
		if(mouseDown[i])
		{
			release = false;
		}
	}
	if(release)
	{
		ReleaseCapture();
	}
}

//PInvoke
extern "C" _AnomalousExport NativeOSWindow* NativeOSWindow_create(NativeOSWindow* parent, String caption, int x, int y, int width, int height, bool floatOnParent, NativeOSWindow::DeleteDelegate deleteCB, NativeOSWindow::SizedDelegate sizedCB, NativeOSWindow::ClosedDelegate closedCB, NativeOSWindow::ActivateDelegate activateCB)
{
	HWND parentHwnd = NULL;
	if(parent != 0 && floatOnParent)
	{
		parentHwnd = (HWND)parent->getHandle();
	}
	return new Win32Window(parentHwnd, caption, x, y, width, height, deleteCB, sizedCB, closedCB, activateCB);
}

//Windows
#include "Windowsx.h"

//Win32 Message Proc
uint getUtf32WithSpecial(WPARAM virtualKey, unsigned int scanCode);

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
			case WM_SETCURSOR:
				if(LOWORD(lParam) == HTCLIENT)
				{
					win->activateCursor();
					return true;
				}
				break;

			//Keyboard
			case WM_KEYDOWN:
			case WM_SYSKEYDOWN:
				if(wParam == VK_MENU)
				{
					win->fireKeyDown(KC_LMENU, 0);
				}
				else if (wParam == VK_CONTROL)
				{
					win->fireKeyDown(KC_LCONTROL, 0);
				}
				else
				{
					win->fireKeyDown(virtualKeyToKeyboardButtonCode(wParam), getUtf32WithSpecial(wParam, (lParam & 0x01FF0000) >> 16));
				}
				break;
			case WM_KEYUP:
			case WM_SYSKEYUP:
				if(wParam == VK_MENU)
				{
					win->fireKeyUp(KC_LMENU);
				}
				else if (wParam == VK_CONTROL)
				{
					win->fireKeyUp(KC_LCONTROL);
				}
				else
				{
					win->fireKeyUp(virtualKeyToKeyboardButtonCode(wParam));
				}
				break;

			//Mouse
			case WM_LBUTTONDOWN:
				win->manageCapture(MB_BUTTON0);
				win->fireMouseButtonDown(MB_BUTTON0);
				break;
			case WM_LBUTTONUP:
				win->fireMouseButtonUp(MB_BUTTON0);
				win->manageRelease(MB_BUTTON0);
				break;
			case WM_RBUTTONDOWN:
				win->manageCapture(MB_BUTTON1);
				win->fireMouseButtonDown(MB_BUTTON1);
				break;
			case WM_RBUTTONUP:
				win->fireMouseButtonUp(MB_BUTTON1);
				win->manageRelease(MB_BUTTON1);
				break;
			case WM_MBUTTONDOWN:
				win->manageCapture(MB_BUTTON2);
				win->fireMouseButtonDown(MB_BUTTON2);
				break;
			case WM_MBUTTONUP:
				win->fireMouseButtonUp(MB_BUTTON2);
				win->manageRelease(MB_BUTTON2);
				break;
			case WM_XBUTTONDOWN:
				switch(GET_XBUTTON_WPARAM (wParam))
				{
					case XBUTTON1:
						win->manageCapture(MB_BUTTON3);
						win->fireMouseButtonDown(MB_BUTTON3);
						break;
					case XBUTTON2:
						win->manageCapture(MB_BUTTON4);
						win->fireMouseButtonDown(MB_BUTTON4);
						break;
				}
				break;
			case WM_XBUTTONUP:
				switch(GET_XBUTTON_WPARAM (wParam))
				{
					case XBUTTON1:
						win->fireMouseButtonUp(MB_BUTTON3);
						win->manageRelease(MB_BUTTON3);
						break;
					case XBUTTON2:
						win->fireMouseButtonUp(MB_BUTTON4);
						win->manageRelease(MB_BUTTON4);
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

uint getUtf32WithSpecial(WPARAM virtualKey, unsigned int scanCode)
{
	switch(virtualKey)
	{
		case VK_NUMPAD0:
			return 48;
		case VK_NUMPAD1:
			return 49;
		case VK_NUMPAD2:
			return 50;
		case VK_NUMPAD3:
			return 51;
		case VK_NUMPAD4:
			return 52;
		case VK_NUMPAD5:
			return 53;
		case VK_NUMPAD6:
			return 54;
		case VK_NUMPAD7:
			return 55;
		case VK_NUMPAD8:
			return 56;
		case VK_NUMPAD9:
			return 57;
		case VK_DIVIDE:
			return 47;
		case VK_DECIMAL:
			return 46;
		default:
			return keycodeToUTF32(scanCode);
	}
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

WNDCLASSEX Win32Window::wndclass;

#include "..\Resource.h"

void Win32Window::createWindowClass(HANDLE hModule)
{
	wndclass.cbSize			=	sizeof( wndclass );
	wndclass.style			=	CS_OWNDC;// | CS_DBLCLKS;/*CS_HREDRAW | CS_VREDRAW |*/ 
	wndclass.lpfnWndProc	=	&MsgProc;
	wndclass.cbClsExtra		=	0;
	wndclass.cbWndExtra		=	0;
	wndclass.hInstance		=	(HINSTANCE)hModule;
	wndclass.hIcon			=	LoadIcon((HINSTANCE)hModule, MAKEINTRESOURCE(IDI_SKULL));
	wndclass.hIconSm		=	NULL;
	wndclass.hCursor		=	LoadCursor( NULL, IDC_ARROW );
	wndclass.hbrBackground	=	( HBRUSH ) ( COLOR_WINDOW );
	wndclass.lpszMenuName	=	NULL;
	wndclass.lpszClassName	=	WIN32_WINDOW_CLASS; // Registered class name
	wndclass.hbrBackground = CreateSolidBrush(RGB(0, 0, 0));

	if(RegisterClassEx(&wndclass))
	{
	}
}

void Win32Window::destroyWindowClass()
{
	UnregisterClass(WIN32_WINDOW_CLASS, wndclass.hInstance);
}