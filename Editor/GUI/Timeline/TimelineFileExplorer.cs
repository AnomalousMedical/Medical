﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;

namespace Medical.GUI
{
    class TimelineFileExplorer : MDIDialog
    {
        public const String PROJECT_WILDCARD = "Timelines (*.tl)|*.tl";

        //File Menu
        MenuItem newProject;
        MenuItem openProject;
        MenuItem newTimelineItem;
        MenuItem saveTimelineItem;
        MenuItem saveTimelineAsItem;
        MenuItem deleteItem;

        //Edit Menu
        MenuItem selectAll;
        MenuItem cut;
        MenuItem copy;
        MenuItem paste;

        //File list
        private MultiListBox fileList;

        //Dialogs
        private NewProjectDialog newProjectDialog;
        private SaveTimelineDialog saveTimelineDialog;

        private MenuItem editMenuItem;
        private bool allMenusEnabled = true;
        private TimelineController timelineController;
        private DocumentController documentController;
        private TimelinePropertiesController timelinePropertiesController;

        public TimelineFileExplorer(TimelineController timelineController, DocumentController documentController, TimelinePropertiesController timelinePropertiesController)
            : base("Medical.GUI.Timeline.TimelineFileExplorer.layout")
        {
            this.timelineController = timelineController;
            timelineController.ResourceLocationChanged += new EventHandler(timelineController_ResourceLocationChanged);
            this.documentController = documentController;
            this.timelinePropertiesController = timelinePropertiesController;

            MenuBar menuBar = window.findWidget("MenuBar") as MenuBar;

            //File Menu
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            MenuControl fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            fileMenu.ItemAccept += new MyGUIEvent(fileMenu_ItemAccept);
            newProject = fileMenu.addItem("New Project");
            openProject = fileMenu.addItem("Open Project");
            fileMenu.addItem("", MenuItemType.Separator);
            newTimelineItem = fileMenu.addItem("New Timeline");
            saveTimelineItem = fileMenu.addItem("Save Timeline");
            saveTimelineAsItem = fileMenu.addItem("Save Timeline As");
            fileMenu.addItem("", MenuItemType.Separator);
            deleteItem = fileMenu.addItem("Delete Selected");

            //Edit menu
            editMenuItem = menuBar.addItem("Edit", MenuItemType.Popup);
            MenuControl editMenu = menuBar.createItemPopupMenuChild(editMenuItem);
            editMenu.ItemAccept += new MyGUIEvent(editMenu_ItemAccept);
            selectAll = editMenu.addItem("Select All");
            cut = editMenu.addItem("Cut");
            copy = editMenu.addItem("Copy");
            paste = editMenu.addItem("Paste");

            AllMenusEnabled = false;

            //File list
            fileList = window.findWidget("FileList") as MultiListBox;
            fileList.addColumn("File", 50);
            fileList.setColumnResizingPolicyAt(0, ResizingPolicy.Fill);
            fileList.ListSelectAccept += new MyGUIEvent(fileList_ListSelectAccept);

            //Dialogs
            newProjectDialog = new NewProjectDialog();
            newProjectDialog.ProjectCreated += new EventHandler(newProjectDialog_ProjectCreated);

            saveTimelineDialog = new SaveTimelineDialog();
            saveTimelineDialog.SaveFile += new EventHandler(saveTimelineDialog_SaveFile);
        }

        public override void Dispose()
        {
            newProjectDialog.Dispose();
            saveTimelineDialog.Dispose();
            base.Dispose();
        }

        public void save()
        {
            saveTimelineClicked(window, EventArgs.Empty);
        }

        public bool AllMenusEnabled
        {
            get
            {
                return allMenusEnabled;
            }
            set
            {
                allMenusEnabled = value;
                newTimelineItem.Enabled = value;
                saveTimelineItem.Enabled = value;
                saveTimelineAsItem.Enabled = value;
                editMenuItem.Enabled = value;
            }
        }

        #region File Menu

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
            else if (menuEventArgs.Item == newTimelineItem)
            {
                newTimelineClicked(source, e);
            }
            else if (menuEventArgs.Item == saveTimelineItem)
            {
                saveTimelineClicked(source, e);
            }
            else if (menuEventArgs.Item == saveTimelineAsItem)
            {
                saveTimelineAsClicked(source, e);
            }
            else if (menuEventArgs.Item == deleteItem)
            {
                deleteClicked(source, e);
            }
        }

