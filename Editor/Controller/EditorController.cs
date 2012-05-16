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

namespace Medical
{
    public delegate void EditorControllerEvent(EditorController editorController);

    public class EditorController
    {
        private EditorPlugin plugin;
        private StandaloneController standaloneController;
        private SendResult<Object> browserResultCallback;
        private EditorUICallbackExtensions uiCallbackExtensions;

        public event EditorControllerEvent ProjectChanged;

        public EditorController(EditorPlugin plugin, StandaloneController standaloneController)
        {
            this.plugin = plugin;
            this.standaloneController = standaloneController;

            uiCallbackExtensions = new EditorUICallbackExtensions(standaloneController, plugin.MedicalUICallback, this);
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
            if (file.EndsWith(".rml"))
            {
                plugin.RmlViewer.changeDocument(fullPath);
                if(!plugin.RmlViewer.Visible)
                {
                    plugin.RmlViewer.open(false);
                }
            }
            else if (file.EndsWith(".mvc"))
            {
                plugin.MvcEditor.load(fullPath);
                if (!plugin.MvcEditor.Visible)
                {
                    plugin.MvcEditor.open(false);
                }
            }
            else if (file.EndsWith(".ddp"))
            {
                plugin.AtlasPluginEditor.loadPlugin(fullPath);
                if (!plugin.AtlasPluginEditor.Visible)
                {
                    plugin.AtlasPluginEditor.open(false);
                }
            }
            else if (file.EndsWith(".seq"))
            {
                plugin.MovementSequenceEditor.openSequence(fullPath);
                if (!plugin.MovementSequenceEditor.Visible)
                {
                    plugin.MovementSequenceEditor.open(false);
                }
            }
            else if (file.EndsWith(".tl"))
            {
                plugin.TimelineEditor.loadTimeline(fullPath);
                if (!plugin.TimelineEditor.Visible)
                {
                    plugin.TimelineEditor.open(false);
                }
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
            plugin.TimelineController.setResourceProvider(ResourceProvider);
            BrowserWindowController.setResourceProvider(ResourceProvider);
            if (ProjectChanged != null)
            {
                ProjectChanged.Invoke(this);
            }
        }
    }
}
