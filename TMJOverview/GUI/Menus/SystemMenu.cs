using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical.GUI
{
    class SystemMenu
    {
        private NativeMenu fileMenu;
        private NativeMenuItem tmjOverview;
        private NativeMenuItem exit;

        private TMJOverviewGUIPlugin guiPlugin;
        private StandaloneController standaloneController;

        public SystemMenu(NativeMenuBar menu, TMJOverviewGUIPlugin guiPlugin, StandaloneController standaloneController)
        {
            this.guiPlugin = guiPlugin;
            this.standaloneController = standaloneController;

            //File menu
            fileMenu = menu.createMenu("&File");

            tmjOverview = fileMenu.append(CommonMenuItems.AutoAssign, "Play TMJ Overview", "Play the TMJ Overview.");
            tmjOverview.Select += new NativeMenuEvent(tmjOverview_Select);

            fileMenu.appendSeparator();

            exit = fileMenu.append(CommonMenuItems.Exit, "&Exit", "Exit the program.");
            exit.Select += new NativeMenuEvent(exit_Select);

            menu.append(fileMenu);
        }

        void tmjOverview_Select(NativeMenuItem item)
        {
            guiPlugin.runTMJOverview();
        }

        void help_Select(NativeMenuItem sender)
        {
            standaloneController.openHelpTopic(0);
        }

        public bool FileMenuEnabled
        {
            get
            {
                return tmjOverview.Enabled;
            }
            set
            {
                tmjOverview.Enabled = value;
            }
        }

        void exit_Select(NativeMenuItem sender)
        {
            standaloneController.closeMainWindow();
        }
    }
}
