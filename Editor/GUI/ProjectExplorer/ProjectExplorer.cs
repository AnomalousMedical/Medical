using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;

namespace Medical.GUI
{
    class ProjectExplorer : MDIDialog
    {
        private String windowTitle;
        private const String windowTitleFormat = "{0} - {1}";

        //File Menu
        MenuItem newProject;
        MenuItem openProject;
        MenuItem newTimelineItem;
        MenuItem saveTimelineItem;
        MenuItem saveTimelineAsItem;
        MenuItem deleteItem;

        Tree fileTree;

        //Dialogs
        private NewProjectDialog newProjectDialog;
        private SaveTimelineDialog saveTimelineDialog;

        private EditorController editorController;

        public ProjectExplorer(EditorController editorController)
            :base("Medical.GUI.ProjectExplorer.ProjectExplorer.layout")
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);

            windowTitle = window.Caption;
            MenuBar menuBar = window.findWidget("MenuBar") as MenuBar;

            fileTree = new Tree((ScrollView)window.findWidget("FileTableScroll"));
            fileTree.NodeMouseDoubleClick += new EventHandler<TreeEventArgs>(fileTree_NodeMouseDoubleClick);

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

            //Dialogs
            newProjectDialog = new NewProjectDialog();
            newProjectDialog.ProjectCreated += new EventHandler(newProjectDialog_ProjectCreated);

            saveTimelineDialog = new SaveTimelineDialog();
            saveTimelineDialog.SaveFile += new EventHandler(saveTimelineDialog_SaveFile);

