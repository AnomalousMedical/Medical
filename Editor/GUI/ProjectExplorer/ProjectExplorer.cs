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
            ProjectExplorerFileNode fileNode = e.Node as ProjectExplorerFileNode;
            if(fileNode != null)
            {
                if (editorController.ResourceProvider.exists(fileNode.FilePath))
                {
                    editorController.openFile(fileNode.FilePath);
                }
            }
        }

        public void createNodesForPath(TreeNodeCollection parentCollection, String path)
        {
            ResourceProvider resourceProvider = editorController.ResourceProvider;
            if (resourceProvider != null)
            {
                fileTree.SuppressLayout = true;
                String[] directories = resourceProvider.listDirectories("*", path, false);
                foreach (String dir in directories)
                {
                    parentCollection.add(new ProjectExplorerDirectoryNode(dir, this));
                }
                String[] files = resourceProvider.listFiles("*", path, false);
                foreach (String file in files)
                {
                    parentCollection.add(new ProjectExplorerFileNode(file));
                }
                fileTree.SuppressLayout = false;
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            fileTree.Nodes.clear();
            if (editorController.ResourceProvider != null)
            {
                createNodesForPath(fileTree.Nodes, "");
                fileTree.layout();
            }
            else
            {
                window.Caption = windowTitle;
            }
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
