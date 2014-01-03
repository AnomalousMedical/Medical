using Engine.Editing;
using Engine.Reflection;
using Medical.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Editor
{
    class ElementNameEditableProperty : BrowseableEditableProperty
    {
        public enum CustomQueries
        {
            BuildBrowser
        }

        public ElementNameEditableProperty(String name, MemberWrapper propertyInfo, Object instance)
            :base(name, propertyInfo, instance)
        {
            DataReadOnly = true;
        }

        protected override Browser buildBrowser(EditUICallback uiCallback)
        {
            return uiCallback.runSyncCustomQuery<Browser>(CustomQueries.BuildBrowser);
        }
    }
}