            this.Resized += new EventHandler(ProjectExplorer_Resized);
        }

        public override void Dispose()
        {
            fileTree.Dispose();
            newProjectDialog.Dispose();
            saveTimelineDialog.Dispose();
            base.Dispose();
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
            editorController.stopPlayingTimelines();
            newProjectDialog.open(true);
            newProjectDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            newProjectDialog.ensureVisible();
        }

        void newProjectDialog_ProjectCreated(object sender, EventArgs e)
        {
            editorController.createNewProject(newProjectDialog.FullProjectName, false);
        }

        void openProjectClicked(Widget source, EventArgs e)
        {
            editorController.stopPlayingTimelines();
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a project.", "", "", "", false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    editorController.openProject(fileDialog.Path);
                }
            }
        }

        void newTimelineClicked(Widget source, EventArgs e)
        {
            //stopTimelineIfPlaying();
            //timelinePropertiesController.createNewTimeline();
        }

        void saveTimelineClicked(Widget source, EventArgs e)
        {
            //stopTimelineIfPlaying();
            //if (timelinePropertiesController.CurrentTimelineFile != null)
            //{
            //    timelinePropertiesController.saveTimeline(timelinePropertiesController.CurrentTimeline, timelinePropertiesController.CurrentTimelineFile);
            //}
            //else
            //{
            //    saveTimelineAsClicked(source, e);
            //}
        }

        void saveTimelineAsClicked(Widget source, EventArgs e)
        {
            //stopTimelineIfPlaying();
            //saveTimelineDialog.open(true);
            //saveTimelineDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            //saveTimelineDialog.ensureVisible();
        }

        void deleteClicked(Widget source, EventArgs e)
        {
            //if (fileList.hasItemSelected())
            //{
            //    String filename = (String)fileList.getItemDataAt(fileList.getIndexSelected());
            //    MessageBox.show(String.Format("Are you sure you want to delete {0}?", filename), "Overwrite?", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, delegate(MessageBoxStyle result)
            //    {
            //        if (result == MessageBoxStyle.Yes)
            //        {
            //            timelinePropertiesController.deleteFile(filename);
            //            updateFileList();
            //        }
            //    });
            //}
        }

        void saveTimelineDialog_SaveFile(object sender, EventArgs e)
        {
            //String filename = saveTimelineDialog.Filename;
            //if (timelineController.resourceExists(filename))
            //{
            //    MessageBox.show(String.Format("The file {0} already exists. Would you like to overwrite it?", filename), "Overwrite?", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, delegate(MessageBoxStyle result)
            //    {
            //        if (result == MessageBoxStyle.Yes)
            //        {
            //            timelinePropertiesController.saveTimeline(timelinePropertiesController.CurrentTimeline, saveTimelineDialog.Filename);
            //            updateFileList();
            //        }
            //    });
            //}
            //else
            //{
            //    timelinePropertiesController.saveTimeline(timelinePropertiesController.CurrentTimeline, filename);
            //    updateFileList();
            //}
        }

        //void fileList_ListSelectAccept(Widget source, EventArgs e)
        //{
        //    stopTimelineIfPlaying();
        //    uint selectedIndex = fileList.getIndexSelected();
        //    if (selectedIndex != uint.MaxValue)
        //    {
        //        timelinePropertiesController.openTimelineFile(fileList.getItemDataAt(selectedIndex).ToString());
        //    }
        //    else
        //    {
        //        MessageBox.show("Please select a file to open.", "Warning", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
        //    }
        //}

        void updateFileList()
        {
            //fileList.removeAllItems();
            //String[] files = timelineController.LEGACY_listResourceFiles("*.tl");
            //foreach (String file in files)
            //{
            //    String fileName = Path.GetFileName(file);
            //    fileList.addItem(fileName, fileName);
            //}
        }

        void fileTree_NodeMouseDoubleClick(object sender, TreeEventArgs e)
        {
            if(e.Node != null && e.Node.Children.Count == 0)
            {
                editorController.openFile(e.Node.UserData.ToString());
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            fileTree.Nodes.clear();
            ResourceProvider resourceProvider = editorController.ResourceProvider;
            if (resourceProvider != null)
            {
                window.Caption = String.Format(windowTitleFormat, windowTitle, resourceProvider.BackingLocation);
                //Update the file list
                fileTree.SuppressLayout = true;
                String[] files = resourceProvider.listFiles("*", "", true);
                foreach (String file in files)
                {
                    String fileName = Path.GetFileName(file);
                    String filePath = Path.GetDirectoryName(file);
                    TreeNodeCollection parentNodeCollection = fileTree.Nodes;
                    if (!String.IsNullOrEmpty(filePath))
                    {
                        filePath = filePath.Replace('\\', '/');
                        int i;
                        String directoryName;
                        String fullDirectoryPath;
                        TreeNode dirNode = null;
                        int slashLoc = 0;
                        for (i = 0; i < filePath.Length; i = slashLoc + 1)
                        {
                            slashLoc = filePath.IndexOf('/', i);
                            if (slashLoc > 0)
                            {
                                directoryName = filePath.Substring(i, slashLoc - i);
                                fullDirectoryPath = filePath.Substring(0, slashLoc);
                                dirNode = computeDirectoryNode(parentNodeCollection, directoryName, fullDirectoryPath);
                                parentNodeCollection = dirNode.Children;
                            }
                            else
                            {
                                break;
                            }
                        }
                        directoryName = filePath.Substring(i);
                        dirNode = computeDirectoryNode(parentNodeCollection, directoryName, filePath);
                        parentNodeCollection = dirNode.Children;
                    }
                    TreeNode fileNode = new TreeNode(fileName);
                    fileNode.UserData = file;
                    parentNodeCollection.add(fileNode);
                }
                fileTree.SuppressLayout = false;
                fileTree.layout();
            }
            else
            {
                window.Caption = windowTitle;
            }
        }

        private TreeNode computeDirectoryNode(TreeNodeCollection parentNodeCollection, String nodeName, String fullDirectoryPath)
        {
            TreeNode dirNode = null;
            //Look for existing node with the directory name
            foreach (TreeNode node in parentNodeCollection)
            {
                if (node.Text == nodeName)
                {
                    dirNode = node;
                    break;
                }
            }
            if (dirNode == null)
            {
                dirNode = new TreeNode(nodeName);
                dirNode.UserData = fullDirectoryPath;
                parentNodeCollection.add(dirNode);
            }
            return dirNode;
        }

        void ProjectExplorer_Resized(object sender, EventArgs e)
        {
            fileTree.layout();
        }
    }
}
