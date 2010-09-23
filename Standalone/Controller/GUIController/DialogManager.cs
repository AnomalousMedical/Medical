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
        private List<Dialog> dialogs = new List<Dialog>();

        public DialogManager()
        {
            
        }

        public void addManagedDialog(Dialog dialog)
        {
            dialogs.Add(dialog);
        }

        public void saveDialogLayout(String file)
        {
            ConfigFile windowConfig = new ConfigFile(file);
            foreach (Dialog dialog in dialogs)
            {
                dialog.serialize(windowConfig);
            }
            windowConfig.writeConfigFile();
        }

        public void loadDialogLayout(String file)
        {
            ConfigFile windowConfig = new ConfigFile(file);
            windowConfig.loadConfigFile();
            foreach (Dialog dialog in dialogs)
            {
                dialog.deserialize(windowConfig);
            }
        }
    }
}
