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
            
        }

        public void requestOpen(View view, AnomalousMvcContext context)
        {
            if (view.IsWindow)
            {
                openPanels.Add(new ViewHostPanelInfo()
                {
                    Queued = view,
                    QueuedContext = context,
                    ElementName = view.ElementName
                });
            }
            else
            {
                ViewHostPanelInfo panel = findPanel(view.ElementName);
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
                    guiManager.closeElement(panel.ElementName, panel.Current.Container);
                    panel.Current = null;
                }
                panel.Queued = null;
                panel.QueuedContext = null;
                if (panel.Current == null)
                {
                    openPanelsEnumerator.RemoveCurrent();
                }
            }
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
            if (elementName.LocationHint == ViewLocations.Floating)
            {
                ViewHostPanelInfo panel = new ViewHostPanelInfo()
                {
                    ElementName = elementName
                };
                openPanels.Add(panel);
                return panel;
            }

            //Try to find an existing panel
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
