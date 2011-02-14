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
        private NativeMenuItem detailedDiagnose;
        private NativeMenuItem quickDiagnose;
        private NativeMenuItem export;
        private NativeMenuItem exit;

        private TMJOverviewGUIPlugin guiPlugin;
        private StandaloneController standaloneController;

        public SystemMenu(NativeMenuBar menu, TMJOverviewGUIPlugin guiPlugin, StandaloneController standaloneController)
        {
            this.guiPlugin = guiPlugin;
            this.standaloneController = standaloneController;

            //File menu
            fileMenu = menu.createMenu("&File");

            detailedDiagnose = fileMenu.append(CommonMenuItems.AutoAssign, "Detailed Diagnose", "Run the detailed diagnosis.");
            detailedDiagnose.Select += new NativeMenuEvent(detailedDiagnose_Select);

            quickDiagnose = fileMenu.append(CommonMenuItems.AutoAssign, "Quick Diagnose", "Run the quick diagnosis.");
            quickDiagnose.Select += new NativeMenuEvent(quickDiagnose_Select);

            export = fileMenu.append(CommonMenuItems.AutoAssign, "Export", "Export the results.");
            export.Select += new NativeMenuEvent(export_Select);

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

        void export_Select(NativeMenuItem item)
        {
            guiPlugin.export();
        }

        void quickDiagnose_Select(NativeMenuItem item)
        {
            guiPlugin.runQuickDiagnosis();
        }

        void detailedDiagnose_Select(NativeMenuItem item)
        {
            guiPlugin.runDetailedDiagnosis();
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
                return detailedDiagnose.Enabled;
            }
            set
            {
                detailedDiagnose.Enabled = value;
                quickDiagnose.Enabled = value;
                export.Enabled = value;
            }
        }

        void exit_Select(NativeMenuItem sender)
        {
            standaloneController.closeMainWindow();
        }
    }
}
