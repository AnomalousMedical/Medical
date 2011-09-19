using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Medical.Controller;

namespace Medical.GUI
{
    class SystemMenu
    {
        private NativeMenuItem exit;

        private AnomalousMainPlugin bodyAtlasGUI;
        private StandaloneController standaloneController;

        public SystemMenu(NativeMenuBar menu, AnomalousMainPlugin piperGUI, StandaloneController standaloneController)
        {
            this.bodyAtlasGUI = piperGUI;
            this.standaloneController = standaloneController;

            //Help Menu
            NativeMenu helpMenu = menu.createMenu("&Help");
			
			NativeMenuItem preferences = helpMenu.append(CommonMenuItems.Preferences, "Preferences", "Set program configuration.");
            preferences.Select += new NativeMenuEvent(preferences_Select);

            exit = helpMenu.append(CommonMenuItems.Exit, "&Exit", "Exit the program.");
            exit.Select += new NativeMenuEvent(exit_Select);

            NativeMenuItem help = helpMenu.append(CommonMenuItems.Help, "Piper's JBO Help", "Open Piper's JBO user manual.");
            help.Select += new NativeMenuEvent(help_Select);

            NativeMenuItem about = helpMenu.append(CommonMenuItems.About, "About", "About this program.");
            about.Select += new NativeMenuEvent(about_Select);

            menu.append(helpMenu);
        }

        void preferences_Select(NativeMenuItem sender)
        {
            bodyAtlasGUI.showOptions();
        }

        void help_Select(NativeMenuItem sender)
        {
            standaloneController.openHelpTopic(0);
        }

        void about_Select(NativeMenuItem sender)
        {
            bodyAtlasGUI.showAboutDialog();
        }

        void exit_Select(NativeMenuItem sender)
        {
            standaloneController.exit();
        }
    }
}
