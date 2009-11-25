using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.GUI
{
    class DockPanelDrawingWindowCloneHost : DockPanelDrawingWindowHost
    {
        public DockPanelDrawingWindowCloneHost(String name, DrawingWindowController controller, DockPanel dock)
            :base(name, controller, dock)
        {
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Float;
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Float;
        }

        protected override string GetPersistString()
        {
            return null;
        }
    }
}
