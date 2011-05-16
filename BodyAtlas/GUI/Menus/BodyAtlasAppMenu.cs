using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using System.Reflection;

namespace Medical.GUI
{
    class BodyAtlasAppMenu : PopupContainer, AppMenu
    {
        private BodyAtlasMainPlugin bodyAtlasGUI;
        private StandaloneController standaloneController;

        private Dictionary<String, Button> recentDocsMap = new Dictionary<string, Button>();
        private RecentDocuments recentDocuments;
        private FlowLayoutContainer recentDocumentsLayout;

        private int recentDocsLeft;
        private int recentDocsTop;
        private int recentDocsWidth;
        private int recentDocsHeight;

        public BodyAtlasAppMenu(BodyAtlasMainPlugin piperGUI, StandaloneController standaloneController)
            :base("Medical.GUI.Menus.BodyAtlasAppMenu.layout")
        {
            this.bodyAtlasGUI = piperGUI;
            this.standaloneController = standaloneController;
            this.recentDocuments = piperGUI.RecentDocuments;

            Button changeSceneButton = widget.findWidget("File/ChangeScene") as Button;
            Button openButton = widget.findWidget("File/Open") as Button;
            Button saveButton = widget.findWidget("File/Save") as Button;
            Button saveAsButton = widget.findWidget("File/SaveAs") as Button;
            Button quitButton = widget.findWidget("File/Quit") as Button;
            Widget recentDocsHorizDivider = widget.findWidget("RecentDocsHorizDivider");
            Widget recentDocsVertDivider = widget.findWidget("RecentDocsVertDivider");

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
            Button helpButton = widget.findWidget("Help") as Button;
            helpButton.MouseButtonClick += new MyGUIEvent(helpButton_MouseButtonClick);

            //About
            Button aboutButton = widget.findWidget("About") as Button;
            aboutButton.MouseButtonClick += new MyGUIEvent(aboutButton_MouseButtonClick);

            //Update
            Button updateButton = widget.findWidget("Updates") as Button;
            updateButton.MouseButtonClick += new MyGUIEvent(updateButton_MouseButtonClick);

            //Options
            Button optionsButton = widget.findWidget("Options") as Button;
            optionsButton.MouseButtonClick += new MyGUIEvent(optionsButton_MouseButtonClick);
        }

        void changeSceneButton_MouseButtonClick(Widget source, EventArgs e)
        {
            bodyAtlasGUI.showChooseSceneDialog();
            this.hide();
        }

        void saveButton_MouseButtonClick(Widget source, EventArgs e)
        {
            bodyAtlasGUI.save();
            this.hide();
        }

        void saveAsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            bodyAtlasGUI.saveAs();
            this.hide();
        }

        void openButton_MouseButtonClick(Widget source, EventArgs e)
        {
            bodyAtlasGUI.open();
            this.hide();
        }

        void quitButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.closeMainWindow();
            this.hide();
        }

        void optionsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            bodyAtlasGUI.showOptions();
            this.hide();
        }

        void aboutButton_MouseButtonClick(Widget source, EventArgs e)
        {
            bodyAtlasGUI.showAboutDialog();
            this.hide();
        }

        void updateButton_MouseButtonClick(Widget source, EventArgs e)
        {
            UpdateManager.checkForUpdates(Assembly.GetAssembly(this.GetType()).GetName().Version, standaloneController.App.ProductID);
            this.hide();
        }

        void helpButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.openHelpTopic(0);
            this.hide();
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
        }

        void recentDocuments_DocumentAdded(RecentDocuments source, string document)
        {
            addDocumentToMenu(document);
        }

        private void addDocumentToMenu(string document)
        {
            Button recentDocButton = widget.createWidgetT("Button", "AppMenuItemButton", recentDocsLeft, recentDocsTop, recentDocsWidth, recentDocsHeight, Align.Left | Align.Top, document) as Button;
            recentDocButton.Caption = Path.GetFileNameWithoutExtension(document);
            recentDocButton.MouseButtonClick += recentDocButton_MouseButtonClick;
            MyGUILayoutContainer container = new MyGUILayoutContainer(recentDocButton);
            recentDocButton.UserObject = container;
            recentDocumentsLayout.insertChild(container, 0);
            recentDocsMap.Add(document, recentDocButton);
        }

        void recentDocButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
            PatientDataFile patientData = new PatientDataFile(source.Name);
            if (patientData.loadHeader())
            {
                bodyAtlasGUI.changeActiveFile(patientData);
                standaloneController.openPatientFile(patientData);
            }
            else
            {
                MessageBox.show(String.Format("Error loading file {0}.", patientData.BackingFile), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }
    }
}
