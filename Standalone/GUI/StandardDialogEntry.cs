using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine;

namespace Medical.GUI
{
    class StandardDialogEntry : DialogEntry
    {
        private Dialog dialog;

        public StandardDialogEntry(Dialog dialog)
        {
            this.dialog = dialog;
            CurrentlyVisible = dialog.Visible;
        }

        public void tempClose()
        {
            CurrentlyVisible = dialog.Visible;
            dialog.Visible = false;
        }

        public void restoreState()
        {
            dialog.Visible = CurrentlyVisible;
        }

        public void serialize(ConfigFile file)
        {
            dialog.serialize(file);
        }

        public void deserialize(ConfigFile file)
        {
            dialog.deserialize(file);
        }

        public void ensureVisible()
        {
            dialog.ensureVisible();
        }

        public bool CurrentlyVisible { get; set; }
    }
}
