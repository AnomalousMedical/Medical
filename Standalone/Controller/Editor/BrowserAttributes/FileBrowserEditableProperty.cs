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

        private String prompt;
        private List<String> searchPatterns;
        private Browser.DisplayHint displayHint;

        public FileBrowserEditableProperty(String name, MemberWrapper propertyInfo, Object instance, IEnumerable<String> searchPatterns, String prompt, Browser.DisplayHint displayHint)
            :base(name, propertyInfo, instance)
        {
            this.searchPatterns = new List<String>(searchPatterns);
            this.prompt = prompt;
            this.displayHint = displayHint;
        }

        protected override Browser buildBrowser(EditUICallback uiCallback)
        {
            Browser browser = uiCallback.runSyncCustomQuery<Browser, IEnumerable<String>, String>(CustomQueries.BuildBrowser, searchPatterns, prompt);
            browser.Hint = displayHint;
            return browser;
        }
    }
}
