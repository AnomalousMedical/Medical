using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Docking;
using Medical.GUI;
using Logging;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;

namespace Medical
{
    public class KryptonDockProvider : DockProvider
    {
        public event DockProviderEvent ActiveDocumentChanged;

        private KryptonDockingManager dockingManager;
        private KryptonDockableWorkspace workspace;
        private Dictionary<String, KryptonDrawingWindowHost> createdWindows = new Dictionary<string, KryptonDrawingWindowHost>();

        public KryptonDockProvider(KryptonDockingManager dockingManager, KryptonDockableWorkspace workspace)
        {
            this.dockingManager = dockingManager;
            dockingManager.PageCloseRequest += new EventHandler<CloseRequestEventArgs>(dockingManager_PageCloseRequest);
            dockingManager.PageFloatingRequest += new EventHandler<CancelUniqueNameEventArgs>(dockingManager_PageFloatingRequest);

            this.workspace = workspace;
            workspace.ActivePageChanged += new EventHandler<ComponentFactory.Krypton.Workspace.ActivePageChangedEventArgs>(workspace_ActivePageChanged);
            workspace.PageCloseClicked += new EventHandler<UniqueNameEventArgs>(workspace_PageCloseClicked);
        }

        void dockingManager_PageFloatingRequest(object sender, CancelUniqueNameEventArgs e)
        {
            KryptonPage page = dockingManager.PageForUniqueName(e.UniqueName);
            if (page.Controls.Count > 0)
            {
                KryptonDrawingWindowHost windowHost = page.Controls[0] as KryptonDrawingWindowHost;
                if(windowHost != null)
                {
                    e.Cancel = !windowHost.AllowFloating;
                }
            }
        }

        public bool restoreFromString(string persistString, out string name, out Engine.Vector3 translation, out Engine.Vector3 lookAt, out int bgColor)
        {
            throw new NotImplementedException();
        }

        public DrawingWindowHost createWindow(string name, DrawingWindowController controller)
        {
            KryptonDrawingWindowHost newWindow = new KryptonDrawingWindowHost(name, controller, dockingManager);
            createdWindows.Add(name, newWindow);
            return newWindow;
        }

        public DrawingWindowHost createCloneWindow(string name, DrawingWindowController controller)
        {
            KryptonDrawingWindowHost newWindow = new KryptonDrawingWindowHost(name, controller, dockingManager);
            createdWindows.Add(name, newWindow);
            newWindow.AllowFloating = true;
            newWindow.DrawingWindow.AllowNavigation = false;
            return newWindow;
        }

        public DrawingWindowHost ActiveDocument
        {
            get
            {
                if (workspace.ActivePage != null)
                {
                    return getPageWindow(workspace.ActivePage);
                }
                return null;
            }
        }

        void workspace_ActivePageChanged(object sender, ComponentFactory.Krypton.Workspace.ActivePageChangedEventArgs e)
        {
            if (ActiveDocumentChanged != null)
            {
                ActiveDocumentChanged.Invoke(this);
            }
            KryptonPage activePage = workspace.ActivePage;
            KryptonWorkspaceCell cell = workspace.CellForPage(activePage);
            if (cell != null)
            {
                foreach (KryptonPage page in cell.Pages)
                {
                    DrawingWindowHost host = getPageWindow(page);
                    if (host != null)
                    {
                        host.DrawingWindow.Enabled = page == activePage;
                    }
                }
            }
        }

        void workspace_PageCloseClicked(object sender, UniqueNameEventArgs e)
        {
            //Log.Debug("Page closed clicked.");
            //KryptonPage page = workspace.PageForUniqueName(e.UniqueName);
            //KryptonDrawingWindowHost host = page.Controls[0] as KryptonDrawingWindowHost;
            //if (host != null)
            //{
            //    host.alertClosing();
            //    page.Dispose();
            //}
            KryptonDrawingWindowHost host;
            createdWindows.TryGetValue(e.UniqueName, out host);
            if (host != null)
            {
                destroyWindow(host);
            }
        }

        void dockingManager_PageCloseRequest(object sender, CloseRequestEventArgs e)
        {
            //Log.Debug("Page close request.");
            //KryptonPage page = workspace.PageForUniqueName(e.UniqueName);
            //if (page != null && page.Controls.Count > 0)
            //{
            //    KryptonDrawingWindowHost host = page.Controls[0] as KryptonDrawingWindowHost;
            //    if (host != null)
            //    {
            //        host.alertClosing();
            //        page.Dispose();
            //    }
            //}
            KryptonDrawingWindowHost host;
            createdWindows.TryGetValue(e.UniqueName, out host);
            if (host != null)
            {
                destroyWindow(host);
            }
        }

        void destroyWindow(KryptonDrawingWindowHost windowHost)
        {
            windowHost.alertClosing();
            windowHost.Dispose();
            createdWindows.Remove(windowHost.Name);
        }

        private DrawingWindowHost getPageWindow(KryptonPage page)
        {
            if (page.Controls.Count > 0)
            {
                return page.Controls[0] as DrawingWindowHost;
            }
            return null;
        }
    }
}
