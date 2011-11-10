using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    public class MDIDialogOpenTask : Task
    {
        private MDIDialog dialog;

        public MDIDialogOpenTask(MDIDialog dialog, String uniqueName, String name, String iconName, String category)
            : this(dialog, uniqueName, name, iconName, category, DEFAULT_WEIGHT)
        {
            
        }

        public MDIDialogOpenTask(MDIDialog dialog, String uniqueName, String name, String iconName, String category, int weight)
            : this(dialog, uniqueName, name, iconName, category, weight, true)
        {
            
        }

        public MDIDialogOpenTask(MDIDialog dialog, String uniqueName, String name, String iconName, String category, bool showOnTaskbar)
            : this(dialog, uniqueName, name, iconName, category, DEFAULT_WEIGHT, showOnTaskbar)
        {

        }

        public MDIDialogOpenTask(MDIDialog dialog, String uniqueName, String name, String iconName, String category, int weight, bool showOnTaskbar)
            : base(uniqueName, name, iconName, category)
        {
            this.dialog = dialog;
            this.Weight = weight;
            this.ShowOnTaskbar = showOnTaskbar;
            dialog.Closed += new EventHandler(dialog_Closed);
            dialog.Shown += new EventHandler(dialog_Shown);
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

        void dialog_Closed(object sender, EventArgs e)
        {
            fireItemClosed();
        }

        void dialog_Shown(object sender, EventArgs e)
        {
            fireRequestShowInTaskbar();
        }

        public override bool Active
        {
            get { return dialog.Visible; }
        }
    }
}
