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
        private bool currentlyMainGUIVisible;
        private Dialog dialog;

        public StandardDialogEntry(Dialog dialog)
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

        public void disposeDialog()
        {
            dialog.Dispose();
        }
    }
}
