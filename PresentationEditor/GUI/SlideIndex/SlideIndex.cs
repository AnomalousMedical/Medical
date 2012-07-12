using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical;
using MyGUIPlugin;
using System.IO;

namespace PresentationEditor.GUI
{
    class SlideIndex : MDIDialog
    {
        private String windowTitle;
        private const String windowTitleFormat = "{0} - {1}";
        private const String Wildcard = "Anomalous Medical Presentation (*.amp)|*.amp;";

        //File Menu
        MenuBar menuBar;
        MenuItem newProject;
        MenuItem openProject;
        MenuItem closeProject;
        MenuItem saveAll;

        private ButtonGrid buttonGrid;

        private EditorController editorController;

        public SlideIndex(EditorController editorController)
            :base("PresentationEditor.GUI.SlideIndex.SlideIndex.layout")
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);

            windowTitle = window.Caption;

            buttonGrid = new ButtonGrid((ScrollView)window.findWidget("FileTableScroll"), new ButtonGridListLayout());

            //File Menu
            menuBar = window.findWidget("MenuBar") as MenuBar;
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            MenuControl fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            fileMenu.ItemAccept += new MyGUIEvent(fileMenu_ItemAccept);
            newProject = fileMenu.addItem("New Project");
            openProject = fileMenu.addItem("Open Project");
            closeProject = fileMenu.addItem("Close Project");
            saveAll = fileMenu.addItem("Save");
        }

        public override void Dispose()
        {
            buttonGrid.Dispose();
            base.Dispose();
        }

        void createNewProjectClicked(Widget source, EventArgs e)
        {
            //if (editorController.ResourceProvider == null || editorController.ResourceProvider.ResourceCache.Count == 0)
            //{
            //    showNewProjectDialog();
            //}
            //else
            //{
            //    MessageBox.show("You have open files, would you like to save them before creating a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
            //    {
            //        if (result == MessageBoxStyle.Ok)
            //        {
            //            editorController.saveAllCachedResources();
            //        }
            //        showNewProjectDialog();
            //    });
            //}
        }

        void showNewProjectDialog()
        {
            //editorController.stopPlayingTimelines();
            //NewProjectDialog.ShowDialog((template, fullProjectName) =>
            //{
            //    if (Directory.Exists(fullProjectName))
            //    {
            //        MessageBox.show(String.Format("The project {0} already exists. Would you like to delete it and create a new one?", fullProjectName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, result =>
            //        {
            //            if (result == MessageBoxStyle.Yes)
            //            {
            //                editorController.createNewProject(fullProjectName, true, template);
            //            }
            //        });
            //    }
            //    else
            //    {
            //        editorController.createNewProject(fullProjectName, false, template);
            //    }
            //});
        }

        void openProjectClicked(Widget source, EventArgs e)
        {
            if (editorController.ResourceProvider == null || editorController.ResourceProvider.ResourceCache.Count == 0)
            {
                showOpenProjectDialog();
            }
            else
            {
                MessageBox.show("You have open files, would you like to save them before opening a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Ok)
                    {
                        editorController.saveAllCachedResources();
                    }
                    showOpenProjectDialog();
                });
            }
        }

        void showOpenProjectDialog()
        {
            editorController.stopPlayingTimelines();
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a project.", "", "", Wildcard, false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    editorController.openProject(Path.GetDirectoryName(fileDialog.Path));
                }
            }
        }

        void fileMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs menuEventArgs = (MenuCtrlAcceptEventArgs)e;
            if (menuEventArgs.Item == newProject)
            {
                createNewProjectClicked(source, e);
            }
            else if (menuEventArgs.Item == openProject)
            {
                openProjectClicked(source, e);
            }
            else if (menuEventArgs.Item == saveAll)
            {
                editorController.saveAllCachedResources();
            }
            else if (menuEventArgs.Item == closeProject)
            {
                editorController.closeProject();
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            if (editorController.ResourceProvider != null)
            {
                window.Caption = String.Format(windowTitleFormat, windowTitle, editorController.ResourceProvider.BackingLocation);
            }
            else
            {
                window.Caption = windowTitle;
            }
        }
    }
}
