using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class MDIDialogOpenTaskMenuItem : TaskMenuItem
    {
        private MDIDialog dialog;

        public MDIDialogOpenTaskMenuItem(MDIDialog dialog, String name, String iconName, String category)
            : base(name, iconName, category)
        {
            this.dialog = dialog;
        }

        public override void clicked()
        {
            dialog.Visible = !dialog.Visible;
        }
    }
}
