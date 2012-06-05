﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    class WindowEntry
    {
        public WindowEntry(MDIWindow window, WindowAlignment alignment, MDIWindow previous = null)
        {
            this.Window = window;
            this.Alignment = alignment;
            this.PreviousWindow = previous;
        }

        public MDIWindow Window { get; set; }

        public MDIWindow PreviousWindow { get; set; }

        public WindowAlignment Alignment { get; set; }
    }

    public class StoredMDILayoutContainer
    {
        class LevelInfo
        {
            public LevelInfo(WindowAlignment alignment, MDIWindow previousWindow = null)
            {
                this.Alignment = alignment;
                this.PreviousWindow = previousWindow;
            }

            public WindowAlignment Alignment { get; set; }

            public MDIWindow PreviousWindow { get; set; }
        }

        private List<WindowEntry> storedWindows = new List<WindowEntry>();
        private LevelInfo currentLevelInfo = null;
        private Stack<LevelInfo> levelInfoStack = new Stack<LevelInfo>();

        public StoredMDILayoutContainer()
        {
            
        }

        internal void addWindowEntry(WindowEntry windowEntry)
        {
            Logging.Log.Debug("Added window entry, type {0}, alignment {1}, parent {2}", windowEntry.Window.GetType(), windowEntry.Alignment, windowEntry.PreviousWindow != null ? windowEntry.PreviousWindow.GetType().Name : "NULL");
            storedWindows.Add(windowEntry);
        }

        public void addMDIWindow(MDIWindow window)
        {
            WindowEntry entry = new WindowEntry(window, currentLevelInfo.Alignment);
            entry.PreviousWindow = currentLevelInfo.PreviousWindow;
            storedWindows.Add(entry);
            currentLevelInfo.PreviousWindow = window;
        }

        public void startLevel(WindowAlignment alignment, MDIWindow firstParent)
        {
            if (currentLevelInfo != null)
            {
                levelInfoStack.Push(currentLevelInfo);
                currentLevelInfo = new LevelInfo(alignment, currentLevelInfo.PreviousWindow);
            }
            else
            {
                currentLevelInfo = new LevelInfo(alignment);
            }
        }

        public void endLevel()
        {
            if (levelInfoStack.Count > 0)
            {
                currentLevelInfo = levelInfoStack.Pop();
            }
        }

        internal void restoreWindows()
        {
            foreach (WindowEntry entry in storedWindows)
            {
                if (entry.PreviousWindow == null)
                {
                    entry.Window.Visible = true;
                }
                else
                {
                    entry.Window.restoreToMDILayout(entry.PreviousWindow, entry.Alignment);
                }
            }
        }
    }
}
