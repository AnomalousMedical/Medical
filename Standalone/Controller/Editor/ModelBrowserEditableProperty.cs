using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    class ModelBrowserEditableProperty : BrowseableEditableProperty
    {
        private Type assignableFromType;

        public ModelBrowserEditableProperty(String name, MemberWrapper propertyInfo, Object instance, Type assignableFromType)
            :base(name, propertyInfo, instance)
        {
            this.assignableFromType = assignableFromType;
        }

        protected override Browser buildBrowser()
        {
            return BrowserWindowController.createModelBrowser(assignableFromType);
        }
    }
}
