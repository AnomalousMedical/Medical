using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using Engine.Platform;

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
        MenuItem saveAll;
        ExtensionActionCollection currentExtensionActions = null;

        private FileBrowserTree fileBrowser;

        //Dialogs
        private NewProjectDialog newProjectDialog;

        private EditorController editorController;

        public ProjectExplorer(EditorController editorController)
            :base("Medical.GUI.ProjectExplorer.ProjectExplorer.layout")
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
            editorController.ExtensionActionsChanged += new EditorControllerEvent(editorController_ExtensionActionsChanged);

            windowTitle = window.Caption;
            menuBar = window.findWidget("MenuBar") as MenuBar;

            fileBrowser = new FileBrowserTree((ScrollView)window.findWidget("FileTableScroll"));
            fileBrowser.FileSelected += new FileBrowserEvent(fileBrowser_FileSelected);
            fileBrowser.NodeContextEvent += new FileBrowserNodeContextEvent(fileBrowser_NodeContextEvent);

            rebuildMenus();

            //Dialogs
            newProjectDialog = new NewProjectDialog();
            newProjectDialog.ProjectCreated += new EventHandler(newProjectDialog_ProjectCreated);

            this.Resized += new EventHandler(ProjectExplorer_Resized);
        }

        public override void Dispose()
        {
            fileBrowser.Dispose();
            newProjectDialog.Dispose();
            base.Dispose();
        }

        void createNewProjectClicked(Widget source, EventArgs e)
        {
            if (editorController.ResourceProvider == null || editorController.ResourceProvider.ResourceCache.Count == 0)
            {
                showNewProjectDialog(source.AbsoluteLeft, source.AbsoluteTop);
            }
            else
            {
                MessageBox.show("You have open files, would you like to save them before creating a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Ok)
                    {
                        editorController.saveAllCachedResources();
                    }
                    showNewProjectDialog(source.AbsoluteLeft, source.AbsoluteTop);
                });
            }
        }

        void showNewProjectDialog(int x, int y)
        {
            editorController.stopPlayingTimelines();
            newProjectDialog.open(true);
            newProjectDialog.Position = new Vector2(x, y);
            newProjectDialog.ensureVisible();
        }

        void newProjectDialog_ProjectCreated(object sender, EventArgs e)
        {
            editorController.createNewProject(newProjectDialog.FullProjectName, false);
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
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a project.", "", "", "", false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    editorController.openProject(fileDialog.Path);
                }
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            fileBrowser.setResourceProvider(editorController.ResourceProvider);
            if (editorController.ResourceProvider != null)
            {
                window.Caption = String.Format(windowTitleFormat, windowTitle, editorController.ResourceProvider.BackingLocation);
                editorController.ResourceProvider.FileCreated += new ResourceProviderFileEvent(ResourceProvider_FileCreated);
                editorController.ResourceProvider.FileDeleted += new ResourceProviderFileDeletedEvent(ResourceProvider_FileDeleted);
                editorController.ResourceProvider.FileRenamed += new ResourceProviderFileRenamedEvent(ResourceProvider_FileRenamed);
            }
            else
            {
                window.Caption = windowTitle;
            }
        }

        void ProjectExplorer_Resized(object sender, EventArgs e)
        {
            fileBrowser.layout();
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

            saveAll = fileMenu.addItem("Save All");
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

        void editorController_ExtensionActionsChanged(EditorController editorController)
        {
            currentExtensionActions = editorController.ExtensionActions;
            rebuildMenus();
        }

        void ResourceProvider_FileCreated(string path, bool isDirectory)
        {
            fileBrowser.fileCreated(path, isDirectory);
        }

        void ResourceProvider_FileRenamed(string path, string oldPath, bool isDirectory)
        {
            fileBrowser.fileRenamed(path, oldPath, isDirectory);
        }

        void ResourceProvider_FileDeleted(string path)
        {
            fileBrowser.fileDeleted(path);
        }

        void fileBrowser_FileSelected(FileBrowserTree tree, string path)
        {
            editorController.openFile(path);
        }

        void fileBrowser_NodeContextEvent(FileBrowserTree tree, string path, bool isDirectory, bool isTopLevel)
        {
            ContextMenu contextMenu = new ContextMenu();
            if (isDirectory)
            {
                contextMenu.add(new ContextMenuItem("Create Directory", path, delegate(ContextMenuItem item)
                {
                    InputBox.GetInput("Directory Name", "Please enter a name for the directory.", true, delegate(String result, ref String errorPrompt)
                    {
                        editorController.ResourceProvider.createDirectory(item.UserObject.ToString(), result);
                        return true;
                    });
                }));

                foreach (EditorTypeController typeController in editorController.TypeControllers)
                {
                    typeController.addCreationMethod(contextMenu, path, isDirectory, isTopLevel);
                }
            }
            if (!isTopLevel)
            {
                contextMenu.add(new ContextMenuItem("Delete", path, delegate(ContextMenuItem item)
                {
                    MessageBox.show(String.Format("Are you sure you want to delete {0}?", item.UserObject.ToString()), "Delete?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            editorController.ResourceProvider.deleteFile(item.UserObject.ToString());
                        }
                    });
                }));
            }
            tree.showContextMenu(contextMenu);
        }
    }
}
