using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical;
using MyGUIPlugin;
using System.IO;
using Medical.Presentation;

namespace PresentationEditor.GUI
{
    class SlideIndex : MDIDialog
    {
        private String windowTitle;
        private const String windowTitleFormat = "{0} - {1}";
        private const String Wildcard = "Anomalous Medical Presentation (*" + PresentationTypeController.PresentationExtension + ")|*" + PresentationTypeController.PresentationExtension + ";";

        //File Menu
        MenuBar menuBar;
        MenuItem newProject;
        MenuItem openProject;
        MenuItem closeProject;
        MenuItem saveAll;

        private ButtonGrid buttonGrid;
        private int lastWidth = -1;
        private int lastHeight = -1;

        private PresentationEditor presentationEditor;

        public SlideIndex(PresentationEditor presentationEditor)
            :base("PresentationEditor.GUI.SlideIndex.SlideIndex.layout")
        {
            this.presentationEditor = presentationEditor;
            presentationEditor.CurrentPresentationChanged += new Action<PresentationEditor>(presentationEditor_CurrentPresentationChanged);
            presentationEditor.SlideAdded += new Action<PresentationEntry>(presentationEditor_SlideAdded);
            presentationEditor.EntryRemoved += new Action<PresentationEntry>(presentationEditor_SlideRemoved);
            presentationEditor.SelectedEntryChanged += new Action<PresentationEditor>(presentationEditor_SelectedEntryChanged);

            windowTitle = window.Caption;
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            buttonGrid = new ButtonGrid((ScrollView)window.findWidget("FileTableScroll"), new ButtonGridListLayout());
            buttonGrid.SelectedValueChanged += new EventHandler(buttonGrid_SelectedValueChanged);

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
            if (presentationEditor.ResourceProvider == null || presentationEditor.ResourceProvider.ResourceCache.Count == 0)
            {
                showNewProjectDialog();
            }
            else
            {
                MessageBox.show("You have open files, would you like to save them before creating a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Ok)
                    {
                        presentationEditor.saveAllCachedResources();
                    }
                    showNewProjectDialog();
                });
            }
        }

        void showNewProjectDialog()
        {
            presentationEditor.stopPlayingTimelines();
            NewPresentationDialog.ShowDialog((fullProjectName) =>
            {
                if (Directory.Exists(fullProjectName))
                {
                    MessageBox.show(String.Format("The presentation {0} already exists. Would you like to delete it and create a new one?", fullProjectName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, result =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            presentationEditor.createNewProject(fullProjectName, true);
                        }
                    });
                }
                else
                {
                    presentationEditor.createNewProject(fullProjectName, false);
                }
            });
        }

        void openProjectClicked(Widget source, EventArgs e)
        {
            if (presentationEditor.ResourceProvider == null || presentationEditor.ResourceProvider.ResourceCache.Count == 0)
            {
                showOpenProjectDialog();
            }
            else
            {
                MessageBox.show("You have open files, would you like to save them before opening a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Ok)
                    {
                        presentationEditor.saveAllCachedResources();
                    }
                    showOpenProjectDialog();
                });
            }
        }

        void showOpenProjectDialog()
        {
            presentationEditor.stopPlayingTimelines();
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a project.", "", "", Wildcard, false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    presentationEditor.openProject(Path.GetDirectoryName(fileDialog.Path), fileDialog.Path);
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
                presentationEditor.saveAllCachedResources();
            }
            else if (menuEventArgs.Item == closeProject)
            {
                presentationEditor.closeProject();
            }
        }

        void presentationEditor_CurrentPresentationChanged(PresentationEditor presentationEditor)
        {
            buttonGrid.clear();
            buttonGrid.SuppressLayout = true;
            foreach (PresentationEntry entry in presentationEditor.Entries)
            {
                addEntryToButtonGrid(entry);
            }
            buttonGrid.SuppressLayout = false;
            buttonGrid.layout();
            if (presentationEditor.ResourceProvider != null)
            {
                window.Caption = String.Format(windowTitleFormat, windowTitle, presentationEditor.ResourceProvider.BackingLocation);
            }
            else
            {
                window.Caption = windowTitle;
            }
        }

        void presentationEditor_SlideRemoved(PresentationEntry obj)
        {
            removeEntryFromButtonGrid(obj);
        }

        void presentationEditor_SlideAdded(PresentationEntry obj)
        {
            addEntryToButtonGrid(obj);
        }

        private void addEntryToButtonGrid(PresentationEntry entry)
        {
            ButtonGridItem item = buttonGrid.addItem("", entry.UniqueName);
            item.UserObject = entry;
        }

        private void removeEntryFromButtonGrid(PresentationEntry entry)
        {
            ButtonGridItem item = buttonGrid.findItemByUserObject(entry);
            buttonGrid.removeItem(item);
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            //Layout only if size changes
            if (window.Width != lastWidth || window.Height != lastHeight)
            {
                lastWidth = window.Width;
                lastHeight = window.Height;
                buttonGrid.layout();
            }
        }

        bool allowSynchronization = true;
        void buttonGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            PresentationEntry entry = null;
            if (buttonGrid.SelectedItem != null)
            {
                entry = (PresentationEntry)buttonGrid.SelectedItem.UserObject;
            }
            synchronizeSelectedSlide(entry, buttonGrid);
        }

        void presentationEditor_SelectedEntryChanged(PresentationEditor obj)
        {
            synchronizeSelectedSlide(obj.SelectedEntry, obj);
        }

        private void synchronizeSelectedSlide(PresentationEntry entry, object sender)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (sender != presentationEditor)
                {
                    presentationEditor.SelectedEntry = entry;
                }
                if (sender != buttonGrid)
                {
                    buttonGrid.SelectedItem = buttonGrid.findItemByUserObject(entry);
                }
                allowSynchronization = true;
            }
        }
    }
}
