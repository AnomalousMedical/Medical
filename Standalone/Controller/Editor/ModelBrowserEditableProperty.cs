using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    public class ModelBrowserEditableProperty : BrowseableEditableProperty
    {
        public enum CustomQueries
        {
            BuildBrowser
        }

        private Type assignableFromType;

        public ModelBrowserEditableProperty(String name, MemberWrapper propertyInfo, Object instance, Type assignableFromType)
            :base(name, propertyInfo, instance)
        {
            this.assignableFromType = assignableFromType;
        }

        protected override Browser buildBrowser(EditUICallback uiCallback)
        {
            return uiCallback.runSyncCustomQuery<Browser, Type>(CustomQueries.BuildBrowser, assignableFromType);
        }
    }
}
