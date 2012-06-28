using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    class ViewBrowserEditableProperty : BrowseableEditableProperty
    {
        public ViewBrowserEditableProperty(String name, MemberWrapper propertyInfo, Object instance)
            :base(name, propertyInfo, instance)
        {

        }

        protected override Browser buildBrowser(EditUICallback uiCallback)
        {
            return BrowserWindowController.createViewBrowser();
        }
    }
}
