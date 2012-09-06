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
    public class ProjectExplorer : MDIDialog
    {
        private String windowTitle;
        private const String windowTitleFormat = "{0} - {1}";

        //File Menu
        MenuBar menuBar;
        MenuItem newProject;
        MenuItem openProject;
        MenuItem closeProject;
        MenuItem saveAll;

        private FileBrowserTree fileBrowser;

        //Dialogs
        private EditorController editorController;

        public ProjectExplorer(EditorController editorController)
            :base("Medical.GUI.ProjectExplorer.ProjectExplorer.layout")
        {
            this.editorController = editorController;
            editorController.ProjectChanged += editorController_ProjectChanged;

            windowTitle = window.Caption;
            menuBar = window.findWidget("MenuBar") as MenuBar;

            fileBrowser = new FileBrowserTree((ScrollView)window.findWidget("FileTableScroll"));
            fileBrowser.FileSelected += new FileBrowserEvent(fileBrowser_FileSelected);
            fileBrowser.NodeContextEvent += new FileBrowserNodeContextEvent(fileBrowser_NodeContextEvent);

            //File Menu
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            MenuControl fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            fileMenu.ItemAccept += new MyGUIEvent(fileMenu_ItemAccept);
            newProject = fileMenu.addItem("New Project");
            openProject = fileMenu.addItem("Open Project");
            closeProject = fileMenu.addItem("Close Project");
            saveAll = fileMenu.addItem("Save All");

            this.Resized += new EventHandler(ProjectExplorer_Resized);
        }

        public override void Dispose()
        {
            fileBrowser.Dispose();
            base.Dispose();
        }

        void createNewProjectClicked(Widget source, EventArgs e)
        {
            if (editorController.ResourceProvider == null || editorController.ResourceProvider.ResourceCache.Count == 0)
            {
                showNewProjectDialog();
            }
            else
            {
                MessageBox.show("You have open files, would you like to save them before creating a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Ok)
                    {
                        editorController.saveAllCachedResources();
                    }
                    showNewProjectDialog();
                });
            }
        }

        void showNewProjectDialog()
        {
            editorController.stopPlayingTimelines();
            NewProjectDialog.ShowDialog((template, fullProjectName) =>
            {
                if (Directory.Exists(fullProjectName))
                {
                    MessageBox.show(String.Format("The project {0} already exists. Would you like to delete it and create a new one?", fullProjectName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, result =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            editorController.createNewProject(fullProjectName, true, template);
                        }
                    });
                }
                else
                {
                    editorController.createNewProject(fullProjectName, false, template);
                }
            });
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
            FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a project.", "", "", "", false);
            fileDialog.showModal((result, paths) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    String path = paths.First();
                    editorController.openProject(Path.GetDirectoryName(path), path);
                }
            });
        }

        void editorController_ProjectChanged(EditorController editorController, String defaultFile)
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
            editorController.openEditor(path);
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

                contextMenu.add(new ContextMenuItem("Add Item", path, delegate(ContextMenuItem item)
                {
                    AddItemDialog.AddItem(editorController.ItemTemplates, (itemTemplate) =>
                        {
                            ((ProjectItemTemplate)itemTemplate).createItem(path, editorController);
                        });
                }));

                contextMenu.add(new ContextMenuItem("Import Files", path, delegate(ContextMenuItem item)
                {
                    FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Choose files to import.", "", "", "", true);
                    fileDialog.showModal((result, paths) =>
                    {
                        if (result == NativeDialogResult.OK)
                        {
                            editorController.importFiles(paths, item.UserObject.ToString());
                        }
                    });
                }));

                contextMenu.add(new ContextMenuItem("Explore To", path, item =>
                    {
                        OtherProcessManager.openLocalURL(editorController.ResourceProvider.getFullFilePath(item.UserObject.ToString()));
                    }));
            }
            if (!isTopLevel)
            {
                contextMenu.add(new ContextMenuItem("Rename", path, delegate(ContextMenuItem item)
                {
                    InputBox.GetInput("Rename", String.Format("Please enter a new name for {0}.", item.UserObject.ToString()), true, delegate(String result, ref String errorPrompt)
                    {
                        String originalExtension = Path.GetExtension(item.UserObject.ToString());
                        String targetName = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(item.UserObject.ToString()), result), originalExtension);
                        if (editorController.ResourceProvider.exists(targetName))
                        {
                            errorPrompt = String.Format("A file named {0} already exists. Please enter another name.", targetName);
                            return false;
                        }

                        editorController.ResourceProvider.move(path, targetName);
                        return true;
                    });
                }));
                contextMenu.add(new ContextMenuItem("Delete", path, delegate(ContextMenuItem item)
                {
                    MessageBox.show(String.Format("Are you sure you want to delete {0}?", item.UserObject.ToString()), "Delete?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            editorController.ResourceProvider.delete(item.UserObject.ToString());
                        }
                    });
                }));
            }
            tree.showContextMenu(contextMenu);
        }
    }
}
