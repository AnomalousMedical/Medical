using System;
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

        private List<ViewHostPanelInfo> queuedFloatingViews = new List<ViewHostPanelInfo>();
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
                viewHost._finishedWithView();
            }
        }

        public void requestOpen(View view, AnomalousMvcContext context)
        {
            if (view.IsWindow)
            {
                queuedFloatingViews.Add(new ViewHostPanelInfo()
                {
                    Queued = view,
                    QueuedContext = context,
                    ElementName = view.ElementName
                });
            }
            else
            {
                ViewHostPanelInfo panel = null;
                switch (view.ElementName.LocationHint)
                {
                    case ViewLocations.Floating:
                        queuedFloatingViews.Add(new ViewHostPanelInfo()
                        {
                            Queued = view,
                            QueuedContext = context,
                            ElementName = view.ElementName
                        });
                        break;
                    default:
                        panel = findPanel(view.ElementName);
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
                        guiManager.changeElement(panel.ElementName, panel.Current.Container, panel.Current._finishedWithView);
                    }
                    //If there is a panel open they must be switched
                    else
                    {
                        ViewHost last = panel.Current;
                        last.closing();
                        panel.Current = viewHostFactory.createViewHost(panel.Queued, panel.QueuedContext);
                        panel.Current.opening();
                        guiManager.changeElement(panel.ElementName, panel.Current.Container, panel.Current._finishedWithView);
                    }
                }
                //There is no other panel queued and the current panel wants to be closed
                else if (panel.Current != null && panel.Current._RequestClosed)
                {
                    panel.Current.closing();
                    guiManager.changeElement(panel.ElementName, null, null);
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
                        host._finishedWithView();
                        closingFloatingViews.Remove(host);
                    }));
                    openFloatingViews.RemoveAt(i--);
                    closingFloatingViews.Add(host);
                }
            }
            foreach (var viewInfo in queuedFloatingViews)
            {
                ViewHost viewHost = viewHostFactory.createViewHost(viewInfo.Queued, viewInfo.QueuedContext);
                viewHost.opening();
                openFloatingViews.Add(viewHost);
                guiManager.changeElement(viewInfo.ElementName, viewHost.Container, null);
                viewHost.ViewClosing += (closingView) =>
                {
                    guiManager.closeElement(viewInfo.ElementName, viewHost.Container);
                };
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

        private ViewHostPanelInfo findPanel(LayoutElementName elementName)
        {
            foreach (var panel in openPanels)
            {
                if (panel.ElementName == elementName)
                {
                    return panel;
                }
            }
            ViewHostPanelInfo newPanel = new ViewHostPanelInfo()
            {
                ElementName = elementName
            };
            openPanels.Add(newPanel);
            return newPanel;
        }
    }
}
