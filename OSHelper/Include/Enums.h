#pragma once

enum NativeDialogResult
{
    YES = 1,
    OK = 2,
    NO = 4,
    CANCEL = 8,
};

NativeDialogResult interpretResults(int resultCode);

enum CommonMenuItems
{
    New,
    Open, 
    Save,
    SaveAs,
    Exit,
    Preferences,
    Help,
    About,
    AutoAssign = -1
};

int convertMenuItemID(CommonMenuItems id);