        void createNewProjectClicked(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            newProjectDialog.open(true);
            newProjectDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            newProjectDialog.ensureVisible();
        }

        void newProjectDialog_ProjectCreated(object sender, EventArgs e)
        {
            timelinePropertiesController.createNewProject(newProjectDialog.FullProjectName, false);
        }

        void openProjectClicked(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a timeline.", newProjectDialog.ProjectLocation, "", PROJECT_WILDCARD, false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    timelinePropertiesController.openProject(fileDialog.Path);
                    documentController.addToRecentDocuments(fileDialog.Path);
                }
            }
        }

        void newTimelineClicked(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            timelinePropertiesController.createNewTimeline();
        }

        void saveTimelineClicked(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            if (timelinePropertiesController.CurrentTimelineFile != null)
            {
                timelinePropertiesController.saveTimeline(timelinePropertiesController.CurrentTimeline, timelinePropertiesController.CurrentTimelineFile);
            }
            else
            {
                saveTimelineAsClicked(source, e);
            }
        }

        void saveTimelineAsClicked(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            saveTimelineDialog.open(true);
            saveTimelineDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            saveTimelineDialog.ensureVisible();
        }

        void deleteClicked(Widget source, EventArgs e)
        {
            if (fileList.hasItemSelected())
            {
                String filename = (String)fileList.getItemDataAt(fileList.getIndexSelected());
                MessageBox.show(String.Format("Are you sure you want to delete {0}?", filename), "Overwrite?", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        timelinePropertiesController.deleteFile(filename);
                        updateFileList();
                    }
                });
            }
        }

        void saveTimelineDialog_SaveFile(object sender, EventArgs e)
        {
            String filename = saveTimelineDialog.Filename;
            if (timelineController.resourceExists(filename))
            {
                MessageBox.show(String.Format("The file {0} already exists. Would you like to overwrite it?", filename), "Overwrite?", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        timelinePropertiesController.saveTimeline(timelinePropertiesController.CurrentTimeline, saveTimelineDialog.Filename);
                        updateFileList();
                    }
                });
            }
            else
            {
                timelinePropertiesController.saveTimeline(timelinePropertiesController.CurrentTimeline, filename);
                updateFileList();
            }
        }

        #endregion

        #region Edit Menu

        void editMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs menuEventArgs = (MenuCtrlAcceptEventArgs)e;
            if (menuEventArgs.Item == copy)
            {
                timelinePropertiesController.copy();
            }
            else if (menuEventArgs.Item == paste)
            {
                timelinePropertiesController.paste();
            }
            else if (menuEventArgs.Item == cut)
            {
                timelinePropertiesController.cut();
            }
            else if (menuEventArgs.Item == selectAll)
            {
                timelinePropertiesController.selectAll();
            }
        }

        #endregion

        private void stopTimelineIfPlaying()
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback(false);
            }
        }

        void timelineController_ResourceLocationChanged(object sender, EventArgs e)
        {
            AllMenusEnabled = timelineController.ResourceProvider != null;
            updateFileList();
            updateWindowCaption();
        }

        void fileList_ListSelectAccept(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            uint selectedIndex = fileList.getIndexSelected();
            if (selectedIndex != uint.MaxValue)
            {
                timelinePropertiesController.openTimelineFile(fileList.getItemDataAt(selectedIndex).ToString());
            }
            else
            {
                MessageBox.show("Please select a file to open.", "Warning", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
            }
        }

        void updateFileList()
        {
            fileList.removeAllItems();
            String[] files = timelineController.listResourceFiles("*.tl");
            foreach (String file in files)
            {
                String fileName = Path.GetFileName(file);
                fileList.addItem(fileName, fileName);
            }
        }

        public void updateWindowCaption()
        {
            if (timelineController.ResourceProvider != null)
            {
                window.Caption = String.Format("Timeline - {0}", timelineController.ResourceProvider.BackingLocation);
            }
            else
            {
                window.Caption = "Timeline";
            }
        }
    }
}
