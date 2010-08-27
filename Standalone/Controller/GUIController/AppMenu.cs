using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Standalone;
using Engine;
using System.IO;

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

            changeSceneButton.MouseButtonClick += new MyGUIEvent(changeSceneButton_MouseButtonClick);
            openButton.MouseButtonClick += new MyGUIEvent(openButton_MouseButtonClick);
            saveButton.MouseButtonClick += new MyGUIEvent(saveButton_MouseButtonClick);
            saveAsButton.MouseButtonClick += new MyGUIEvent(saveAsButton_MouseButtonClick);
            quitButton.MouseButtonClick += new MyGUIEvent(quitButton_MouseButtonClick);

            recentDocumentsLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 0.0f, new Vector2(149, 14));
            recentDocuments.DocumentAdded += new RecentDocumentEvent(recentDocuments_DocumentAdded);
            recentDocuments.DocumentReaccessed += new RecentDocumentEvent(recentDocuments_DocumentReaccessed);
            recentDocuments.DocumentRemoved += new RecentDocumentEvent(recentDocuments_DocumentRemoved);
            recentDocumentsLayout.SuppressLayout = true;
            foreach (String document in recentDocuments)
            {
                Button recentDocButton = mainWidget.createWidgetT("Button", "AppMenuItemButton", 149, 14, 248, 20, Align.Left | Align.Top, document) as Button;
                recentDocButton.Caption = Path.GetFileNameWithoutExtension(document);
                recentDocButton.MouseButtonClick += recentDocButton_MouseButtonClick;
                MyGUILayoutContainer container = new MyGUILayoutContainer(recentDocButton);
                recentDocButton.UserObject = container;
                recentDocumentsLayout.addChild(container);
                //doc.ExtraText = Path.GetDirectoryName(document);
                recentDocsMap.Add(document, recentDocButton);
            }
            recentDocumentsLayout.SuppressLayout = false;
            recentDocumentsLayout.layout();
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

        void recentDocuments_DocumentRemoved(RecentDocuments source, string document)
        {
            Button doc;
            recentDocsMap.TryGetValue(document, out doc);
            if (doc != null)
            {
                recentDocumentsLayout.removeChild(doc.UserObject as LayoutContainer);
                recentDocsMap.Remove(document);
            }
        }

        void recentDocuments_DocumentReaccessed(RecentDocuments source, string document)
        {
            Button doc;
            recentDocsMap.TryGetValue(document, out doc);
            if (doc != null)
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
            Button recentDocButton = mainWidget.createWidgetT("Button", "AppMenuItemButton", 149, 14, 248, 20, Align.Left | Align.Top, document) as Button;
            recentDocButton.Caption = Path.GetFileNameWithoutExtension(document);
            recentDocButton.MouseButtonClick += recentDocButton_MouseButtonClick;
            MyGUILayoutContainer container = new MyGUILayoutContainer(recentDocButton);
            recentDocButton.UserObject = container;
            recentDocumentsLayout.insertChild(container, 0);
            //doc.ExtraText = Path.GetDirectoryName(document);
            recentDocsMap.Add(document, recentDocButton);
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
    }
}
