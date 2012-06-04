using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public class StoredMDILayoutContainer
    {
        class WindowEntry
        {
            public WindowEntry(MDIWindow window, WindowAlignment alignment)
            {
                this.Window = window;
                this.Alignment = alignment;
            }

            public MDIWindow Window { get; set; }

            public WindowAlignment Alignment { get; set; }
        }

        private List<WindowEntry> storedWindows = new List<WindowEntry>();
        private WindowAlignment currentAlignment;

        public StoredMDILayoutContainer(WindowAlignment startingAlignment)
        {
            currentAlignment = startingAlignment;
        }

        public void addMDIWindow(MDIWindow window)
        {
            storedWindows.Add(new WindowEntry(window, currentAlignment));
        }

        internal void restoreWindows()
        {
            if (storedWindows.Count > 0)
            {
                WindowEntry previousWindow = storedWindows[0];
                previousWindow.Window.Visible = true;
                for (int i = 1; i < storedWindows.Count; ++i)
                {
                    storedWindows[i].Window.restoreToMDILayout(previousWindow.Window, previousWindow.Alignment);
                    previousWindow = storedWindows[i];
                }
            }
        }

        public WindowAlignment CurrentAlignment
        {
            get
            {
                return currentAlignment;
            }
            set
            {
                currentAlignment = value;
            }
        }
    }
}
