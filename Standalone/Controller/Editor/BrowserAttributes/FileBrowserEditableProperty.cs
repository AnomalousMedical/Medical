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

        public FileBrowserEditableProperty(String name, MemberWrapper propertyInfo, Object instance, IEnumerable<String> searchPatterns, String prompt)
            :base(name, propertyInfo, instance)
        {
            this.searchPatterns = new List<String>(searchPatterns);
            this.prompt = prompt;
        }

        protected override Browser buildBrowser(EditUICallback uiCallback)
        {
            return uiCallback.runSyncCustomQuery<Browser, IEnumerable<String>, String>(CustomQueries.BuildBrowser, searchPatterns, prompt);
        }
    }
}
