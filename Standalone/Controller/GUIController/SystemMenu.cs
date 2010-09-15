using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Standalone;
using MyGUIPlugin;
using System.IO;

namespace Medical.GUI
{
    class SystemMenu
    {
        private RecentDocuments recentDocuments = MedicalConfig.RecentDocuments;

        private wx.MenuItem recentPatients;
        private Dictionary<String, wx.MenuItem> recentDocMenuItems = new Dictionary<string, wx.MenuItem>();
        private Dictionary<int, string> menuIDsToFiles = new Dictionary<int, string>();

        private wx.Menu fileMenu;
        private wx.MenuItem changeScene;
        private wx.MenuItem open;
        private wx.MenuItem save;
        private wx.MenuItem saveAs;
        private wx.MenuItem exit;

        private PiperJBOGUI piperGUI;
        private StandaloneController standaloneController;

        public SystemMenu(wx.MenuBar menu, PiperJBOGUI piperGUI, StandaloneController standaloneController)
        {
            this.piperGUI = piperGUI;
            this.standaloneController = standaloneController;

            //File menu
            fileMenu = new wx.Menu();

            changeScene = fileMenu.Append((int)wx.MenuIDs.wxID_NEW, "&New Scene...\tCtrl+N", "Change to a new scene.");
            changeScene.Select += new wx.EventListener(changeScene_Select);

            open = fileMenu.Append((int)wx.MenuIDs.wxID_OPEN, "&Open...\tCtrl+O", "Open existing distortions.");
            open.Select += new wx.EventListener(open_Select);

            recentPatients = fileMenu.Append(-1, "Recent Patients", new wx.Menu());
            foreach (String document in recentDocuments)
            {
                createWindowMenuDocument(document);
            }
            recentDocuments.DocumentAdded += new RecentDocumentEvent(recentDocuments_DocumentAdded);
            recentDocuments.DocumentReaccessed += new RecentDocumentEvent(recentDocuments_DocumentReaccessed);
            recentDocuments.DocumentRemoved += new RecentDocumentEvent(recentDocuments_DocumentRemoved);

            save = fileMenu.Append((int)wx.MenuIDs.wxID_SAVE, "&Save...\tCtrl+S", "Save current distortions.");
            save.Select += new wx.EventListener(save_Select);

            saveAs = fileMenu.Append((int)wx.MenuIDs.wxID_SAVEAS, "Save &As...", "Save current distortions as.");
            saveAs.Select += new wx.EventListener(saveAs_Select);

            fileMenu.AppendSeparator();

            exit = fileMenu.Append((int)wx.MenuIDs.wxID_EXIT, "&Exit", "Exit the program.");
            exit.Select += new wx.EventListener(exit_Select);

            menu.Append(fileMenu, "&File");

            //Tools Menu
            wx.Menu toolsMenu = new wx.Menu();

            wx.MenuItem cloneWindow = toolsMenu.Append(-1, "Clone Window", "Open a window that displays the main window with no controls.");
            cloneWindow.Select += new wx.EventListener(cloneWindow_Select);

            wx.MenuItem showStats = toolsMenu.Append(-1, "Show Statistics", "Show performance statistics.");
            showStats.Select += new wx.EventListener(showStats_Select);

            wx.MenuItem preferences = toolsMenu.Append((int)wx.MenuIDs.wxID_PREFERENCES, "Preferences", "Set program configuration.");
            preferences.Select += new wx.EventListener(preferences_Select);

            menu.Append(toolsMenu, "&Tools");

            //Help Menu
            wx.Menu helpMenu = new wx.Menu();

            wx.MenuItem help = helpMenu.Append((int)wx.MenuIDs.wxID_HELP, "Piper's JBO Help", "Open Piper's JBO user manual.");
            help.Select += new wx.EventListener(help_Select);

            wx.MenuItem about = helpMenu.Append((int)wx.MenuIDs.wxID_ABOUT, "About", "About this program.");
            about.Select += new wx.EventListener(about_Select);

            menu.Append(helpMenu, "&Help");
        }

        void showStats_Select(object sender, wx.Event e)
        {
            standaloneController.SceneViewController.ActiveWindow.ShowStats = !standaloneController.SceneViewController.ActiveWindow.ShowStats;
        }

        void cloneWindow_Select(object sender, wx.Event e)
        {
            piperGUI.toggleCloneWindow();
        }

        void preferences_Select(object sender, wx.Event e)
        {
            piperGUI.showOptions();
        }

        void help_Select(object sender, wx.Event e)
        {
            standaloneController.openHelpTopic(0);
        }

        void about_Select(object sender, wx.Event e)
        {
            piperGUI.showAboutDialog();
        }

        public bool MenuEnabled
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

        void exit_Select(object sender, wx.Event e)
        {
            standaloneController.shutdown();
        }

        void saveAs_Select(object sender, wx.Event e)
        {
            piperGUI.saveAs();
        }

        void save_Select(object sender, wx.Event e)
        {
            piperGUI.save();
        }

        void open_Select(object sender, wx.Event e)
        {
            piperGUI.open();
        }

        void changeScene_Select(object sender, wx.Event e)
        {
            piperGUI.showChooseSceneDialog();
        }

        void recentDocMenuItem_Click(object sender, wx.Event e)
        {
            String document = menuIDsToFiles[e.ID];
            PatientDataFile patientData = new PatientDataFile(document);
            if (patientData.loadHeader())
            {
                piperGUI.changeActiveFile(patientData);
                standaloneController.openPatientFile(patientData);
            }
            else
            {
                MessageBox.show(String.Format("Error loading file {0}.", patientData.BackingFile), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        void recentDocuments_DocumentRemoved(RecentDocuments source, string document)
        {
            wx.MenuItem recentDocMenuItem;
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
            wx.MenuItem recentDocMenuItem;
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
                wx.MenuItem recentDocMenuItem = recentPatients.SubMenu.Insert(0, -1, Path.GetFileNameWithoutExtension(document));
                MainWindow.Instance.EVT_MENU(recentDocMenuItem.ID, recentDocMenuItem_Click);
                recentDocMenuItem.Help = document;
                menuIDsToFiles.Add(recentDocMenuItem.ID, document);
                recentDocMenuItems.Add(document, recentDocMenuItem);
            }
        }
    }
}
