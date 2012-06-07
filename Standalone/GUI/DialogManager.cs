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
        private Dictionary<MDIDialog, DialogEntry> mdiDialogs = new Dictionary<MDIDialog, DialogEntry>();
        private MDILayoutManager mdiLayoutManager;
        private StoredMDILayout storedLayout;

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
            MDIDialogEntry entry = new MDIDialogEntry(dialog);
            dialogs.Add(entry);
            mdiDialogs.Add(dialog, entry);
        }

        internal void removeManagedDialog(MDIDialog dialog)
        {
            DialogEntry entry;
            if(mdiDialogs.TryGetValue(dialog, out entry))
            {
                mdiDialogs.Remove(dialog);
                dialogs.Remove(entry);
            }
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
            storedLayout = mdiLayoutManager.storeCurrentLayout();
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
            mdiLayoutManager.restoreLayout(storedLayout);
            storedLayout = null;
        }
    }
}
