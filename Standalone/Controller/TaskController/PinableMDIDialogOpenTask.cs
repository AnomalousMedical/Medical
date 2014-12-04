using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller;

namespace Medical
{
    /// <summary>
    /// This is a special MDIDialogOpenTask for Pinable dialogs. When the dialog is unpinned and floating
    /// it will open the dialog near the task, otherwise it will respect the position it was given.
    /// </summary>
    public class PinableMDIDialogOpenTask : Task
    {
        private PinableMDIDialog dialog;

        public PinableMDIDialogOpenTask(PinableMDIDialog dialog, String uniqueName, String name, String iconName, String category)
            : this(dialog, uniqueName, name, iconName, category, DEFAULT_WEIGHT)
        {
            
        }

        public PinableMDIDialogOpenTask(PinableMDIDialog dialog, String uniqueName, String name, String iconName, String category, int weight)
            : this(dialog, uniqueName, name, iconName, category, weight, true)
        {
            
        }

        public PinableMDIDialogOpenTask(PinableMDIDialog dialog, String uniqueName, String name, String iconName, String category, bool showOnTaskbar)
            : this(dialog, uniqueName, name, iconName, category, DEFAULT_WEIGHT, showOnTaskbar)
        {

        }

        public PinableMDIDialogOpenTask(PinableMDIDialog dialog, String uniqueName, String name, String iconName, String category, int weight, bool showOnTaskbar)
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
                if(dialog.AllowAutoPosition)
                {
                    dialog.Position = positioner.findGoodWindowPosition(dialog.Width, dialog.Height);
                }
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
