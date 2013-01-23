using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.GUI
{
    public interface RmlWysiwygBrowserProvider
    {
        Browser createActionBrowser();

        Browser createFileBrowser(IEnumerable<string> searchPatterns, string prompt);

        Browser createFileBrowser(IEnumerable<string> searchPatterns, String prompt, String leadingPath);
    }
}
