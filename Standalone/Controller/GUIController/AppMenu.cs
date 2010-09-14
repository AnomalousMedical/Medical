using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Standalone;
using Engine;
using System.IO;
using System.Reflection;

namespace Medical.GUI
{
    class AppMenu : IDisposable
    {
        private Layout layout;
        private PopupContainer popupContainer;
        private PiperJBOGUI piperGUI;
        private StandaloneController standaloneController;

        private Dictionary<String, Button> recentDocsMap = new Dictionary<string, Button>();
        private RecentDocuments recentDocuments = MedicalConfig.RecentDocuments;
        private FlowLayoutContainer recentDocumentsLayout;
        private Widget mainWidget;

        private int recentDocsLeft;
        private int recentDocsTop;
        private int recentDocsWidth;
        private int recentDocsHeight;

        public AppMenu(PiperJBOGUI piperGUI, StandaloneController standaloneController)
        {
            this.piperGUI = piperGUI;
            this.standaloneController = standaloneController;

            layout = LayoutManager.Instance.loadLayout("Medical.Controller.GUIController.AppMenu.layout");
            mainWidget = layout.getWidget(0);
            mainWidget.Visible = false;
            popupContainer = new PopupContainer(mainWidget);

            Button changeSceneButton = mainWidget.findWidget("File/ChangeScene") as Button;
            Button openButton = mainWidget.findWidget("File/Open") as Button;
            Button saveButton = mainWidget.findWidget("File/Save") as Button;
            Button saveAsButton = mainWidget.findWidget("File/SaveAs") as Button;
            Button quitButton = mainWidget.findWidget("File/Quit") as Button;
            Widget recentDocsHorizDivider = mainWidget.findWidget("RecentDocsHorizDivider");
            Widget recentDocsVertDivider = mainWidget.findWidget("RecentDocsVertDivider");

            recentDocsLeft = recentDocsHorizDivider.Left + recentDocsHorizDivider.Width + 1;
            recentDocsTop = recentDocsVertDivider.Top + recentDocsVertDivider.Height + 1;
            recentDocsWidth = recentDocsVertDivider.Width;
            recentDocsHeight = 20;

            changeSceneButton.MouseButtonClick += new MyGUIEvent(changeSceneButton_MouseButtonClick);
            openButton.MouseButtonClick += new MyGUIEvent(openButton_MouseButtonClick);
            saveButton.MouseButtonClick += new MyGUIEvent(saveButton_MouseButtonClick);
            saveAsButton.MouseButtonClick += new MyGUIEvent(saveAsButton_MouseButtonClick);
            quitButton.MouseButtonClick += new MyGUIEvent(quitButton_MouseButtonClick);

            recentDocumentsLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 0.0f, new Vector2(recentDocsLeft, recentDocsTop));
            recentDocuments.DocumentAdded += new RecentDocumentEvent(recentDocuments_DocumentAdded);
            recentDocuments.DocumentReaccessed += new RecentDocumentEvent(recentDocuments_DocumentReaccessed);
            recentDocuments.DocumentRemoved += new RecentDocumentEvent(recentDocuments_DocumentRemoved);
            recentDocumentsLayout.SuppressLayout = true;
            foreach (String document in recentDocuments)
            {
                addDocumentToMenu(document);
            }
            recentDocumentsLayout.SuppressLayout = false;
            recentDocumentsLayout.layout();

            //Help
            Button helpButton = mainWidget.findWidget("Help") as Button;
            helpButton.MouseButtonClick += new MyGUIEvent(helpButton_MouseButtonClick);

            //About
            Button aboutButton = mainWidget.findWidget("About") as Button;
            aboutButton.MouseButtonClick += new MyGUIEvent(aboutButton_MouseButtonClick);

            //Update
            Button updateButton = mainWidget.findWidget("Updates") as Button;
            updateButton.MouseButtonClick += new MyGUIEvent(updateButton_MouseButtonClick);

            //Options
            Button optionsButton = mainWidget.findWidget("Options") as Button;
            optionsButton.MouseButtonClick += new MyGUIEvent(optionsButton_MouseButtonClick);
        }

