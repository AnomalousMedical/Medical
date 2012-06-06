using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    class FileBrowserEditableProperty : BrowseableEditableProperty
    {
        private String browserSearchPattern;
        private String prompt;

        public FileBrowserEditableProperty(String name, MemberWrapper propertyInfo, Object instance, String browserSearchPattern, String prompt)
            :base(name, propertyInfo, instance)
        {
            this.browserSearchPattern = browserSearchPattern;
            this.prompt = prompt;
        }

        protected override Browser buildBrowser()
        {
            return BrowserWindowController.createFileBrowser(browserSearchPattern, prompt);
        }
    }
}
