using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public delegate void FileChosenCallback(String filename);

    interface ITimelineFileBrowser
    {
        void promptForFile(String filterString, FileChosenCallback callback);
    }
}
