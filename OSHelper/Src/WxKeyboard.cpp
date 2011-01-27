#include "StdAfx.h"
#include "WxKeyboard.h"
#include "NativeOSWindow.h"

KeyboardButtonCode WxKeyboard::keyConverter[397];

WxKeyboard::WxKeyboard(NativeOSWindow* osWindow, KeyDownDelegate keyDownCB, KeyUpDelegate keyUpCB)
:osWindow(osWindow),
keyDownCB(keyDownCB),
keyUpCB(keyUpCB),
downKeyCode(0)
{
	createConverterTable();

	wxWindow* window = osWindow;

	window->Bind(wxEVT_KEY_DOWN, &WxKeyboard::OnKeyDown, this);
	window->Bind(wxEVT_CHAR, &WxKeyboard::OnChar, this);
	window->Bind(wxEVT_KEY_UP, &WxKeyboard::OnKeyUp, this);
}

WxKeyboard::~WxKeyboard(void)
{
}

void WxKeyboard::OnKeyDown(wxKeyEvent& kevt)
{
    //If not one of the special case keys allow the Char event to handle the key press.
    downKeyCode = kevt.GetKeyCode();
    if (downKeyCode < 300)
    {
        kevt.Skip();
    }
    else
    {
		KeyboardButtonCode buttonCode = keyConverter[downKeyCode];
        uint sendChar = 0;
        //Check to see if one of the special buttons needs to be translated to a symbol or number
        switch (buttonCode)
        {
            case KC_NUMPAD7:
                sendChar = '7';
                break;
            case KC_NUMPAD8:
                sendChar = '8';
                break;
            case KC_NUMPAD9:
                sendChar = '9';
                break;
            case KC_SUBTRACT:
                sendChar = '-';
                break;
            case KC_NUMPAD4:
                sendChar = '4';
                break;
            case KC_NUMPAD5:
                sendChar = '5';
                break;
            case KC_NUMPAD6:
                sendChar = '6';
                break;
            case KC_ADD:
                sendChar = '+';
                break;
            case KC_NUMPAD1:
                sendChar = '1';
                break;
            case KC_NUMPAD2:
                sendChar = '2';
                break;
            case KC_NUMPAD3:
                sendChar = '3';
                break;
            case KC_NUMPAD0:
                sendChar = '0';
                break;
            case KC_DECIMAL:
                sendChar = '.';
                break;
            case KC_DIVIDE:
                sendChar = '/';
                break;
            case KC_MULTIPLY:
                sendChar = '*';
                break;
        }
		keyDownCB(buttonCode, sendChar);
    }
}

void WxKeyboard::OnChar(wxKeyEvent& kevt)
{
    KeyboardButtonCode buttonCode = keyConverter[downKeyCode];
	uint sendChar = kevt.GetUnicodeKey();
	keyDownCB(buttonCode, sendChar);
}

void WxKeyboard::OnKeyUp(wxKeyEvent& kevt)
{
    kevt.Skip();
    KeyboardButtonCode buttonCode = keyConverter[kevt.GetKeyCode()];
    keyUpCB(buttonCode);
}

