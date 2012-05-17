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
        MenuBar menuBar;
        MenuItem newProject;
        MenuItem openProject;
        ExtensionActionCollection currentExtensionActions = null;

        Tree fileTree;

        //Dialogs
        private NewProjectDialog newProjectDialog;

        private EditorController editorController;

        public ProjectExplorer(EditorController editorController)
            :base("Medical.GUI.ProjectExplorer.ProjectExplorer.layout")
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
            editorController.ExtensionActionsChanged += new EditorControllerEvent(editorController_ExtensionActionsChanged);
            editorController.FileCreated += new FileSystemEventHandler(editorController_FileCreated);
            editorController.FileDeleted += new FileSystemEventHandler(editorController_FileDeleted);
            editorController.FileRenamed += new RenamedEventHandler(editorController_FileRenamed);

            windowTitle = window.Caption;
            menuBar = window.findWidget("MenuBar") as MenuBar;

            fileTree = new Tree((ScrollView)window.findWidget("FileTableScroll"));
            fileTree.NodeMouseDoubleClick += new EventHandler<TreeEventArgs>(fileTree_NodeMouseDoubleClick);

            rebuildMenus();

            //Dialogs
            newProjectDialog = new NewProjectDialog();
            newProjectDialog.ProjectCreated += new EventHandler(newProjectDialog_ProjectCreated);

            this.Resized += new EventHandler(ProjectExplorer_Resized);
        }

        public override void Dispose()
        {
            fileTree.Dispose();
            newProjectDialog.Dispose();
            base.Dispose();
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

        private void rebuildMenus()
        {
            menuBar.removeAllItems();
            //File Menu
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            MenuControl fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            fileMenu.ItemAccept += new MyGUIEvent(fileMenu_ItemAccept);
            newProject = fileMenu.addItem("New Project");
            openProject = fileMenu.addItem("Open Project");

            if (currentExtensionActions != null)
            {
                Dictionary<String, MenuControl> menus = new Dictionary<string, MenuControl>();
                menus.Add("File", fileMenu);

                foreach (ExtensionAction action in currentExtensionActions)
                {
                    MenuControl menu;
                    if (!menus.TryGetValue(action.Category, out menu))
                    {
                        MenuItem menuItem = menuBar.addItem(action.Category, MenuItemType.Popup);
                        menu = menuBar.createItemPopupMenuChild(menuItem);
                        menu.ItemAccept += new MyGUIEvent(menu_ItemAccept);
                        menus.Add(action.Category, menu);
                    }
                    MenuItem item = menu.addItem(action.Name);
                    item.UserObject = action;
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
            else
            {
                menu_ItemAccept(source, e);
            }
        }

        void menu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            ExtensionAction action = mcae.Item.UserObject as ExtensionAction;
            if (action != null)
            {
                action.execute();
            }
        }

        void editorController_FileRenamed(object sender, RenamedEventArgs e)
        {

        }

        void editorController_FileDeleted(object sender, FileSystemEventArgs e)
        {

        }

        void editorController_FileCreated(object sender, FileSystemEventArgs e)
        {

        }

        void editorController_ExtensionActionsChanged(EditorController editorController)
        {
            currentExtensionActions = editorController.ExtensionActions;
            rebuildMenus();
        }
    }
}
