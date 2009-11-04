using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class DrawingWindowCloneHost : DrawingWindowHost
    {
        public DrawingWindowCloneHost(String name, DrawingWindowController controller)
            :base(name, controller)
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
