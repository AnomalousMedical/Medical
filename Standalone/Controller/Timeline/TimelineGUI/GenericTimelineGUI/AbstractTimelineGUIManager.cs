using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.GUI;

namespace Medical
{
    class AbstractTimelineGUIManager : UpdateListener
    {
        public static AbstractTimelineGUIManager Instance { get; private set; }

        private GUIManager guiManager;
        private List<AbstractTimelineGUI> requestList = new List<AbstractTimelineGUI>();
        private UpdateTimer updateTimer;
        private bool updating = false;
        private TimelineNavigationBar navigationBar = null;

        public AbstractTimelineGUIManager(UpdateTimer updateTimer, GUIManager guiManager)
        {
            if (Instance != null)
            {
                throw new Exception("Only create one instance of the AbstractTimelineGUIManager.");
            }
            Instance = this;
            this.guiManager = guiManager;
            this.updateTimer = updateTimer;
        }

        public void requestOpen(AbstractTimelineGUI gui)
        {
            gui._RequestClosed = false;
            requestList.Add(gui);
            subscribeToUpdates();
            if (navigationBar != null)
            {
                navigationBar.activeGUIChanged(gui);
            }
        }

        public void requestClose(AbstractTimelineGUI gui)
        {
            gui._RequestClosed = true;
            requestList.Add(gui);
            subscribeToUpdates();
        }

        public void showNavigationBar()
        {
            if (navigationBar == null)
            {
                navigationBar = new TimelineNavigationBar();
            }
            guiManager.changeTopPanel(navigationBar.Layout, null);
        }

        public void hideNavigationBar()
        {
            if (navigationBar != null)
            {
                guiManager.changeTopPanel(null, delegate(LayoutContainer oldChild)
                {
                    navigationBar.Dispose();
                    navigationBar = null;
                });
            }
        }

        public void addToNavigationBar(String timeline, String text, String imageKey)
        {
            if (navigationBar == null)
            {
                navigationBar = new TimelineNavigationBar();
            }
            navigationBar.addPanel(timeline, text, imageKey);
        }

        public void clearNavigationBar()
        {
            if (navigationBar != null)
            {
                navigationBar.clearPanels();
            }
        }

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        /// <summary>
        /// This class will not actually commit its interface changes until the
        /// next frame tick. This way all requests can be bundled up and
        /// processed at once.
        /// </summary>
        /// <param name="clock"></param>
        public void sendUpdate(Clock clock)
        {
            AbstractTimelineGUI gui;
            LayoutContainer nextContainer;
            AbstractTimelineGUI currentGui;
            for (int i = 0; i < requestList.Count; ++i)
            {
                gui = requestList[i];
                if (gui._RequestClosed)
                {
                    //Read ahead and find any other guis that want to be open.
                    nextContainer = null;
                    for (int j = i; j < requestList.Count && nextContainer == null; ++j)
                    {
                        currentGui = requestList[j];
                        if (!currentGui._RequestClosed)
                        {
                            //Found another gui that wants to be open. Store it and open it, also remove it from the request list.
                            nextContainer = currentGui.Container;
                            requestList.Remove(currentGui);
                        }
                    }
                    guiManager.changeLeftPanel(nextContainer, gui._animationCallback);
                }
                else
                {
                    guiManager.changeLeftPanel(gui.Container);
                }
            }
            requestList.Clear();
            unsubscribeFromUpdates();
        }

        private void subscribeToUpdates()
        {
            if (!updating)
            {
                updateTimer.addFixedUpdateListener(this);
                updating = true;
            }
        }

        private void unsubscribeFromUpdates()
        {
            if (updating)
            {
                updateTimer.removeFixedUpdateListener(this);
                updating = false;
            }
        }
    }
}
