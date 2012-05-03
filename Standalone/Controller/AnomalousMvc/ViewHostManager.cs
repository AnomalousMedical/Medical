using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.GUI;

namespace Medical.Controller.AnomalousMvc
{
    class ViewHostManager : UpdateListener
    {
        private UpdateTimer updateTimer;
        private GUIManager guiManager;
        private List<ViewHost> requestList = new List<ViewHost>();
        private bool updating = false;

        public ViewHostManager(UpdateTimer updateTimer, GUIManager guiManager)
        {
            this.updateTimer = updateTimer;
            this.guiManager = guiManager;
        }

        public void requestOpen(ViewHost viewHost)
        {
            viewHost._RequestClosed = false;
            requestList.Add(viewHost);
            subscribeToUpdates();
        }

        public void requestClose(ViewHost viewHost)
        {
            viewHost._RequestClosed = true;
            requestList.Add(viewHost);
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
            ViewHost gui;
            LayoutContainer nextContainer;
            ViewHost currentGui;
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
