#pragma once

enum NativeDialogResult
{
    YES = 1,
    OK = 2,
    NO = 4,
    CANCEL = 8,
};

typedef void (*NativeDialogResultCallback)(NativeDialogResult result);

//File Open Dialog
typedef void (*FileOpenDialogSetPathString)(String path);
typedef void (*FileOpenDialogResultCallback)(NativeDialogResult result);

//File Save Dialog
typedef void (*FileSaveDialogResultCallback)(NativeDialogResult result, String file);