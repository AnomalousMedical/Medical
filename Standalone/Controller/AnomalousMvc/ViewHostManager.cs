﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.GUI;
using Engine.Attributes;
using Engine;

namespace Medical.Controller.AnomalousMvc
{
    [SingleEnum]
    public enum ViewLocations
    {
        Left,
        Right,
        Top,
        Bottom,
        Floating,
    }

    class ViewHostManager : IDisposable
    {
        private GUIManager guiManager;
        private ViewHostFactory viewHostFactory;

        private List<KeyValuePair<View, AnomalousMvcContext>> queuedFloatingViews = new List<KeyValuePair<View, AnomalousMvcContext>>();
        private List<ViewHost> openFloatingViews = new List<ViewHost>();
        private List<ViewHost> closingFloatingViews = new List<ViewHost>();
        private List<ViewHostPanelInfo> openPanels = new List<ViewHostPanelInfo>();
        private RemovableListIterator<ViewHostPanelInfo> openPanelsEnumerator;

        public ViewHostManager(GUIManager guiManager, ViewHostFactory viewHostFactory)
        {
            this.guiManager = guiManager;
            this.viewHostFactory = viewHostFactory;
            openPanelsEnumerator = new RemovableListIterator<ViewHostPanelInfo>(openPanels);
        }

        public void Dispose()
        {
            foreach (ViewHost viewHost in closingFloatingViews)
            {
                viewHost._animationCallback(null);
            }
        }

        public void requestOpen(View view, AnomalousMvcContext context)
        {
            if (view.IsWindow)
            {
                queuedFloatingViews.Add(new KeyValuePair<View, AnomalousMvcContext>(view, context));
            }
            else
            {
                //Temp
                ViewHostPanelInfo panel = null;
                switch (view.ViewLocation)
                {
                    case ViewLocations.Left:
                        panel = findPanel(BorderPanelNames.Left, BorderPanelSets.Outer);
                        break;
                    case ViewLocations.Right:
                        panel = findPanel(BorderPanelNames.Right, BorderPanelSets.Outer);
                        break;
                    case ViewLocations.Top:
                        panel = findPanel(BorderPanelNames.Top, BorderPanelSets.Outer);
                        break;
                    case ViewLocations.Bottom:
                        panel = findPanel(BorderPanelNames.Bottom, BorderPanelSets.Outer);
                        break;
                    case ViewLocations.Floating:
                        queuedFloatingViews.Add(new KeyValuePair<View, AnomalousMvcContext>(view, context));
                        break;
                }
                if (panel != null)
                {
                    panel.Queued = view;
                    panel.QueuedContext = context;
                }
            }
        }

        public void requestClose(ViewHost viewHost)
        {
            if (viewHost != null)
            {
                viewHost._RequestClosed = true;
            }
        }

        public void requestCloseAll()
        {
            foreach (var panel in openPanels)
            {
                if (panel.Current != null)
                {
                    panel.Current._RequestClosed = true;
                }
            }
            foreach (ViewHost viewHost in openFloatingViews)
            {
                viewHost._RequestClosed = true;
            }
        }

        public void processViewChanges()
        {
            foreach (var panel in openPanelsEnumerator.Values)
            {
                if (panel.Queued != null)
                {
                    //If there is no panel open
                    if (panel.Current == null)
                    {
                        panel.Current = viewHostFactory.createViewHost(panel.Queued, panel.QueuedContext);
                        panel.Current.opening();
                        guiManager.changePanel(panel.PanelSet, panel.PanelName, panel.Current.Container);
                    }
                    //If there is a panel open they must be switched
                    else
                    {
                        ViewHost last = panel.Current;
                        last.closing();
                        panel.Current = viewHostFactory.createViewHost(panel.Queued, panel.QueuedContext);
                        panel.Current.opening();
                        guiManager.changePanel(panel.PanelSet, panel.PanelName, panel.Current.Container, last._animationCallback);
                    }
                }
                //There is no other panel queued and the current panel wants to be closed
                else if (panel.Current != null && panel.Current._RequestClosed)
                {
                    panel.Current.closing();
                    guiManager.changePanel(panel.PanelSet, panel.PanelName, null, panel.Current._animationCallback);
                    panel.Current = null;
                }
                panel.Queued = null;
                panel.QueuedContext = null;
                if (panel.Current == null)
                {
                    openPanelsEnumerator.RemoveCurrent();
                }
            }

            //Floating views
            for (int i = 0; i < openFloatingViews.Count; ++i)
            {
                ViewHost host = openFloatingViews[i];
                if (host._RequestClosed)
                {
                    host.closing();
                    //We want to delay the animation callback till after the event.
                    ThreadManager.invoke(new Action(() =>
                    {
                        host._animationCallback(null);
                        closingFloatingViews.Remove(host);
                    }));
                    openFloatingViews.RemoveAt(i--);
                    closingFloatingViews.Add(host);
                }
            }
            foreach (KeyValuePair<View, AnomalousMvcContext> viewInfo in queuedFloatingViews)
            {
                ViewHost viewHost = viewHostFactory.createViewHost(viewInfo.Key, viewInfo.Value);
                viewHost.opening();
                openFloatingViews.Add(viewHost);
                if (viewInfo.Key.FillScreen)
                {
                    guiManager.addFullscreenPopup(viewHost.Container);
                    viewHost.ViewClosing += (closingView) =>
                    {
                        guiManager.removeFullscreenPopup(viewHost.Container);
                    };
                }
            }
            queuedFloatingViews.Clear();
        }

        public bool isViewOpen(String name)
        {
            foreach (var panel in openPanels)
            {
                if (panel.Current != null && panel.Current.Name == name)
                {
                    return true;
                }
            }

            foreach (ViewHost viewHost in openFloatingViews)
            {
                if (viewHost.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public ViewHost findViewHost(String name)
        {
            foreach (var panel in openPanels)
            {
                if (panel.Current != null && panel.Current.Name == name)
                {
                    return panel.Current;
                }
            }

            foreach (ViewHost viewHost in openFloatingViews)
            {
                if (viewHost.Name == name)
                {
                    return viewHost;
                }
            }

            return null;
        }

        public StoredViewCollection generateSavedViewLayout()
        {
            StoredViewCollection storedViews = new StoredViewCollection();
            foreach (var panel in openPanels)
            {
                if (panel.Current != null)
                {
                    storedViews.addView(panel.Current.View);
                }
            }
            foreach (ViewHost viewHost in openFloatingViews)
            {
                storedViews.addView(viewHost.View);
            }
            return storedViews;
        }

        public void restoreSavedViewLayout(StoredViewCollection storedViews, AnomalousMvcContext context)
        {
            foreach (View view in storedViews.Views)
            {
                requestOpen(view, context);
            }
        }

        public bool HasOpenViews
        {
            get
            {
                if (openFloatingViews.Count > 0)
                {
                    return true;
                }
                foreach (var panel in openPanels) //Make sure the views are actually open
                {
                    if (panel.Current != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private ViewHostPanelInfo findPanel(BorderPanelNames name, BorderPanelSets set)
        {
            foreach (var panel in openPanels)
            {
                if (panel.PanelName == name && panel.PanelSet == set)
                {
                    return panel;
                }
            }
            ViewHostPanelInfo newPanel = new ViewHostPanelInfo()
            {
                PanelSet = set,
                PanelName = name
            };
            openPanels.Add(newPanel);
            return newPanel;
        }
    }
}
