using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical.GUI
{
    class SystemMenu
    {
        private RecentDocuments recentDocuments = MedicalConfig.RecentDocuments;

        private NativeMenuItem recentPatients;
        private Dictionary<String, NativeMenuItem> recentDocMenuItems = new Dictionary<string, NativeMenuItem>();
        private Dictionary<int, string> menuIDsToFiles = new Dictionary<int, string>();

        private NativeMenu fileMenu;
        private NativeMenuItem changeScene;
        private NativeMenuItem open;
        private NativeMenuItem save;
        private NativeMenuItem saveAs;
        private NativeMenuItem exit;

        private PiperJBOGUIPlugin piperGUI;
        private StandaloneController standaloneController;

        public SystemMenu(NativeMenuBar menu, PiperJBOGUIPlugin piperGUI, StandaloneController standaloneController)
        {
            this.piperGUI = piperGUI;
            this.standaloneController = standaloneController;

            //File menu
            fileMenu = new NativeMenu();

            changeScene = fileMenu.Append(CommonMenuItems.New, "&New Scene...\tCtrl+N", "Change to a new scene.");
            changeScene.Select += new NativeMenuEvent(changeScene_Select);

            open = fileMenu.Append(CommonMenuItems.Open, "&Open...\tCtrl+O", "Open existing distortions.");
            open.Select += new NativeMenuEvent(open_Select);

            recentPatients = fileMenu.Append(CommonMenuItems.AutoAssign, "Recent Patients", new NativeMenu());
            foreach (String document in recentDocuments)
            {
                createWindowMenuDocument(document);
            }
            recentDocuments.DocumentAdded += new RecentDocumentEvent(recentDocuments_DocumentAdded);
            recentDocuments.DocumentReaccessed += new RecentDocumentEvent(recentDocuments_DocumentReaccessed);
            recentDocuments.DocumentRemoved += new RecentDocumentEvent(recentDocuments_DocumentRemoved);

            save = fileMenu.Append(CommonMenuItems.Save, "&Save...\tCtrl+S", "Save current distortions.");
            save.Select += new NativeMenuEvent(save_Select);

            saveAs = fileMenu.Append(CommonMenuItems.SaveAs, "Save &As...", "Save current distortions as.");
            saveAs.Select += new NativeMenuEvent(saveAs_Select);

            fileMenu.AppendSeparator();

            exit = fileMenu.Append(CommonMenuItems.Exit, "&Exit", "Exit the program.");
            exit.Select += new NativeMenuEvent(exit_Select);

            menu.Append(fileMenu, "&File");

            //Utilities Menu
            NativeMenu utilitiesMenu = new NativeMenu();

            NativeMenuItem cloneWindow = utilitiesMenu.Append(CommonMenuItems.AutoAssign, "Clone Window", "Open a window that displays the main window with no controls.");
            cloneWindow.Select += new NativeMenuEvent(cloneWindow_Select);

            NativeMenuItem preferences = utilitiesMenu.Append(CommonMenuItems.Preferences, "Preferences", "Set program configuration.");
            preferences.Select += new NativeMenuEvent(preferences_Select);

            menu.Append(utilitiesMenu, "&Utilities");

            //Help Menu
            NativeMenu helpMenu = new NativeMenu();

            NativeMenuItem help = helpMenu.Append(CommonMenuItems.Help, "Piper's JBO Help", "Open Piper's JBO user manual.");
            help.Select += new NativeMenuEvent(help_Select);

            NativeMenuItem about = helpMenu.Append(CommonMenuItems.About, "About", "About this program.");
            about.Select += new NativeMenuEvent(about_Select);

            menu.Append(helpMenu, "&Help");
        }

        void cloneWindow_Select(NativeMenuItem sender)
        {
            piperGUI.toggleCloneWindow();
        }

        void preferences_Select(NativeMenuItem sender)
        {
            piperGUI.showOptions();
        }

        void help_Select(NativeMenuItem sender)
        {
            standaloneController.openHelpTopic(0);
        }

        void about_Select(NativeMenuItem sender)
        {
            piperGUI.showAboutDialog();
        }

        public bool FileMenuEnabled
        {
            get
            {
                return changeScene.Enabled;
            }
            set
            {
                changeScene.Enabled = value;
                open.Enabled = value;
                save.Enabled = value;
                saveAs.Enabled = value;
                recentPatients.Enabled = value;
            }
        }

        void exit_Select(NativeMenuItem sender)
        {
            standaloneController.shutdown();
        }

        void saveAs_Select(NativeMenuItem sender)
        {
            piperGUI.saveAs();
        }

        void save_Select(NativeMenuItem sender)
        {
            piperGUI.save();
        }

        void open_Select(NativeMenuItem sender)
        {
            piperGUI.open();
        }

        void changeScene_Select(NativeMenuItem sender)
        {
            piperGUI.showChooseSceneDialog();
        }

        void recentDocMenuItem_Click(NativeMenuItem sender)
        {
            String document = menuIDsToFiles[sender.ID];
            PatientDataFile patientData = new PatientDataFile(document);
            if (patientData.loadHeader())
            {
                piperGUI.changeActiveFile(patientData);
                standaloneController.openPatientFile(patientData);
            }
            else
            {
                MyGUIPlugin.MessageBox.show(String.Format("Error loading file {0}.", patientData.BackingFile), "Load Error", MyGUIPlugin.MessageBoxStyle.Ok | MyGUIPlugin.MessageBoxStyle.IconError);
            }
        }

        void recentDocuments_DocumentRemoved(RecentDocuments source, string document)
        {
            NativeMenuItem recentDocMenuItem;
            if (recentDocMenuItems.TryGetValue(document, out recentDocMenuItem))
            {
                recentDocMenuItems.Remove(document);
                menuIDsToFiles.Remove(recentDocMenuItem.ID);
                recentPatients.SubMenu.Remove(recentDocMenuItem);
                recentDocMenuItem.Dispose();
            }
        }

        void recentDocuments_DocumentReaccessed(RecentDocuments source, string document)
        {
            NativeMenuItem recentDocMenuItem;
            if (recentDocMenuItems.TryGetValue(document, out recentDocMenuItem))
            {
                recentPatients.SubMenu.Remove(recentDocMenuItem);
                recentPatients.SubMenu.Insert(0, recentDocMenuItem);
            }
        }

        void recentDocuments_DocumentAdded(RecentDocuments source, string document)
        {
            createWindowMenuDocument(document);
        }

        void createWindowMenuDocument(String document)
        {
            if (recentPatients != null)
            {
                NativeMenuItem recentDocMenuItem = recentPatients.SubMenu.Insert(0, -1, Path.GetFileNameWithoutExtension(document));
                recentDocMenuItem.Select += new NativeMenuEvent(recentDocMenuItem_Click);
                recentDocMenuItem.Help = document;
                menuIDsToFiles.Add(recentDocMenuItem.ID, document);
                recentDocMenuItems.Add(document, recentDocMenuItem);
            }
        }
    }
}
