using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    public class ActionBrowserEditableProperty : BrowseableEditableProperty
    {
        public enum CustomQueries
        {
            BuildBrowser
        }

        public ActionBrowserEditableProperty(String name, MemberWrapper propertyInfo, Object instance)
            :base(name, propertyInfo, instance)
        {

        }

        protected override Browser buildBrowser(EditUICallback uiCallback)
        {
            return uiCallback.runSyncCustomQuery<Browser>(CustomQueries.BuildBrowser);
        }
    }
}
