#define KEY_MAP_MAX 256

//This is a map of windows virtual keys to engine keys
KeyboardButtonCode keyMap[KEY_MAP_MAX] = 
{
//0
KC_UNASSIGNED,
//#define VK_LBUTTON        0x01
KC_UNASSIGNED,
//#define VK_RBUTTON        0x02
KC_UNASSIGNED,
//#define VK_CANCEL         0x03
KC_UNASSIGNED,
//#define VK_MBUTTON        0x04    /* NOT contiguous with L & RBUTTON */
KC_UNASSIGNED,
//#define VK_XBUTTON1       0x05    /* NOT contiguous with L & RBUTTON */
KC_UNASSIGNED,
//#define VK_XBUTTON2       0x06    /* NOT contiguous with L & RBUTTON */
KC_UNASSIGNED,
// * 0x07 : unassigned
KC_UNASSIGNED,
//#define VK_BACK           0x08
KC_BACK,
//#define VK_TAB            0x09
KC_TAB,
// * 0x0A - 0x0B : reserved
KC_UNASSIGNED,
KC_UNASSIGNED,
//#define VK_CLEAR          0x0C
KC_UNASSIGNED,
//#define VK_RETURN         0x0D
KC_RETURN,
// * 0x0E - 0x0F : reserved
KC_UNASSIGNED,
KC_UNASSIGNED,
//#define VK_SHIFT          0x10
KC_LSHIFT,
//#define VK_CONTROL        0x11
KC_LCONTROL,
//#define VK_MENU           0x12
KC_LMENU,
//#define VK_PAUSE          0x13
KC_PAUSE,
//#define VK_CAPITAL        0x14
KC_CAPITAL,
//#define VK_KANA           0x15
//#define VK_HANGEUL        0x15  /* old name - should be here for compatibility */
//#define VK_HANGUL         0x15
KC_UNASSIGNED,
// 0x16 Undefined
KC_UNASSIGNED,
//#define VK_JUNJA          0x17
KC_UNASSIGNED,
//#define VK_FINAL          0x18
KC_UNASSIGNED,
//#define VK_HANJA          0x19
//#define VK_KANJI          0x19
KC_UNASSIGNED,
// 0x1a Undefined
KC_UNASSIGNED,
//#define VK_ESCAPE         0x1B
KC_ESCAPE,
//#define VK_CONVERT        0x1C
KC_UNASSIGNED,
//#define VK_NONCONVERT     0x1D
KC_UNASSIGNED,
//#define VK_ACCEPT         0x1E
KC_UNASSIGNED,
//#define VK_MODECHANGE     0x1F
KC_UNASSIGNED,
//#define VK_SPACE          0x20
KC_SPACE,
//#define VK_PRIOR          0x21
KC_PGUP,
//#define VK_NEXT           0x22
KC_PGDOWN,
//#define VK_END            0x23
KC_END,
//#define VK_HOME           0x24
KC_HOME,
//#define VK_LEFT           0x25
KC_LEFT,
//#define VK_UP             0x26
KC_UP,
//#define VK_RIGHT          0x27
KC_RIGHT,
//#define VK_DOWN           0x28
KC_DOWN,
//#define VK_SELECT         0x29
KC_UNASSIGNED,
//#define VK_PRINT          0x2A
KC_UNASSIGNED,
//#define VK_EXECUTE        0x2B
KC_UNASSIGNED,
//#define VK_SNAPSHOT       0x2C
KC_UNASSIGNED,
//#define VK_INSERT         0x2D
KC_INSERT,
//#define VK_DELETE         0x2E
KC_DELETE,
//#define VK_HELP           0x2F
KC_UNASSIGNED,
// * VK_0 - VK_9 are the same as ASCII '0' - '9' (0x30 - 0x39)
KC_0,
KC_1,
KC_2,
KC_3,
KC_4,
KC_5,
KC_6,
KC_7,
KC_8,
KC_9,
// * 3a - 0x40 : unassigned
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
// * VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)
KC_A,
KC_B,
KC_C,
KC_D,
KC_E,
KC_F,
KC_G,
KC_H,
KC_I,
KC_J,
KC_K,
KC_L,
KC_M,
KC_N,
KC_O,
KC_P,
KC_Q,
KC_R,
KC_S,
KC_T,
KC_U,
KC_V,
KC_W,
KC_X,
KC_Y,
KC_Z,
//#define VK_LWIN           0x5B
KC_LWIN,
//#define VK_RWIN           0x5C
KC_RWIN,
//#define VK_APPS           0x5D
KC_APPS,
// * 0x5E : reserved
KC_UNASSIGNED,
//#define VK_SLEEP          0x5F
KC_SLEEP,
//#define VK_NUMPAD0        0x60
KC_NUMPAD0,
//#define VK_NUMPAD1        0x61
KC_NUMPAD1,
//#define VK_NUMPAD2        0x62
KC_NUMPAD2,
//#define VK_NUMPAD3        0x63
KC_NUMPAD3,
//#define VK_NUMPAD4        0x64
KC_NUMPAD4,
//#define VK_NUMPAD5        0x65
KC_NUMPAD5,
//#define VK_NUMPAD6        0x66
KC_NUMPAD6,
//#define VK_NUMPAD7        0x67
KC_NUMPAD7,
//#define VK_NUMPAD8        0x68
KC_NUMPAD8,
//#define VK_NUMPAD9        0x69
KC_NUMPAD9,
//#define VK_MULTIPLY       0x6A
KC_MULTIPLY,
//#define VK_ADD            0x6B
KC_ADD,
//#define VK_SEPARATOR      0x6C
KC_UNASSIGNED,
//#define VK_SUBTRACT       0x6D
KC_SUBTRACT,
//#define VK_DECIMAL        0x6E
KC_DECIMAL,
//#define VK_DIVIDE         0x6F
KC_DIVIDE,
//#define VK_F1             0x70
KC_F1,
//#define VK_F2             0x71
KC_F2,
//#define VK_F3             0x72
KC_F3,
//#define VK_F4             0x73
KC_F4,
//#define VK_F5             0x74
KC_F5,
//#define VK_F6             0x75
KC_F6,
//#define VK_F7             0x76
KC_F7,
//#define VK_F8             0x77
KC_F8,
//#define VK_F9             0x78
KC_F9,
//#define VK_F10            0x79
KC_F10,
//#define VK_F11            0x7A
KC_UNASSIGNED,
//#define VK_F12            0x7B
KC_UNASSIGNED,
//#define VK_F13            0x7C
KC_UNASSIGNED,
//#define VK_F14            0x7D
KC_UNASSIGNED,
//#define VK_F15            0x7E
KC_UNASSIGNED,
//#define VK_F16            0x7F
KC_UNASSIGNED,
//#define VK_F17            0x80
KC_UNASSIGNED,
//#define VK_F18            0x81
KC_UNASSIGNED,
//#define VK_F19            0x82
KC_UNASSIGNED,
//#define VK_F20            0x83
KC_UNASSIGNED,
//#define VK_F21            0x84
KC_UNASSIGNED,
//#define VK_F22            0x85
KC_UNASSIGNED,
//#define VK_F23            0x86
KC_UNASSIGNED,
//#define VK_F24            0x87
KC_UNASSIGNED,
// * 0x88 - 0x8F : unassigned
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
//#define VK_NUMLOCK        0x90
KC_NUMLOCK,
//#define VK_SCROLL         0x91
KC_SCROLL,
//#define VK_OEM_NEC_EQUAL  0x92 - 0x96 OEM Specific
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
// * 0x97 - 0x9F : unassigned
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
//#define VK_LSHIFT         0xA0
KC_LSHIFT,
//#define VK_RSHIFT         0xA1
KC_RSHIFT,
//#define VK_LCONTROL       0xA2
KC_LCONTROL,
//#define VK_RCONTROL       0xA3
KC_RCONTROL,
//#define VK_LMENU          0xA4
KC_LMENU,
//#define VK_RMENU          0xA5
KC_RMENU,
//#define VK_BROWSER_BACK        0xA6
KC_WEBBACK,
//#define VK_BROWSER_FORWARD     0xA7
KC_WEBFORWARD,
//#define VK_BROWSER_REFRESH     0xA8
KC_WEBREFRESH,
//#define VK_BROWSER_STOP        0xA9
KC_WEBSTOP,
//#define VK_BROWSER_SEARCH      0xAA
KC_WEBSEARCH,
//#define VK_BROWSER_FAVORITES   0xAB
KC_WEBFAVORITES,
//#define VK_BROWSER_HOME        0xAC
KC_UNASSIGNED,
//#define VK_VOLUME_MUTE         0xAD
KC_MUTE,
//#define VK_VOLUME_DOWN         0xAE
KC_VOLUMEDOWN,
//#define VK_VOLUME_UP           0xAF
KC_VOLUMEUP,
//#define VK_MEDIA_NEXT_TRACK    0xB0
KC_NEXTTRACK,
//#define VK_MEDIA_PREV_TRACK    0xB1
KC_PREVTRACK,
//#define VK_MEDIA_STOP          0xB2
KC_MEDIASTOP,
//#define VK_MEDIA_PLAY_PAUSE    0xB3
KC_PLAYPAUSE,
//#define VK_LAUNCH_MAIL         0xB4
KC_MAIL,
//#define VK_LAUNCH_MEDIA_SELECT 0xB5
KC_MEDIASELECT,
//#define VK_LAUNCH_APP1         0xB6
KC_UNASSIGNED,
//#define VK_LAUNCH_APP2         0xB7
KC_UNASSIGNED,
// * 0xB8 - 0xB9 : reserved
KC_UNASSIGNED,
KC_UNASSIGNED,
//#define VK_OEM_1          0xBA   // ';:' for US
KC_SEMICOLON,
//#define VK_OEM_PLUS       0xBB   // '+' any country
KC_EQUALS,
//#define VK_OEM_COMMA      0xBC   // ',' any country
KC_COMMA,
//#define VK_OEM_MINUS      0xBD   // '-' any country
KC_MINUS,
//#define VK_OEM_PERIOD     0xBE   // '.' any country
KC_PERIOD,
//#define VK_OEM_2          0xBF   // '/?' for US
KC_SLASH,
//#define VK_OEM_3          0xC0   // '`~' for US
KC_GRAVE,
// * 0xC1 - 0xD7 : reserved
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
// * 0xD8 - 0xDA : unassigned
KC_UNASSIGNED,
KC_UNASSIGNED,
KC_UNASSIGNED,
//#define VK_OEM_4          0xDB  //  '[{' for US
KC_LBRACKET,
//#define VK_OEM_5          0xDC  //  '\|' for US
KC_BACKSLASH,
//#define VK_OEM_6          0xDD  //  ']}' for US
KC_RBRACKET,
//#define VK_OEM_7          0xDE  //  ''"' for US
KC_APOSTROPHE,
//#define VK_OEM_8          0xDF
KC_UNASSIGNED,
// * 0xE0 : reserved
KC_UNASSIGNED,
//#define VK_OEM_AX         0xE1  //  'AX' key on Japanese AX kbd
KC_UNASSIGNED,
//#define VK_OEM_102        0xE2  //  "<>" or "\|" on RT 102-key kbd.
KC_UNASSIGNED,
//#define VK_ICO_HELP       0xE3  //  Help key on ICO
KC_UNASSIGNED,
//#define VK_ICO_00         0xE4  //  00 key on ICO
KC_UNASSIGNED,
//#define VK_PROCESSKEY     0xE5
KC_UNASSIGNED,
//#define VK_ICO_CLEAR      0xE6
KC_UNASSIGNED,
//#define VK_PACKET         0xE7
KC_UNASSIGNED,
// * 0xE8 : unassigned
KC_UNASSIGNED,
//#define VK_OEM_RESET      0xE9
KC_UNASSIGNED,
//#define VK_OEM_JUMP       0xEA
KC_UNASSIGNED,
//#define VK_OEM_PA1        0xEB
KC_UNASSIGNED,
//#define VK_OEM_PA2        0xEC
KC_UNASSIGNED,
//#define VK_OEM_PA3        0xED
KC_UNASSIGNED,
//#define VK_OEM_WSCTRL     0xEE
KC_UNASSIGNED,
//#define VK_OEM_CUSEL      0xEF
KC_UNASSIGNED,
//#define VK_OEM_ATTN       0xF0
KC_UNASSIGNED,
//#define VK_OEM_FINISH     0xF1
KC_UNASSIGNED,
//#define VK_OEM_COPY       0xF2
KC_UNASSIGNED,
//#define VK_OEM_AUTO       0xF3
KC_UNASSIGNED,
//#define VK_OEM_ENLW       0xF4
KC_UNASSIGNED,
//#define VK_OEM_BACKTAB    0xF5
KC_UNASSIGNED,
//#define VK_ATTN           0xF6
KC_UNASSIGNED,
//#define VK_CRSEL          0xF7
KC_UNASSIGNED,
//#define VK_EXSEL          0xF8
KC_UNASSIGNED,
//#define VK_EREOF          0xF9
KC_UNASSIGNED,
//#define VK_PLAY           0xFA
KC_UNASSIGNED,
//#define VK_ZOOM           0xFB
KC_UNASSIGNED,
//#define VK_NONAME         0xFC
KC_UNASSIGNED,
//#define VK_PA1            0xFD
KC_UNASSIGNED,
//#define VK_OEM_CLEAR      0xFE
KC_UNASSIGNED
};