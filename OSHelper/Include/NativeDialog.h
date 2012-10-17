#pragma once

enum NativeDialogResult
{
    YES = 1,
    NDYES = YES, //Workaround for obj-c YES keyword
    OK = 2,
    NO = 4,
    NDNO = NO, //Workaround for obj-c NO keyword
    CANCEL = 8,
};

class Color
{
public:
	float r, g, b, a;

	Color()
		:r(0.0f),
		g(0.0f),
		b(0.0f),
		a(0.0f)
	{

	}

	Color(byte r, byte g, byte b)
		:r(r / 255.0f),
		g(g / 255.0f),
		b(b / 255.0f),
		a(1.0f)
	{

	}
    
    Color(float r, float g, float b)
    :r(r),
    g(g),
    b(b),
    a(1.0f)
	{
        
	}
};

typedef void (*NativeDialogResultCallback)(NativeDialogResult result);

//File Open Dialog
typedef void (*FileOpenDialogSetPathString)(String path);
typedef void (*FileOpenDialogResultCallback)(NativeDialogResult result);

//File Save Dialog
typedef void (*FileSaveDialogResultCallback)(NativeDialogResult result, String file);

//Dir Dialog
typedef void (*DirDialogResultCallback)(NativeDialogResult result, String file);

//Color Dialog
typedef void (*ColorDialogResultCallback)(NativeDialogResult result, Color color);