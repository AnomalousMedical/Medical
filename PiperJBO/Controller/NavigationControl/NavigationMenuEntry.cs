using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Medical
{
    public class NavigationMenuEntry : IDisposable
    {
        private String state;
        private LinkedList<NavigationMenuEntry> subEntries;
        private Bitmap thumbnail;

        internal NavigationMenuEntry(String text)
        {
            this.Text = text;
        }

        public void Dispose()
        {
            if (subEntries != null)
            {
                foreach (NavigationMenuEntry entry in subEntries)
                {
                    entry.Dispose();
                }
            }
            if (thumbnail != null)
            {
                thumbnail.Dispose();
            }
        }

        internal NavigationMenuEntry addNavigationState(NavigationState state)
        {
            NavigationMenuEntry stateEntry = new NavigationMenuEntry(state.Name);
            stateEntry.NavigationState = state.Name;
            addSubEntry(stateEntry);
            return stateEntry;
        }

        internal void removeNavigationState(NavigationState state)
        {
            NavigationMenuEntry remove = null;
            foreach (NavigationMenuEntry entry in subEntries)
            {
                if (entry.NavigationState == state.Name)
                {
                    remove = entry;
                    break;
                }
            }
            if (remove != null)
            {
                removeSubEntry(remove);
            }
        }

        internal void addSubEntry(NavigationMenuEntry subEntry)
        {
            if (subEntries == null)
            {
                subEntries = new LinkedList<NavigationMenuEntry>();
            }
            subEntries.AddLast(subEntry);
        }

        internal void removeSubEntry(NavigationMenuEntry subEntry)
        {
            if (subEntries != null)
            {
                subEntries.Remove(subEntry);
            }
        }

        public String Text { get; set; }

        public String LayerState { get; set; }

        /// <summary>
        /// Set the thumbnail of this entry. The image is not copied, but will
        /// be disposed by this class. If a copy needs to be made make before
        /// setting this property.
        /// </summary>
        public Bitmap Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                if (thumbnail != null)
                {
                    thumbnail.Dispose();
                }
                thumbnail = value;
            }
        }

        public IEnumerable<NavigationMenuEntry> SubEntries
        {
            get
            {
                return subEntries;
            }
        }

        public String NavigationState
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }
    }
}
