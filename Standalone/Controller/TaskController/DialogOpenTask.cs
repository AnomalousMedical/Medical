using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical
{
    public class DialogOpenTask : Task
    {
        private Dialog dialog;

        public DialogOpenTask(Dialog dialog, String uniqueName, String name, String iconName, String category)
            : this(dialog, uniqueName, name, iconName, category, DEFAULT_WEIGHT)
        {
            
        }

        public DialogOpenTask(Dialog dialog, String uniqueName, String name, String iconName, String category, int weight)
            : this(dialog, uniqueName, name, iconName, category, weight, true)
        {
            
        }

        public DialogOpenTask(Dialog dialog, String uniqueName, String name, String iconName, String category, bool showOnTaskbar)
            : this(dialog, uniqueName, name, iconName, category, DEFAULT_WEIGHT, showOnTaskbar)
        {

        }

        public DialogOpenTask(Dialog dialog, String uniqueName, String name, String iconName, String category, int weight, bool showOnTaskbar)
            : base(uniqueName, name, iconName, category)
        {
            this.dialog = dialog;
            this.Weight = weight;
            this.ShowOnTaskbar = showOnTaskbar;
            dialog.Closed += new EventHandler(dialog_Closed);
        }

        public override void clicked(TaskPositioner positioner)
        {
            if (!dialog.Visible)
            {
                dialog.Visible = true;
            }
            else
            {
                dialog.bringToFront();
            }
        }

        public override bool Active
        {
            get { return dialog.Visible; }
        }

        void dialog_Closed(object sender, EventArgs e)
        {
            fireItemClosed();
        }
    }
}
