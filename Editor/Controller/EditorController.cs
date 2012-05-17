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

        public event EditorControllerEvent ProjectChanged;
        public event EditorControllerEvent ExtensionActionsChanged;

        private FileSystemWatcher fileWatcher = null;

        public EditorController(EditorPlugin plugin, StandaloneController standaloneController)
        {
            this.plugin = plugin;
            this.standaloneController = standaloneController;

            uiCallbackExtensions = new EditorUICallbackExtensions(standaloneController, plugin.MedicalUICallback, this);
        }

        public void Dispose()
        {
            if (fileWatcher != null)
            {
                fileWatcher.Dispose();
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
                projectChanged();
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
            ResourceProvider = new FilesystemResourceProvider(Path.GetDirectoryName(projectPath));
            projectChanged();
            //Add to recent documents
        }

        public void openFile(String file)
        {
            String fullPath = ResourceProvider.getFullFilePath(file);
            String extension = Path.GetExtension(file).ToLowerInvariant();

            EditorTypeController typeController;
            if (typeControllers.TryGetValue(extension, out typeController))
            {
                typeController.openFile(fullPath);
            }
            else
            {
                OtherProcessManager.openLocalURL(fullPath);
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

        public ResourceProvider ResourceProvider { get; private set; }

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
            ResourceProvider = new FilesystemResourceProvider(projectName);
            projectChanged();
        }

        private void projectChanged()
        {
            if (fileWatcher != null)
            {
                fileWatcher.Dispose();
            }
            plugin.TimelineController.setResourceProvider(ResourceProvider);
            BrowserWindowController.setResourceProvider(ResourceProvider);
            if (ProjectChanged != null)
            {
                ProjectChanged.Invoke(this);
            }
            String projectDirectory = ResourceProvider.BackingLocation;
            if (Directory.Exists(ResourceProvider.BackingLocation))
            {
                fileWatcher = new FileSystemWatcher(projectDirectory);
                fileWatcher.Changed += new FileSystemEventHandler(fileWatcher_Changed);
                fileWatcher.EnableRaisingEvents = true;
            }
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            EditorTypeController typeController;
            String extension = Path.GetExtension(e.FullPath);
            if (typeControllers.TryGetValue(extension, out typeController))
            {
                typeController.fileChanged(e, extension);
            }
        }
    }
}
