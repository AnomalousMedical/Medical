using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller;

namespace Medical.GUI
{
    public class DialogManager
    {
        private List<DialogEntry> dialogs = new List<DialogEntry>();
        private MDILayoutManager mdiLayoutManager;

        public DialogManager(MDILayoutManager mdiLayoutManager)
        {
            this.mdiLayoutManager = mdiLayoutManager;
        }

        public void addManagedDialog(Dialog dialog)
        {
            dialogs.Add(new StandardDialogEntry(dialog));
        }

        public void addManagedDialog(MDIDialog dialog)
        {
            dialog.MDIManager = mdiLayoutManager;
            dialogs.Add(new MDIDialogEntry(dialog));
        }

        public void saveDialogLayout(ConfigFile windowConfig)
        {
            foreach (DialogEntry dialog in dialogs)
            {
                dialog.serialize(windowConfig);
            }
        }

        public void loadDialogLayout(ConfigFile windowConfig)
        {
            foreach (DialogEntry dialog in dialogs)
            {
                dialog.deserialize(windowConfig);
            }
        }

        public void windowResized()
        {
            foreach (DialogEntry dialog in dialogs)
            {
                dialog.ensureVisible();
            }
        }

        public void closeMainGUIDialogs()
        {
            foreach (DialogEntry dialog in dialogs)
            {
                dialog.closeMainGUIDialog();
            }
        }

        public void reopenMainGUIDialogs()
        {
            foreach (DialogEntry dialog in dialogs)
            {
                dialog.openMainGUIDialog();
            }
        }
    }
}
