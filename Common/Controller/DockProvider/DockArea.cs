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

        public void close(DockContent content)
        {
            dockingManager.RemovePage(content.Page, !content.HideOnClose);
        }

        public void show(DockContent content, DockAreas alignment)
        {
            pageArray[0] = content.Page;
            DockingEdge edge = DockingEdge.Left;
            switch(alignment)
            {
                case DockAreas.DockBottom:
                    dockingManager.AddDockspace("Bottom", DockingEdge.Bottom, pageArray);
                    break;
                case DockAreas.DockTop:
                    dockingManager.AddDockspace("Top", DockingEdge.Top, pageArray);
                    break;
                case DockAreas.DockLeft:
                    dockingManager.AddDockspace("Left", DockingEdge.Left, pageArray);
                    break;
                case DockAreas.DockRight:
                    dockingManager.AddDockspace("Right", DockingEdge.Right, pageArray);
                    break;
                case DockAreas.Document:
                    dockingManager.AddToWorkspace("Workspace", pageArray);
                    break;
                case DockAreas.Float:
                    dockingManager.AddFloatingWindow("Floating", pageArray);
                    break;
            }
        }

        public DockContent ActiveDocument { get; set; }

        public void SaveAsXml(String filename)
        {

        }
    }
}
