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
            this.DockAreas = DockAreas.Float;
            this.ShowHint = DockState.Float;
        }

        protected override string GetPersistString()
        {
            return null;
        }
    }
}