void WxKeyboard::createConverterTable()
{
	//Top Row
	keyConverter[(int)WXK_ESCAPE] = KC_ESCAPE;
	keyConverter[(int)WXK_F1] = KC_F1;
	keyConverter[(int)WXK_F2] = KC_F2;
	keyConverter[(int)WXK_F3] = KC_F3;
	keyConverter[(int)WXK_F4] = KC_F4;
	keyConverter[(int)WXK_F5] = KC_F5;
	keyConverter[(int)WXK_F6] = KC_F6;
	keyConverter[(int)WXK_F7] = KC_F7;
	keyConverter[(int)WXK_F8] = KC_F8;
	keyConverter[(int)WXK_F9] = KC_F9;
	keyConverter[(int)WXK_F10] = KC_F10;
	keyConverter[(int)WXK_F11] = KC_F11;
	keyConverter[(int)WXK_F12] = KC_F12;
	keyConverter[(int)WXK_F13] = KC_F13;
	keyConverter[(int)WXK_F14] = KC_F14;
	keyConverter[(int)WXK_F15] = KC_F15;
	keyConverter[(int)WXK_SNAPSHOT] = KC_SYSRQ;
	keyConverter[(int)WXK_SCROLL] = KC_SCROLL;
	keyConverter[(int)WXK_PAUSE] = KC_PAUSE;

	//Number row
	keyConverter[126] = KC_GRAVE;
	keyConverter[96] = KC_GRAVE;
	keyConverter[48] = KC_0;
	keyConverter[49] = KC_1;
	keyConverter[50] = KC_2;
	keyConverter[51] = KC_3;
	keyConverter[52] = KC_4;
	keyConverter[53] = KC_5;
	keyConverter[54] = KC_6;
	keyConverter[55] = KC_7;
	keyConverter[56] = KC_8;
	keyConverter[57] = KC_9;
	keyConverter[45] = KC_MINUS;
	keyConverter[43] = KC_EQUALS;
	keyConverter[61] = KC_EQUALS;
	keyConverter[(int)WXK_BACK] = KC_BACK;

	//QWERTY Row
	keyConverter[(int)WXK_TAB] = KC_TAB;
	keyConverter[91] = KC_LBRACKET;
	keyConverter[93] = KC_RBRACKET;
	keyConverter[92] = KC_BACKSLASH;

	//ASDF Row
	keyConverter[(int)WXK_CAPITAL] = KC_CAPITAL;
	keyConverter[59] = KC_SEMICOLON;
	keyConverter[39] = KC_APOSTROPHE;
	keyConverter[(int)WXK_RETURN] = KC_RETURN;

	//ZXCV Row
	keyConverter[(int)WXK_SHIFT] = KC_LSHIFT;
	keyConverter[44] = KC_COMMA;
	keyConverter[46] = KC_PERIOD;
	keyConverter[47] = KC_SLASH;

	//Spacebar Row
	keyConverter[(int)WXK_CONTROL] = KC_LCONTROL;
	keyConverter[(int)WXK_WINDOWS_LEFT] = KC_LWIN;
	keyConverter[(int)WXK_COMMAND] = KC_LWIN;
	keyConverter[(int)WXK_ALT] = KC_LMENU;
	keyConverter[(int)WXK_SPACE] = KC_SPACE;
	keyConverter[(int)WXK_WINDOWS_RIGHT] = KC_RWIN;

	//Cursor position keys
	keyConverter[(int)WXK_INSERT] = KC_INSERT;
	keyConverter[(int)WXK_HOME] = KC_HOME;
	keyConverter[(int)WXK_PAGEUP] = KC_PGUP;
	keyConverter[(int)WXK_DELETE] = KC_DELETE;
	keyConverter[(int)WXK_END] = KC_END;
	keyConverter[(int)WXK_PAGEDOWN] = KC_PGDOWN;

	//Arrow Keys
	keyConverter[(int)WXK_LEFT] = KC_LEFT;
	keyConverter[(int)WXK_UP] = KC_UP;
	keyConverter[(int)WXK_RIGHT] = KC_RIGHT;
	keyConverter[(int)WXK_DOWN] = KC_DOWN;

	//Numpad numlock on
	keyConverter[(int)WXK_NUMPAD0] = KC_NUMPAD0;
	keyConverter[(int)WXK_NUMPAD1] = KC_NUMPAD1;
	keyConverter[(int)WXK_NUMPAD2] = KC_NUMPAD2;
	keyConverter[(int)WXK_NUMPAD3] = KC_NUMPAD3;
	keyConverter[(int)WXK_NUMPAD4] = KC_NUMPAD4;
	keyConverter[(int)WXK_NUMPAD5] = KC_NUMPAD5;
	keyConverter[(int)WXK_NUMPAD6] = KC_NUMPAD6;
	keyConverter[(int)WXK_NUMPAD7] = KC_NUMPAD7;
	keyConverter[(int)WXK_NUMPAD8] = KC_NUMPAD8;
	keyConverter[(int)WXK_NUMPAD9] = KC_NUMPAD9;
	keyConverter[(int)WXK_NUMPAD_DECIMAL] = KC_DECIMAL;

	//Numpad numlock off
	keyConverter[(int)WXK_NUMPAD_SPACE] = KC_SPACE;
	keyConverter[(int)WXK_NUMPAD_TAB] = KC_TAB;
	keyConverter[(int)WXK_NUMPAD_F1] = KC_F1;
	keyConverter[(int)WXK_NUMPAD_F2] = KC_F2;
	keyConverter[(int)WXK_NUMPAD_F3] = KC_F3;
	keyConverter[(int)WXK_NUMPAD_F4] = KC_F4;
	keyConverter[(int)WXK_NUMPAD_HOME] = KC_HOME;
	keyConverter[(int)WXK_NUMPAD_LEFT] = KC_LEFT;
	keyConverter[(int)WXK_NUMPAD_UP] = KC_UP;
	keyConverter[(int)WXK_NUMPAD_RIGHT] = KC_RIGHT;
	keyConverter[(int)WXK_NUMPAD_DOWN] = KC_DOWN;
	keyConverter[(int)WXK_NUMPAD_PAGEUP] = KC_PGUP;
	keyConverter[(int)WXK_NUMPAD_PAGEDOWN] = KC_PGDOWN;
	keyConverter[(int)WXK_NUMPAD_END] = KC_END;
	keyConverter[(int)WXK_NUMPAD_BEGIN] = KC_HOME;
	keyConverter[(int)WXK_NUMPAD_INSERT] = KC_INSERT;
	keyConverter[(int)WXK_NUMPAD_DELETE] = KC_DELETE;

	//Numpad global
	keyConverter[(int)WXK_NUMLOCK] = KC_NUMLOCK;
	keyConverter[(int)WXK_NUMPAD_ENTER] = KC_NUMPADENTER;
	keyConverter[(int)WXK_NUMPAD_ADD] = KC_ADD;
	keyConverter[(int)WXK_NUMPAD_SUBTRACT] = KC_SUBTRACT;
	keyConverter[(int)WXK_NUMPAD_DIVIDE] = KC_DIVIDE;
	keyConverter[(int)WXK_NUMPAD_MULTIPLY] = KC_MULTIPLY;

	//Letters
	keyConverter[65] = KC_A;
	keyConverter[66] = KC_B;
	keyConverter[67] = KC_C;
	keyConverter[68] = KC_D;
	keyConverter[69] = KC_E;
	keyConverter[70] = KC_F;
	keyConverter[71] = KC_G;
	keyConverter[72] = KC_H;
	keyConverter[73] = KC_I;
	keyConverter[74] = KC_J;
	keyConverter[75] = KC_K;
	keyConverter[76] = KC_L;
	keyConverter[77] = KC_M;
	keyConverter[78] = KC_N;
	keyConverter[79] = KC_O;
	keyConverter[80] = KC_P;
	keyConverter[81] = KC_Q;
	keyConverter[82] = KC_R;
	keyConverter[83] = KC_S;
	keyConverter[84] = KC_T;
	keyConverter[85] = KC_U;
	keyConverter[86] = KC_V;
	keyConverter[87] = KC_W;
	keyConverter[88] = KC_X;
	keyConverter[89] = KC_Y;
	keyConverter[90] = KC_Z;

	/*Unknown
	keyConverter[(int)WXK_DECIMAL]
	keyConverter[(int)WXK_DIVIDE] = KC_DIVIDE;
	keyConverter[(int)WXK_MULTIPLY] = KC_MULTIPLY;
	keyConverter[(int)WXK_SUBTRACT] = KC_SUBTRACT;
	keyConverter[(int)WXK_ADD] = KC_ADD;
	keyConverter[(int)WXK_F16] = KC_F16;
	keyConverter[(int)WXK_F17] = KC_F17;
	keyConverter[(int)WXK_F18] = KC_F18;
	keyConverter[(int)WXK_F19] = KC_F19;
	keyConverter[(int)WXK_F20] = KC_F20;
	keyConverter[(int)WXK_F21] = KC_F21;
	keyConverter[(int)WXK_F22] = KC_F22;
	keyConverter[(int)WXK_F23] = KC_F23;
	keyConverter[(int)WXK_F24] = KC_F24;
	keyConverter[(int)WXK_SPECIAL1] = 
	keyConverter[(int)WXK_SPECIAL2] = 194,
	keyConverter[(int)WXK_SPECIAL3] = 195,
	keyConverter[(int)WXK_SPECIAL4] = 196,
	keyConverter[(int)WXK_SPECIAL5] = 197,
	keyConverter[(int)WXK_SPECIAL6] = 198,
	keyConverter[(int)WXK_SPECIAL7] = 199,
	keyConverter[(int)WXK_SPECIAL8] = 200,
	keyConverter[(int)WXK_SPECIAL9] = 201,
	keyConverter[(int)WXK_SPECIAL10] = 202,
	keyConverter[(int)WXK_SPECIAL11] = 203,
	keyConverter[(int)WXK_SPECIAL12] = 204,
	keyConverter[(int)WXK_SPECIAL13] = 205,
	keyConverter[(int)WXK_SPECIAL14] = 206,
	keyConverter[(int)WXK_SPECIAL15] = 207,
	keyConverter[(int)WXK_SPECIAL16] = 208,
	keyConverter[(int)WXK_SPECIAL17] = 209,
	keyConverter[(int)WXK_SPECIAL18] = 210,
	keyConverter[(int)WXK_SPECIAL19] = 211,
	keyConverter[(int)WXK_SPECIAL20] = 212,
	keyConverter[(int)WXK_START];
	keyConverter[(int)WXK_LBUTTON];
	keyConverter[(int)WXK_RBUTTON];
	keyConverter[(int)WXK_CANCEL];
	keyConverter[(int)WXK_MBUTTON];
	keyConverter[(int)WXK_CLEAR];
	keyConverter[(int)WXK_SELECT];
	keyConverter[(int)WXK_PRINT];
	keyConverter[(int)WXK_EXECUTE];
	keyConverter[(int)WXK_HELP];
	keyConverter[(int)WXK_SEPARATOR];
	keyConverter[(int)WXK_NUMPAD_SEPARATOR];
	keyConverter[(int)WXK_WINDOWS_MENU];*/
}

//PInvoke
extern "C" _AnomalousExport WxKeyboard* WxKeyboard_new(NativeOSWindow* osWindow, WxKeyboard::KeyDownDelegate keyDownCB, WxKeyboard::KeyUpDelegate keyUpCB)
{
	return new WxKeyboard(osWindow, keyDownCB, keyUpCB);
}

extern "C" _AnomalousExport void WxKeyboard_delete(WxKeyboard* keyboard)
{
	delete keyboard;
}