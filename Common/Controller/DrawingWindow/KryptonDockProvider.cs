using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Docking;
using Medical.GUI;
using Logging;
using ComponentFactory.Krypton.Navigator;

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
            e.Cancel = true;
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
            throw new NotImplementedException();
        }

        public DrawingWindowHost ActiveDocument
        {
            get
            {
                if (workspace.ActivePage != null && workspace.ActivePage.Controls.Count > 0)
                {
                    return workspace.ActivePage.Controls[0] as DrawingWindowHost;
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
            destroyWindow(createdWindows[e.UniqueName]);
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
            destroyWindow(createdWindows[e.UniqueName]);
        }

        void destroyWindow(KryptonDrawingWindowHost windowHost)
        {
            windowHost.alertClosing();
            windowHost.Dispose();
            createdWindows.Remove(windowHost.Name);
        }
    }
}
