using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine;

namespace Medical.GUI
{
    class MDIDialogEntry : DialogEntry
    {
        private bool currentlyMainGUIVisible;
        private MDIDialog dialog;

        public MDIDialogEntry(MDIDialog dialog)
        {
            this.dialog = dialog;
            currentlyMainGUIVisible = dialog.Visible;
        }

        public void closeMainGUIDialog()
        {
            currentlyMainGUIVisible = dialog.Visible;
            dialog.Visible = false;
        }

        public void openMainGUIDialog()
        {
            dialog.Visible = currentlyMainGUIVisible;
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
    }
}
