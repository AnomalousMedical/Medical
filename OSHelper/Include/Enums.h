#pragma once

enum NativeDialogResult
{
    YES = 1,
    OK = 2,
    NO = 4,
    CANCEL = 8,
};

NativeDialogResult interpretResults(int resultCode);