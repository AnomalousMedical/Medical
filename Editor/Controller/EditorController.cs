using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Logging;
using MyGUIPlugin;
using Medical.Editor;
using Engine.Editing;
using Medical.GUI;
using Engine.Saving.XMLSaver;
using Engine.Saving;
using System.Xml;

namespace Medical
{
    public delegate void EditorControllerEvent(EditorController editorController);

    public class EditorController : IDisposable
    {
        private static XmlSaver xmlSaver = new XmlSaver();

        public static XmlSaver XmlSaver
        {
            get
            {
                return xmlSaver;
            }
        }

        private EditorPlugin plugin;
        private StandaloneController standaloneController;
        private SendResult<Object> browserResultCallback;
        private EditorUICallbackExtensions uiCallbackExtensions;
        private ExtensionActionCollection extensionActions;
        private Dictionary<String, EditorTypeController> typeControllers = new Dictionary<String, EditorTypeController>();
        private EditorResourceProvider resourceProvider;

        public event EditorControllerEvent ProjectChanged;
        public event EditorControllerEvent ExtensionActionsChanged;

        public EditorController(EditorPlugin plugin, StandaloneController standaloneController)
        {
            this.plugin = plugin;
            this.standaloneController = standaloneController;
            standaloneController.DocumentController.addDocumentHandler(new ProjectDocumentHandler(this));

            uiCallbackExtensions = new EditorUICallbackExtensions(standaloneController, plugin.MedicalUICallback, this);
        }

        public void Dispose()
        {
            if (resourceProvider != null)
            {
                resourceProvider.Dispose();
            }
        }

        public void addTypeController(EditorTypeController typeController)
        {
            typeControllers.Add(typeController.Extension, typeController);
        }

        public void createNewProject(String filename, bool deleteOld)
        {
            try
            {
                if (deleteOld)
                {
                    File.Delete(filename);
                }
                createProject(filename);
                standaloneController.DocumentController.addToRecentDocuments(filename);
            }
            catch (Exception ex)
            {
                String errorMessage = String.Format("Error creating new project {0}.", ex.Message);
                Log.Error(errorMessage);
                MessageBox.show(errorMessage, "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        public void openProject(String projectPath)
        {
            projectChanged(projectPath);
            standaloneController.DocumentController.addToRecentDocuments(projectPath);
            if (!plugin.ProjectExplorer.Visible)
            {
                plugin.ProjectExplorer.open(false);
            }
        }

        public void saveAllCachedResources()
        {
            if (resourceProvider != null)
            {
                foreach (CachedResource resource in resourceProvider.ResourceCache.Resources)
                {
                    resource.save();
                }
            }
        }

        public void openFile(String file)
        {
            String extension = Path.GetExtension(file).ToLowerInvariant();

            EditorTypeController typeController;
            if (typeControllers.TryGetValue(extension, out typeController))
            {
                typeController.openFile(file);
            }
            else
            {
                OtherProcessManager.openLocalURL(ResourceProvider.getFullFilePath(file));
            }
        }

        public void stopPlayingTimelines()
        {
            if (plugin.TimelineController.Playing)
            {
                plugin.TimelineController.stopPlayback();
            }
        }

        public void showBrowser(Browser browser, SendResult<Object> browserResultCallback)
        {
            this.browserResultCallback = browserResultCallback;

            BrowserWindow browserWindow = plugin.BrowserWindow;
            browserWindow.setBrowser(browser);
            browserWindow.ItemSelected += browserWindow_ItemSelected;
            browserWindow.Canceled += browserWindow_Canceled;
            browserWindow.open(true);
        }

        public void addCachedResource(CachedResource cachedResource)
        {
            resourceProvider.ResourceCache.add(cachedResource);
        }

        public void removeCachedResource(String filename)
        {
            resourceProvider.ResourceCache.remove(filename);
        }

        public void importFiles(LinkedList<String> files, String targetPath)
        {
            Queue<String> conflictedFiles = new Queue<string>();
            foreach (String file in files)
            {
                if (resourceProvider.exists(Path.Combine(targetPath, Path.GetFileName(file))))
                {
                    conflictedFiles.Enqueue(file);
                }
                else
                {
                    resourceProvider.addFile(file, targetPath);
                }
            }
            importConflictedFiles(conflictedFiles, targetPath);
        }

        public EditorResourceProvider ResourceProvider
        {
            get
            {
                return resourceProvider;
            }
        }

        public IEnumerable<EditorTypeController> TypeControllers
        {
            get
            {
                return typeControllers.Values;
            }
        }

        public ExtensionActionCollection ExtensionActions
        {
            get
            {
                return extensionActions;
            }
            set
            {
                if (this.extensionActions != value)
                {
                    this.extensionActions = value;
                    if (ExtensionActionsChanged != null)
                    {
                        ExtensionActionsChanged.Invoke(this);
                    }
                }
            }
        }

        private void importConflictedFiles(Queue<String> conflictedFiles, String targetPath)
        {
            if (conflictedFiles.Count > 0)
            {
                String file = conflictedFiles.Dequeue();
                MessageBox.show(String.Format("A file named {0} already exists in {1}.\nWould you like to overwrite it?", file, targetPath), "Overwrite", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        resourceProvider.addFile(file, targetPath);
                    }
                    importConflictedFiles(conflictedFiles, targetPath);
                });
            }
        }

        void browserWindow_ItemSelected(object sender, EventArgs e)
        {
            BrowserWindow browserWindow = plugin.BrowserWindow;
            if (browserResultCallback != null)
            {
                String error = null;
                browserResultCallback.Invoke(browserWindow.SelectedValue, ref error);
                browserResultCallback = null;
            }
            browserWindow.ItemSelected -= browserWindow_ItemSelected;
            browserWindow.Canceled -= browserWindow_Canceled;
        }

        void browserWindow_Canceled(object sender, EventArgs e)
        {
            BrowserWindow browserWindow = plugin.BrowserWindow;
            browserResultCallback = null;
            browserWindow.ItemSelected -= browserWindow_ItemSelected;
            browserWindow.Canceled -= browserWindow_Canceled;
        }

        private void createProject(string projectName)
        {
            if (!Directory.Exists(projectName))
            {
                Directory.CreateDirectory(projectName);
            }
            projectChanged(projectName);
        }

        private void projectChanged(String projectPath)
        {
            if (resourceProvider != null)
            {
                resourceProvider.Dispose();
            }
            resourceProvider = new EditorResourceProvider(projectPath);
            plugin.TimelineController.setResourceProvider(ResourceProvider);
            BrowserWindowController.setResourceProvider(ResourceProvider);
            if (ProjectChanged != null)
            {
                ProjectChanged.Invoke(this);
            }
        }
    }
}
