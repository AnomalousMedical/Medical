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
            dialog.CurrentDockLocation = DockLocation.Floating;
            dialog.AllowedDockLocations = DockLocation.All;
            dialogs.Add(new MDIDialogEntry(dialog));
        }

        public void saveDialogLayout(String file)
        {
            ConfigFile windowConfig = new ConfigFile(file);
            foreach (DialogEntry dialog in dialogs)
            {
                dialog.serialize(windowConfig);
            }
            windowConfig.writeConfigFile();
        }

        public void loadDialogLayout(String file)
        {
            ConfigFile windowConfig = new ConfigFile(file);
            windowConfig.loadConfigFile();
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

        public void temporarilyCloseDialogs()
        {
            foreach (DialogEntry dialog in dialogs)
            {
                dialog.tempClose();
            }
        }

        public void reopenDialogs()
        {
            foreach (DialogEntry dialog in dialogs)
            {
                dialog.restoreState();
            }
        }
    }
}
