using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using Medical.GUI;

namespace Medical
{
    public class DockPanelDockProvider : DockProvider
    {
        public event DockProviderEvent ActiveDocumentChanged;

        private DockPanel dockPanel;

        public DockPanelDockProvider(DockPanel dockPanel)
        {
            this.dockPanel = dockPanel;
            dockPanel.ActiveDocumentChanged += new EventHandler(dockPanel_ActiveDocumentChanged);
        }

        void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (ActiveDocumentChanged != null)
            {
                ActiveDocumentChanged.Invoke(this);
            }
        }

        public bool restoreFromString(string persistString, out string name, out Engine.Vector3 translation, out Engine.Vector3 lookAt, out int bgColor)
        {
            return DockPanelDrawingWindowHost.RestoreFromString(persistString, out name, out translation, out lookAt, out bgColor);
        }

        public DrawingWindowHost createWindow(string name, DrawingWindowController controller)
        {
            return new DockPanelDrawingWindowHost(name, controller, dockPanel);
        }

        public DrawingWindowHost createCloneWindow(string name, DrawingWindowController controller)
        {
            return new DockPanelDrawingWindowCloneHost(name, controller, dockPanel);
        }

        public DrawingWindowHost ActiveDocument
        {
            get
            {
                return dockPanel.ActiveDocument as DrawingWindowHost;
            }
        }
    }
}
