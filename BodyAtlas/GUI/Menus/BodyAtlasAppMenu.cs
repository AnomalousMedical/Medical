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
        private DocumentController documentController;
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
            this.documentController = standaloneController.DocumentController;

            Button changeSceneButton = widget.findWidget("File/ChangeScene") as Button;
            Button openButton = widget.findWidget("File/Open") as Button;
            Button saveButton = widget.findWidget("File/Save") as Button;
            Button saveAsButton = widget.findWidget("File/SaveAs") as Button;
            Button logoutButton = widget.findWidget("LogOut") as Button;
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
            logoutButton.MouseButtonClick += new MyGUIEvent(logoutButton_MouseButtonClick);
            quitButton.MouseButtonClick += new MyGUIEvent(quitButton_MouseButtonClick);

            recentDocumentsLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 0.0f, new Vector2(recentDocsLeft, recentDocsTop));
            documentController.DocumentAdded += new RecentDocumentEvent(recentDocuments_DocumentAdded);
            documentController.DocumentReaccessed += new RecentDocumentEvent(recentDocuments_DocumentReaccessed);
            documentController.DocumentRemoved += new RecentDocumentEvent(recentDocuments_DocumentRemoved);
            recentDocumentsLayout.SuppressLayout = true;
            foreach (String document in documentController.RecentDocuments)
            {
                addDocumentToMenu(document, false);
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

        void logoutButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Logging out will delete your local license file. This will require you to log in the next time you use this program.\nYou will also not be able to use the software in offline mode until you log back in and save your password.", "Log Out", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, 
                delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        standaloneController.App.LicenseManager.deleteLicense();
                        standaloneController.exit();
                    }
                });
            this.hide();
        }

        void quitButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.exit();
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
            addDocumentToMenu(document, true);
        }

        /// <summary>
        /// Add a document to the menu
        /// </summary>
        /// <param name="document">The doc to add</param>
        /// <param name="putStart">True to insert at the start false to put at the end</param>
        private void addDocumentToMenu(string document, bool putStart)
        {
            Button recentDocButton = widget.createWidgetT("Button", "AppMenuItemButton", recentDocsLeft, recentDocsTop, recentDocsWidth, recentDocsHeight, Align.Left | Align.Top, document) as Button;
            recentDocButton.Caption = Path.GetFileNameWithoutExtension(document);
            recentDocButton.MouseButtonClick += recentDocButton_MouseButtonClick;
            MyGUILayoutContainer container = new MyGUILayoutContainer(recentDocButton);
            recentDocButton.UserObject = container;
            if (putStart)
            {
                recentDocumentsLayout.insertChild(container, 0);
            }
            else
            {
                recentDocumentsLayout.addChild(container);
            }
            recentDocsMap.Add(document, recentDocButton);
        }

        void recentDocButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
            documentController.openFile(source.Name);
        }
    }
}
