using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.View
{
    class DrawingWindowCloneHost : DrawingWindowHost
    {
        public DrawingWindowCloneHost(String name, DrawingWindowController controller)
            :base(name, controller)
        {

        }

        public override bool IsClone
        {
            get
            {
                return true;
            }
        }
    }
}
