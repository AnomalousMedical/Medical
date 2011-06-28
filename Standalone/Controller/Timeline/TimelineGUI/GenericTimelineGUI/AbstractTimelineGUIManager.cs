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

        private CrossFadeLayoutContainer leftFadeContainer;
        private GUIManager guiManager;
        private List<AbstractTimelineGUI> requestList = new List<AbstractTimelineGUI>();
        private UpdateTimer updateTimer;
        private bool updating = false;

        public AbstractTimelineGUIManager(UpdateTimer updateTimer, GUIManager guiManager)
        {
            if (Instance != null)
            {
                throw new Exception("Only create one instance of the GenericTimelineGUIManager.");
            }
            Instance = this;
            leftFadeContainer = new CrossFadeLayoutContainer(updateTimer);
            this.guiManager = guiManager;
            this.updateTimer = updateTimer;
        }

        public void requestOpen(AbstractTimelineGUI gui)
        {
            gui._RequestClosed = false;
            requestList.Add(gui);
            subscribeToUpdates();
        }

        public void requestClose(AbstractTimelineGUI gui)
        {
            gui._RequestClosed = true;
            requestList.Add(gui);
            subscribeToUpdates();
        }

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            foreach (AbstractTimelineGUI gui in requestList)
            {
                if (gui._RequestClosed)
                {
                    guiManager.changeLeftPanel(null, gui._animationCallback);
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
