using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class NavigationMenus : IDisposable
    {
        private LinkedList<NavigationMenuEntry> parentEntries = new LinkedList<NavigationMenuEntry>();

        public NavigationMenuEntry addParentEntry(String text)
        {
            NavigationMenuEntry newEntry = new NavigationMenuEntry(text);
            parentEntries.AddLast(newEntry);
            return newEntry;
        }

        internal void addParentEntry(NavigationMenuEntry entry)
        {
            parentEntries.AddLast(entry);
        }

        public void Dispose()
        {
            foreach (NavigationMenuEntry entry in parentEntries)
            {
                entry.Dispose();
            }
        }

        public void removeParentEntry(NavigationMenuEntry entry)
        {
            parentEntries.Remove(entry);
        }

        public NavigationMenuEntry addSubEntry(NavigationMenuEntry parent, String text)
        {
            NavigationMenuEntry newEntry = new NavigationMenuEntry(text);
            parent.addSubEntry(newEntry);
            return newEntry;
        }

        public void removeSubEntry(NavigationMenuEntry parent, NavigationMenuEntry child)
        {
            parent.removeSubEntry(child);
        }

        public void addState(NavigationMenuEntry target, NavigationState state)
        {
            target.addNavigationState(state);
        }

        public void removeState(NavigationMenuEntry target, NavigationState state)
        {
            target.removeNavigationState(state);
        }

        public IEnumerable<NavigationMenuEntry> ParentEntries
        {
            get
            {
                return parentEntries;
            }
        }
    }
}
