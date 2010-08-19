using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    class WxKeyboard : Engine.Platform.Keyboard, IDisposable
    {
        public override event KeyEvent KeyPressed;
        public override event KeyEvent KeyReleased;

        private WxOSWindow window;
        private bool[] keysDown = new bool[256];
        bool altDown = false;
        bool ctrlDown = false;
        bool shiftDown = false;
        private int downKeyCode = 0;

        public WxKeyboard(WxOSWindow window)
        {
            this.window = window;

            window.WxWindow.AddEventListener(wx.Event.wxEVT_KEY_DOWN, OnKeyDown);
            window.WxWindow.AddEventListener(wx.Event.wxEVT_CHAR, OnChar);
            window.WxWindow.AddEventListener(wx.Event.wxEVT_KEY_UP, OnKeyUp);
        }

        public void Dispose()
        {
            window.WxWindow.RemoveListener(OnKeyDown);
            window.WxWindow.RemoveListener(OnChar);
            window.WxWindow.RemoveListener(OnKeyUp);
        }

        public override void capture()
        {
            
        }

        void OnKeyDown(object sender, wx.Event evt)
        {
            wx.KeyEvent kevt = (wx.KeyEvent)evt;
            //If not one of the special case keys allow the Char event to handle the key press.
            downKeyCode = kevt.KeyCode;
            if (downKeyCode < 300)
            {
                evt.Skip();
            }
            else
            {
                KeyboardButtonCode buttonCode = keyConverter[kevt.KeyCode];
                if (!keysDown[(int)buttonCode])
                {
                    keysDown[(int)buttonCode] = true;
                    switch ((wx.KeyCode)kevt.KeyCode)
                    {
                        case wx.KeyCode.WXK_SHIFT:
                            shiftDown = true;
                            break;

                        case wx.KeyCode.WXK_ALT:
                            altDown = true;
                            break;

                        case wx.KeyCode.WXK_CONTROL:
                            ctrlDown = true;
                            break;
                    }
                    uint sendChar = 0;
                    //Check to see if one of the special buttons needs to be translated to a symbol or number
                    switch (buttonCode)
                    {
                        case KeyboardButtonCode.KC_NUMPAD7:
                            sendChar = '7';
                            break;
                        case KeyboardButtonCode.KC_NUMPAD8:
                            sendChar = '8';
                            break;
                        case KeyboardButtonCode.KC_NUMPAD9:
                            sendChar = '9';
                            break;
                        case KeyboardButtonCode.KC_SUBTRACT:
                            sendChar = '-';
                            break;
                        case KeyboardButtonCode.KC_NUMPAD4:
                            sendChar = '4';
                            break;
                        case KeyboardButtonCode.KC_NUMPAD5:
                            sendChar = '5';
                            break;
                        case KeyboardButtonCode.KC_NUMPAD6:
                            sendChar = '6';
                            break;
                        case KeyboardButtonCode.KC_ADD:
                            sendChar = '+';
                            break;
                        case KeyboardButtonCode.KC_NUMPAD1:
                            sendChar = '1';
                            break;
                        case KeyboardButtonCode.KC_NUMPAD2:
                            sendChar = '2';
                            break;
                        case KeyboardButtonCode.KC_NUMPAD3:
                            sendChar = '3';
                            break;
                        case KeyboardButtonCode.KC_NUMPAD0:
                            sendChar = '0';
                            break;
                        case KeyboardButtonCode.KC_DECIMAL:
                            sendChar = '.';
                            break;
                        case KeyboardButtonCode.KC_DIVIDE:
                            sendChar = '/';
                            break;
                        case KeyboardButtonCode.KC_MULTIPLY:
                            sendChar = '*';
                            break;
                    }
                    if (KeyPressed != null)
                    {
                        KeyPressed.Invoke(buttonCode, sendChar);
                    }

                    //Logging.Log.Debug("Down Wx Keycode {0} Internal Keycode {1} Char \'{2}\'", ((wx.KeyCode)kevt.KeyCode).ToString(), keyConverter[kevt.KeyCode].ToString(), sendChar != 0 ? (char)sendChar : ' ');
                }
            }
        }

        void OnChar(object sender, wx.Event evt)
        {
            wx.KeyEvent kevt = (wx.KeyEvent)evt;
            KeyboardButtonCode buttonCode = keyConverter[downKeyCode];
            if (!keysDown[(int)buttonCode])
            {
                keysDown[(int)buttonCode] = true;
                if (KeyPressed != null)
                {
                    KeyPressed.Invoke(buttonCode, kevt.UnicodeChar);
                }

                //Logging.Log.Debug("Char Keycode {0} Internal Keycode {1} Char \'{2}\'", downKeyCode.ToString(), buttonCode, (char)kevt.UnicodeChar);
            }
        }

        void OnKeyUp(object sender, wx.Event evt)
        {
            evt.Skip();
            wx.KeyEvent kevt = (wx.KeyEvent)evt;
            KeyboardButtonCode buttonCode = keyConverter[kevt.KeyCode];
            keysDown[(int)buttonCode] = false;
            switch ((wx.KeyCode)kevt.KeyCode)
            {
                case wx.KeyCode.WXK_SHIFT:
                    shiftDown = false;
                    break;

                case wx.KeyCode.WXK_ALT:
                    altDown = false;
                    break;

                case wx.KeyCode.WXK_CONTROL:
                    ctrlDown = false;
                    break;
            }
            if (KeyReleased != null)
            {
                KeyReleased.Invoke(buttonCode, 0);
            }

            //Logging.Log.Debug("Up Wx Keycode {0} Internal Keycode {1}", ((wx.KeyCode)kevt.KeyCode).ToString(), keyConverter[kevt.KeyCode].ToString());
        }

        public override string getAsString(KeyboardButtonCode code)
        {
            return "";
        }

        public override bool isKeyDown(KeyboardButtonCode keyCode)
        {
            return keysDown[(int)keyCode];
        }

        public override bool isModifierDown(Modifier keyCode)
        {
            switch (keyCode)
            {
                case Modifier.Shift:
                    return shiftDown;
                case Modifier.Alt:
                    return altDown;
                case Modifier.Ctrl:
                    return ctrlDown;
                default:
                    return false;
            }
        }

        private static KeyboardButtonCode[] keyConverter = new KeyboardButtonCode[397];

        static WxKeyboard()
        {
            //Top Row
            keyConverter[(int)wx.KeyCode.WXK_ESCAPE] = KeyboardButtonCode.KC_ESCAPE;
            keyConverter[(int)wx.KeyCode.WXK_F1] = KeyboardButtonCode.KC_F1;
            keyConverter[(int)wx.KeyCode.WXK_F2] = KeyboardButtonCode.KC_F2;
            keyConverter[(int)wx.KeyCode.WXK_F3] = KeyboardButtonCode.KC_F3;
            keyConverter[(int)wx.KeyCode.WXK_F4] = KeyboardButtonCode.KC_F4;
            keyConverter[(int)wx.KeyCode.WXK_F5] = KeyboardButtonCode.KC_F5;
            keyConverter[(int)wx.KeyCode.WXK_F6] = KeyboardButtonCode.KC_F6;
            keyConverter[(int)wx.KeyCode.WXK_F7] = KeyboardButtonCode.KC_F7;
            keyConverter[(int)wx.KeyCode.WXK_F8] = KeyboardButtonCode.KC_F8;
            keyConverter[(int)wx.KeyCode.WXK_F9] = KeyboardButtonCode.KC_F9;
            keyConverter[(int)wx.KeyCode.WXK_F10] = KeyboardButtonCode.KC_F10;
            keyConverter[(int)wx.KeyCode.WXK_F11] = KeyboardButtonCode.KC_F11;
            keyConverter[(int)wx.KeyCode.WXK_F12] = KeyboardButtonCode.KC_F12;
            keyConverter[(int)wx.KeyCode.WXK_F13] = KeyboardButtonCode.KC_F13;
            keyConverter[(int)wx.KeyCode.WXK_F14] = KeyboardButtonCode.KC_F14;
            keyConverter[(int)wx.KeyCode.WXK_F15] = KeyboardButtonCode.KC_F15;
            keyConverter[(int)wx.KeyCode.WXK_SNAPSHOT] = KeyboardButtonCode.KC_SYSRQ;
            keyConverter[(int)wx.KeyCode.WXK_SCROLL] = KeyboardButtonCode.KC_SCROLL;
            keyConverter[(int)wx.KeyCode.WXK_PAUSE] = KeyboardButtonCode.KC_PAUSE;

            //Number row
            keyConverter[126] = KeyboardButtonCode.KC_GRAVE;
            keyConverter[96] = KeyboardButtonCode.KC_GRAVE;
            keyConverter[48] = KeyboardButtonCode.KC_0;
            keyConverter[49] = KeyboardButtonCode.KC_1;
            keyConverter[50] = KeyboardButtonCode.KC_2;
            keyConverter[51] = KeyboardButtonCode.KC_3;
            keyConverter[52] = KeyboardButtonCode.KC_4;
            keyConverter[53] = KeyboardButtonCode.KC_5;
            keyConverter[54] = KeyboardButtonCode.KC_6;
            keyConverter[55] = KeyboardButtonCode.KC_7;
            keyConverter[56] = KeyboardButtonCode.KC_8;
            keyConverter[57] = KeyboardButtonCode.KC_9;
            keyConverter[45] = KeyboardButtonCode.KC_MINUS;
            keyConverter[43] = KeyboardButtonCode.KC_EQUALS;
            keyConverter[61] = KeyboardButtonCode.KC_EQUALS;
            keyConverter[(int)wx.KeyCode.WXK_BACK] = KeyboardButtonCode.KC_BACK;

            //QWERTY Row
            keyConverter[(int)wx.KeyCode.WXK_TAB] = KeyboardButtonCode.KC_TAB;
            keyConverter[91] = KeyboardButtonCode.KC_LBRACKET;
            keyConverter[93] = KeyboardButtonCode.KC_RBRACKET;
            keyConverter[92] = KeyboardButtonCode.KC_BACKSLASH;

            //ASDF Row
            keyConverter[(int)wx.KeyCode.WXK_CAPITAL] = KeyboardButtonCode.KC_CAPITAL;
            keyConverter[59] = KeyboardButtonCode.KC_SEMICOLON;
            keyConverter[39] = KeyboardButtonCode.KC_APOSTROPHE;
            keyConverter[(int)wx.KeyCode.WXK_RETURN] = KeyboardButtonCode.KC_RETURN;

            //ZXCV Row
            keyConverter[(int)wx.KeyCode.WXK_SHIFT] = KeyboardButtonCode.KC_LSHIFT;
            keyConverter[44] = KeyboardButtonCode.KC_COMMA;
            keyConverter[46] = KeyboardButtonCode.KC_PERIOD;
            keyConverter[47] = KeyboardButtonCode.KC_SLASH;

            //Spacebar Row
            keyConverter[(int)wx.KeyCode.WXK_CONTROL] = KeyboardButtonCode.KC_LCONTROL;
            keyConverter[(int)wx.KeyCode.WXK_WINDOWS_LEFT] = KeyboardButtonCode.KC_LWIN;
            keyConverter[(int)wx.KeyCode.WXK_COMMAND] = KeyboardButtonCode.KC_LWIN;
            keyConverter[(int)wx.KeyCode.WXK_ALT] = KeyboardButtonCode.KC_LMENU;
            keyConverter[(int)wx.KeyCode.WXK_SPACE] = KeyboardButtonCode.KC_SPACE;
            keyConverter[(int)wx.KeyCode.WXK_WINDOWS_RIGHT] = KeyboardButtonCode.KC_RWIN;

            //Cursor position keys
            keyConverter[(int)wx.KeyCode.WXK_INSERT] = KeyboardButtonCode.KC_INSERT;
            keyConverter[(int)wx.KeyCode.WXK_HOME] = KeyboardButtonCode.KC_HOME;
            keyConverter[(int)wx.KeyCode.WXK_PAGEUP] = KeyboardButtonCode.KC_PGUP;
            keyConverter[(int)wx.KeyCode.WXK_DELETE] = KeyboardButtonCode.KC_DELETE;
            keyConverter[(int)wx.KeyCode.WXK_END] = KeyboardButtonCode.KC_END;
            keyConverter[(int)wx.KeyCode.WXK_PAGEDOWN] = KeyboardButtonCode.KC_PGDOWN;

            //Arrow Keys
            keyConverter[(int)wx.KeyCode.WXK_LEFT] = KeyboardButtonCode.KC_LEFT;
            keyConverter[(int)wx.KeyCode.WXK_UP] = KeyboardButtonCode.KC_UP;
            keyConverter[(int)wx.KeyCode.WXK_RIGHT] = KeyboardButtonCode.KC_RIGHT;
            keyConverter[(int)wx.KeyCode.WXK_DOWN] = KeyboardButtonCode.KC_DOWN;

            //Numpad numlock on
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD0] = KeyboardButtonCode.KC_NUMPAD0;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD1] = KeyboardButtonCode.KC_NUMPAD1;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD2] = KeyboardButtonCode.KC_NUMPAD2;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD3] = KeyboardButtonCode.KC_NUMPAD3;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD4] = KeyboardButtonCode.KC_NUMPAD4;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD5] = KeyboardButtonCode.KC_NUMPAD5;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD6] = KeyboardButtonCode.KC_NUMPAD6;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD7] = KeyboardButtonCode.KC_NUMPAD7;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD8] = KeyboardButtonCode.KC_NUMPAD8;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD9] = KeyboardButtonCode.KC_NUMPAD9;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_DECIMAL] = KeyboardButtonCode.KC_DECIMAL;

            //Numpad numlock off
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_SPACE] = KeyboardButtonCode.KC_SPACE;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_TAB] = KeyboardButtonCode.KC_TAB;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_F1] = KeyboardButtonCode.KC_F1;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_F2] = KeyboardButtonCode.KC_F2;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_F3] = KeyboardButtonCode.KC_F3;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_F4] = KeyboardButtonCode.KC_F4;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_HOME] = KeyboardButtonCode.KC_HOME;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_LEFT] = KeyboardButtonCode.KC_LEFT;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_UP] = KeyboardButtonCode.KC_UP;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_RIGHT] = KeyboardButtonCode.KC_RIGHT;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_DOWN] = KeyboardButtonCode.KC_DOWN;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_PAGEUP] = KeyboardButtonCode.KC_PGUP;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_PAGEDOWN] = KeyboardButtonCode.KC_PGDOWN;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_END] = KeyboardButtonCode.KC_END;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_BEGIN] = KeyboardButtonCode.KC_HOME;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_INSERT] = KeyboardButtonCode.KC_INSERT;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_DELETE] = KeyboardButtonCode.KC_DELETE;

            //Numpad global
            keyConverter[(int)wx.KeyCode.WXK_NUMLOCK] = KeyboardButtonCode.KC_NUMLOCK;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_ENTER] = KeyboardButtonCode.KC_NUMPADENTER;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_ADD] = KeyboardButtonCode.KC_ADD;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_SUBTRACT] = KeyboardButtonCode.KC_SUBTRACT;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_DIVIDE] = KeyboardButtonCode.KC_DIVIDE;
            keyConverter[(int)wx.KeyCode.WXK_NUMPAD_MULTIPLY] = KeyboardButtonCode.KC_MULTIPLY;

            //Letters
            keyConverter[65] = KeyboardButtonCode.KC_A;
            keyConverter[66] = KeyboardButtonCode.KC_B;
            keyConverter[67] = KeyboardButtonCode.KC_C;
            keyConverter[68] = KeyboardButtonCode.KC_D;
            keyConverter[69] = KeyboardButtonCode.KC_E;
            keyConverter[70] = KeyboardButtonCode.KC_F;
            keyConverter[71] = KeyboardButtonCode.KC_G;
            keyConverter[72] = KeyboardButtonCode.KC_H;
            keyConverter[73] = KeyboardButtonCode.KC_I;
            keyConverter[74] = KeyboardButtonCode.KC_J;
            keyConverter[75] = KeyboardButtonCode.KC_K;
            keyConverter[76] = KeyboardButtonCode.KC_L;
            keyConverter[77] = KeyboardButtonCode.KC_M;
            keyConverter[78] = KeyboardButtonCode.KC_N;
            keyConverter[79] = KeyboardButtonCode.KC_O;
            keyConverter[80] = KeyboardButtonCode.KC_P;
            keyConverter[81] = KeyboardButtonCode.KC_Q;
            keyConverter[82] = KeyboardButtonCode.KC_R;
            keyConverter[83] = KeyboardButtonCode.KC_S;
            keyConverter[84] = KeyboardButtonCode.KC_T;
            keyConverter[85] = KeyboardButtonCode.KC_U;
            keyConverter[86] = KeyboardButtonCode.KC_V;
            keyConverter[87] = KeyboardButtonCode.KC_W;
            keyConverter[88] = KeyboardButtonCode.KC_X;
            keyConverter[89] = KeyboardButtonCode.KC_Y;
            keyConverter[90] = KeyboardButtonCode.KC_Z;
            
            //Unknown
            //keyConverter[(int)wx.KeyCode.WXK_DECIMAL]
            //keyConverter[(int)wx.KeyCode.WXK_DIVIDE] = KeyboardButtonCode.KC_DIVIDE;
            //keyConverter[(int)wx.KeyCode.WXK_MULTIPLY] = KeyboardButtonCode.KC_MULTIPLY;
            //keyConverter[(int)wx.KeyCode.WXK_SUBTRACT] = KeyboardButtonCode.KC_SUBTRACT;
            //keyConverter[(int)wx.KeyCode.WXK_ADD] = KeyboardButtonCode.KC_ADD;
            //keyConverter[(int)wx.KeyCode.WXK_F16] = KeyboardButtonCode.KC_F16;
            //keyConverter[(int)wx.KeyCode.WXK_F17] = KeyboardButtonCode.KC_F17;
            //keyConverter[(int)wx.KeyCode.WXK_F18] = KeyboardButtonCode.KC_F18;
            //keyConverter[(int)wx.KeyCode.WXK_F19] = KeyboardButtonCode.KC_F19;
            //keyConverter[(int)wx.KeyCode.WXK_F20] = KeyboardButtonCode.KC_F20;
            //keyConverter[(int)wx.KeyCode.WXK_F21] = KeyboardButtonCode.KC_F21;
            //keyConverter[(int)wx.KeyCode.WXK_F22] = KeyboardButtonCode.KC_F22;
            //keyConverter[(int)wx.KeyCode.WXK_F23] = KeyboardButtonCode.KC_F23;
            //keyConverter[(int)wx.KeyCode.WXK_F24] = KeyboardButtonCode.KC_F24;
            //keyConverter[(int)WXK_SPECIAL1] = 
            //keyConverter[(int)WXK_SPECIAL2] = 194,
            //keyConverter[(int)WXK_SPECIAL3] = 195,
            //keyConverter[(int)WXK_SPECIAL4] = 196,
            //keyConverter[(int)WXK_SPECIAL5] = 197,
            //keyConverter[(int)WXK_SPECIAL6] = 198,
            //keyConverter[(int)WXK_SPECIAL7] = 199,
            //keyConverter[(int)WXK_SPECIAL8] = 200,
            //keyConverter[(int)WXK_SPECIAL9] = 201,
            //keyConverter[(int)WXK_SPECIAL10] = 202,
            //keyConverter[(int)WXK_SPECIAL11] = 203,
            //keyConverter[(int)WXK_SPECIAL12] = 204,
            //keyConverter[(int)WXK_SPECIAL13] = 205,
            //keyConverter[(int)WXK_SPECIAL14] = 206,
            //keyConverter[(int)WXK_SPECIAL15] = 207,
            //keyConverter[(int)WXK_SPECIAL16] = 208,
            //keyConverter[(int)WXK_SPECIAL17] = 209,
            //keyConverter[(int)WXK_SPECIAL18] = 210,
            //keyConverter[(int)WXK_SPECIAL19] = 211,
            //keyConverter[(int)WXK_SPECIAL20] = 212,
            //keyConverter[(int)WXK_START];
            //keyConverter[(int)WXK_LBUTTON];
            //keyConverter[(int)WXK_RBUTTON];
            //keyConverter[(int)WXK_CANCEL];
            //keyConverter[(int)WXK_MBUTTON];
            //keyConverter[(int)WXK_CLEAR];
            //keyConverter[(int)wx.KeyCode.WXK_SELECT];
            //keyConverter[(int)wx.KeyCode.WXK_PRINT];
            //keyConverter[(int)wx.KeyCode.WXK_EXECUTE];
            //keyConverter[(int)wx.KeyCode.WXK_HELP];
            //keyConverter[(int)wx.KeyCode.WXK_SEPARATOR];
            //keyConverter[(int)wx.KeyCode.WXK_NUMPAD_SEPARATOR];
            //keyConverter[(int)wx.KeyCode.WXK_WINDOWS_MENU];
        }
    }
}
