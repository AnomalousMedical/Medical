using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class DialogManager
    {
        private class DialogEntry
        {
            private Dialog dialog;

            public DialogEntry(Dialog dialog)
            {
                this.dialog = dialog;
                Visible = dialog.Visible;
            }

            public void tempClose()
            {
                Visible = dialog.Visible;
                dialog.Visible = false;
            }

            public void restoreState()
            {
                dialog.Visible = Visible;
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

            public bool Visible { get; set; }
        }

        private List<DialogEntry> dialogs = new List<DialogEntry>();

        public DialogManager()
        {
            
        }

        public void addManagedDialog(Dialog dialog)
        {
            dialogs.Add(new DialogEntry(dialog));
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
