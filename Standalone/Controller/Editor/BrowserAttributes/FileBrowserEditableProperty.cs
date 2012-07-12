using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    public class FileBrowserEditableProperty : BrowseableEditableProperty
    {
        public enum CustomQueries
        {
            BuildBrowser
        }

        private String browserSearchPattern;
        private String prompt;

        public FileBrowserEditableProperty(String name, MemberWrapper propertyInfo, Object instance, String browserSearchPattern, String prompt)
            :base(name, propertyInfo, instance)
        {
            this.browserSearchPattern = browserSearchPattern;
            this.prompt = prompt;
        }

        protected override Browser buildBrowser(EditUICallback uiCallback)
        {
            return uiCallback.runSyncCustomQuery<Browser, String, String>(CustomQueries.BuildBrowser, browserSearchPattern, prompt);
        }
    }
}
