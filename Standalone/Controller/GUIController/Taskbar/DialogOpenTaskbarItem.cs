using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class DialogOpenTaskbarItem : TaskbarItem
    {
        private Dialog dialog;

        public DialogOpenTaskbarItem(Dialog dialog, String name, String iconName)
            :base(name, iconName)
        {
            this.dialog = dialog;
            dialog.Shown += new EventHandler(dialog_Shown);
            dialog.Closed += new EventHandler(dialog_Closed);
        }

        public override void clicked(Widget source, EventArgs e)
        {
            dialog.Visible = !dialog.Visible;
        }

        void dialog_Closed(object sender, EventArgs e)
        {
            taskbarButton.StateCheck = false;
        }

        void dialog_Shown(object sender, EventArgs e)
        {
            taskbarButton.StateCheck = true;
        }
    }
}
