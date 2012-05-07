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

        public FileBrowserEditableProperty(String name, MemberWrapper propertyInfo, Object instance, String browserSearchPattern)
            :base(name, propertyInfo, instance)
        {
            this.browserSearchPattern = browserSearchPattern;
        }

        protected override Browser buildBrowser()
        {
            return BrowserWindowController.createFileBrowser(browserSearchPattern);
        }
    }
}
