using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;

namespace Medical
{
    public class DockArea
    {
        private KryptonDockingManager dockingManager;
        KryptonPage[] pageArray = new KryptonPage[1];

        public DockArea(KryptonDockableWorkspace workspace, KryptonPanel panel, Form form)
        {
            dockingManager = new KryptonDockingManager();

            KryptonDockingWorkspace n = dockingManager.ManageWorkspace("Workspace", workspace);
            dockingManager.ManageControl("Panel", panel);
            dockingManager.ManageFloating("Floating", form);
        }

        public void show(DockContent content)
        {
            pageArray[0] = content.Page;
            dockingManager.AddToWorkspace("Workspace", pageArray);
        }

        public DockContent ActiveDocument { get; set; }

        public void SaveAsXml(String filename)
        {

        }
    }
}
