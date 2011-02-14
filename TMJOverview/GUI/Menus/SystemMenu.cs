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

            NativeMenuItem preferences = fileMenu.append(CommonMenuItems.Preferences, "Preferences", "Set program configuration.");
            preferences.Select += new NativeMenuEvent(preferences_Select);

            exit = fileMenu.append(CommonMenuItems.Exit, "&Exit", "Exit the program.");
            exit.Select += new NativeMenuEvent(exit_Select);

            menu.append(fileMenu);

            //Help Menu
            NativeMenu helpMenu = menu.createMenu("&Help");

            NativeMenuItem help = helpMenu.append(CommonMenuItems.Help, "Piper's JBO Help", "Open Piper's JBO user manual.");
            help.Select += new NativeMenuEvent(help_Select);

            NativeMenuItem about = helpMenu.append(CommonMenuItems.About, "About", "About this program.");
            about.Select += new NativeMenuEvent(about_Select);

            menu.append(helpMenu);
        }

        void tmjOverview_Select(NativeMenuItem item)
        {
            guiPlugin.runTMJOverview();
        }

        void preferences_Select(NativeMenuItem sender)
        {
            guiPlugin.showOptions();
        }

        void help_Select(NativeMenuItem sender)
        {
            standaloneController.openHelpTopic(0);
        }

        void about_Select(NativeMenuItem sender)
        {
            guiPlugin.showAboutDialog();
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