        public void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
        }

        public void show(int x, int y)
        {
            popupContainer.show(x, y);
        }

        void changeSceneButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.showChooseSceneDialog();
            popupContainer.hide();
        }

        void saveButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.save();
            popupContainer.hide();
        }

        void saveAsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.saveAs();
            popupContainer.hide();
        }

        void openButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.open();
            popupContainer.hide();
        }

        void quitButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.shutdown();
            popupContainer.hide();
        }

        void optionsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.showOptions();
            popupContainer.hide();
        }

        void aboutButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.showAboutDialog();
            popupContainer.hide();
        }

        void updateButton_MouseButtonClick(Widget source, EventArgs e)
        {
            UpdateManager.checkForUpdates(Assembly.GetAssembly(this.GetType()).GetName().Version);
            popupContainer.hide();
        }

        void helpButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.openHelpTopic(0);
            popupContainer.hide();
        }

        void recentDocuments_DocumentRemoved(RecentDocuments source, string document)
        {
            Button doc;
            recentDocsMap.TryGetValue(document, out doc);
            if (doc != null)
            {
                recentDocumentsLayout.removeChild(doc.UserObject as LayoutContainer);
                recentDocsMap.Remove(document);
                Gui.Instance.destroyWidget(doc);
            }

#if CREATE_MAINWINDOW_MENU
            windowMenuDocumentRemoved(document);
#endif
        }

        void recentDocuments_DocumentReaccessed(RecentDocuments source, string document)
        {
            Button doc;
            if (recentDocsMap.TryGetValue(document, out doc))
            {
                LayoutContainer layout = (LayoutContainer)doc.UserObject;
                recentDocumentsLayout.SuppressLayout = true;
                recentDocumentsLayout.removeChild(layout);
                recentDocumentsLayout.SuppressLayout = false;
                recentDocumentsLayout.insertChild(layout, 0);
            }

#if CREATE_MAINWINDOW_MENU
            windowMenuDocumentReaccessed(document);
#endif
        }

        void recentDocuments_DocumentAdded(RecentDocuments source, string document)
        {
            addDocumentToMenu(document);
        }

        private void addDocumentToMenu(string document)
        {
            Button recentDocButton = mainWidget.createWidgetT("Button", "AppMenuItemButton", recentDocsLeft, recentDocsTop, recentDocsWidth, recentDocsHeight, Align.Left | Align.Top, document) as Button;
            recentDocButton.Caption = Path.GetFileNameWithoutExtension(document);
            recentDocButton.MouseButtonClick += recentDocButton_MouseButtonClick;
            MyGUILayoutContainer container = new MyGUILayoutContainer(recentDocButton);
            recentDocButton.UserObject = container;
            recentDocumentsLayout.insertChild(container, 0);
            recentDocsMap.Add(document, recentDocButton);

#if CREATE_MAINWINDOW_MENU
            createWindowMenuDocument(document);
#endif
        }

        void recentDocButton_MouseButtonClick(Widget source, EventArgs e)
        {
            popupContainer.hide();
            PatientDataFile patientData = new PatientDataFile(source.Name);
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

#if CREATE_MAINWINDOW_MENU

        private wx.MenuItem recentPatients;
        private Dictionary<String, wx.MenuItem> recentDocMenuItems = new Dictionary<string, wx.MenuItem>();
        private Dictionary<int, string> menuIDsToFiles = new Dictionary<int, string>();

        private wx.Menu fileMenu;
        private wx.MenuItem changeScene;
        private wx.MenuItem open;
        private wx.MenuItem save;
        private wx.MenuItem saveAs;
        private wx.MenuItem exit;

        public void createMenus(wx.MenuBar menu)
        {
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

            save = fileMenu.Append((int)wx.MenuIDs.wxID_SAVE, "&Save...\tCtrl+S", "Save current distortions.");
            save.Select += new wx.EventListener(save_Select);

            saveAs = fileMenu.Append((int)wx.MenuIDs.wxID_SAVEAS, "Save &As...", "Save current distortions as.");
            saveAs.Select += new wx.EventListener(saveAs_Select);

            fileMenu.AppendSeparator();

            exit = fileMenu.Append((int)wx.MenuIDs.wxID_EXIT, "&Exit", "Exit the program.");
            exit.Select += new wx.EventListener(exit_Select);

            menu.Append(fileMenu, "&File");
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

        private void createWindowMenuDocument(string document)
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

        private void windowMenuDocumentReaccessed(string document)
        {
            wx.MenuItem recentDocMenuItem;
            if (recentDocMenuItems.TryGetValue(document, out recentDocMenuItem))
            {
                recentPatients.SubMenu.Remove(recentDocMenuItem);
                recentPatients.SubMenu.Insert(0, recentDocMenuItem);
            }
        }

        private void windowMenuDocumentRemoved(string document)
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
#endif
    }
}
