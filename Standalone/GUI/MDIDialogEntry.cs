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
        private MDIDialog dialog;

        public MDIDialogEntry(MDIDialog dialog)
        {
            this.dialog = dialog;
        }

        public void closeMainGUIDialog()
        {
            dialog.Visible = false;
        }

        public void openMainGUIDialog()
        {
            //Do nothing, this is restored by the layout manager
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
