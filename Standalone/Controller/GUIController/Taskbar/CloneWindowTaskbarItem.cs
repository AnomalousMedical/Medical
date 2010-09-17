using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class CloneWindowTaskbarItem : TaskbarItem
    {
        private PiperJBOGUI piperGUI;

        public CloneWindowTaskbarItem(PiperJBOGUI piperGUI)
            :base("Clone Window", "CloneWindowLarge")
        {
            this.piperGUI = piperGUI;
        }

        public override void clicked(Widget source, EventArgs e)
        {
            piperGUI.toggleCloneWindow();
        }
    }
}
