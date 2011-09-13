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
        private DocumentController documentController;

        private NativeMenuItem recentPatients;
        private Dictionary<String, NativeMenuItem> recentDocMenuItems = new Dictionary<string, NativeMenuItem>();
        private Dictionary<int, string> menuIDsToFiles = new Dictionary<int, string>();

        private NativeMenu fileMenu;
        private NativeMenuItem changeScene;
        private NativeMenuItem open;
        private NativeMenuItem save;
        private NativeMenuItem saveAs;
        private NativeMenuItem exit;

        private BodyAtlasMainPlugin bodyAtlasGUI;
        private StandaloneController standaloneController;

        public SystemMenu(NativeMenuBar menu, BodyAtlasMainPlugin piperGUI, StandaloneController standaloneController, LicenseManager licenseManager)
        {
            this.bodyAtlasGUI = piperGUI;
            this.standaloneController = standaloneController;
            this.documentController = standaloneController.DocumentController;

            //File menu
            fileMenu = menu.createMenu("&File");

            changeScene = fileMenu.append(CommonMenuItems.New, "&New Scene...\tCtrl+N", "Change to a new scene.");
            changeScene.Select += new NativeMenuEvent(changeScene_Select);

            open = fileMenu.append(CommonMenuItems.Open, "&Open...\tCtrl+O", "Open existing distortions.");
            open.Select += new NativeMenuEvent(open_Select);

            recentPatients = fileMenu.append(CommonMenuItems.AutoAssign, "Recent Patients", "", true);
            foreach (String document in documentController.RecentDocuments)
            {
                createWindowMenuDocument(document);
            }
            documentController.DocumentAdded += new RecentDocumentEvent(recentDocuments_DocumentAdded);
            documentController.DocumentReaccessed += new RecentDocumentEvent(recentDocuments_DocumentReaccessed);
            documentController.DocumentRemoved += new RecentDocumentEvent(recentDocuments_DocumentRemoved);

            save = fileMenu.append(CommonMenuItems.Save, "&Save...\tCtrl+S", "Save current distortions.");
            save.Select += new NativeMenuEvent(save_Select);

            saveAs = fileMenu.append(CommonMenuItems.SaveAs, "Save &As...", "Save current distortions as.");
            saveAs.Select += new NativeMenuEvent(saveAs_Select);

            fileMenu.appendSeparator();

            NativeMenuItem preferences = fileMenu.append(CommonMenuItems.Preferences, "Preferences", "Set program configuration.");
            preferences.Select += new NativeMenuEvent(preferences_Select);

            exit = fileMenu.append(CommonMenuItems.Exit, "&Exit", "Exit the program.");
            exit.Select += new NativeMenuEvent(exit_Select);

            menu.append(fileMenu);

            //if (PlatformConfig.AllowCloneWindows && licenseManager.allowFeature((int)Features.PIPER_JBO_FEATURE_CLONE_WINDOW))
            //{
            //    //Utilities Menu
            //    NativeMenu utilitiesMenu = menu.createMenu("&Utilities");

            //    NativeMenuItem cloneWindow = utilitiesMenu.append(CommonMenuItems.AutoAssign, "Clone Window", "Open a window that displays the main window with no controls.");
            //    cloneWindow.Select += new NativeMenuEvent(cloneWindow_Select);

            //    menu.append(utilitiesMenu);
            //}

            //Help Menu
            NativeMenu helpMenu = menu.createMenu("&Help");

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
            standaloneController.exit();
        }

        void saveAs_Select(NativeMenuItem sender)
        {
            bodyAtlasGUI.saveAs();
        }

        void save_Select(NativeMenuItem sender)
        {
            bodyAtlasGUI.save();
        }

        void open_Select(NativeMenuItem sender)
        {
            bodyAtlasGUI.open();
        }

        void changeScene_Select(NativeMenuItem sender)
        {
            bodyAtlasGUI.showChooseSceneDialog();
        }

        void recentDocMenuItem_Click(NativeMenuItem sender)
        {
            String document = menuIDsToFiles[sender.ID];
            documentController.openFile(document);
        }

        void recentDocuments_DocumentRemoved(RecentDocuments source, string document)
        {
            NativeMenuItem recentDocMenuItem;
            if (recentDocMenuItems.TryGetValue(document, out recentDocMenuItem))
            {
                recentDocMenuItems.Remove(document);
                menuIDsToFiles.Remove(recentDocMenuItem.ID);
                recentPatients.SubMenu.remove(recentDocMenuItem);
                recentDocMenuItem.Dispose();
            }
        }

        void recentDocuments_DocumentReaccessed(RecentDocuments source, string document)
        {
            NativeMenuItem recentDocMenuItem;
            if (recentDocMenuItems.TryGetValue(document, out recentDocMenuItem))
            {
                recentPatients.SubMenu.remove(recentDocMenuItem);
                recentPatients.SubMenu.insert(0, recentDocMenuItem);
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
                NativeMenuItem recentDocMenuItem = recentPatients.SubMenu.insert(0, CommonMenuItems.AutoAssign, Path.GetFileNameWithoutExtension(document), "");
                recentDocMenuItem.Select += new NativeMenuEvent(recentDocMenuItem_Click);
                recentDocMenuItem.Help = document;
                menuIDsToFiles.Add(recentDocMenuItem.ID, document);
                recentDocMenuItems.Add(document, recentDocMenuItem);
            }
        }
    }
}
