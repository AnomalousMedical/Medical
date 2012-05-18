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
        public event FileSystemEventHandler FileChanged;
        public event FileSystemEventHandler FileDeleted;
        public event FileSystemEventHandler FileCreated;
        public event RenamedEventHandler FileRenamed;

        public EditorController(EditorPlugin plugin, StandaloneController standaloneController)
        {
            this.plugin = plugin;
            this.standaloneController = standaloneController;

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
                //Add to recent documents
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
            projectChanged(Path.GetDirectoryName(projectPath));
            //Add to recent documents
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

        public Saveable loadObject(String filename)
        {
            return resourceProvider.openSaveable(filename);
        }

        public void saveObject(String filename, Saveable saveable)
        {
            resourceProvider.saveSaveable(filename, saveable);
        }

        public void addCachedResource(CachedResource cachedResource)
        {
            resourceProvider.ResourceCache.add(cachedResource);
        }

        public void removeCachedResource(String filename)
        {
            resourceProvider.ResourceCache.remove(filename);
        }

        public ResourceProvider ResourceProvider
        {
            get
            {
                return resourceProvider;
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
            resourceProvider.FileChanged += new FileSystemEventHandler(resourceProvider_FileChanged);
            resourceProvider.FileCreated += new FileSystemEventHandler(resourceProvider_FileCreated);
            resourceProvider.FileDeleted += new FileSystemEventHandler(resourceProvider_FileDeleted);
            resourceProvider.FileRenamed += new RenamedEventHandler(resourceProvider_FileRenamed);
            plugin.TimelineController.setResourceProvider(ResourceProvider);
            BrowserWindowController.setResourceProvider(ResourceProvider);
            if (ProjectChanged != null)
            {
                ProjectChanged.Invoke(this);
            }
        }

        void resourceProvider_FileRenamed(object sender, RenamedEventArgs e)
        {
            if (FileRenamed != null)
            {
                FileRenamed.Invoke(sender, e);
            }
        }

        void resourceProvider_FileDeleted(object sender, FileSystemEventArgs e)
        {
            if (FileDeleted != null)
            {
                FileDeleted.Invoke(sender, e);
            }
        }

        void resourceProvider_FileCreated(object sender, FileSystemEventArgs e)
        {
            if (FileCreated != null)
            {
                FileCreated.Invoke(sender, e);
            }
        }

        void resourceProvider_FileChanged(object sender, FileSystemEventArgs e)
        {
            if (FileChanged != null)
            {
                FileChanged.Invoke(sender, e);
            }
        }
    }
}
