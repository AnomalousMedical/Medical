using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Logging;
using MyGUIPlugin;
using Medical.Editor;

namespace Medical
{
    delegate void EditorControllerEvent(EditorController editorController);

    class EditorController
    {
        private EditorPlugin plugin;
        private StandaloneController standaloneController;

        public event EditorControllerEvent ProjectChanged;

        public EditorController(EditorPlugin plugin, StandaloneController standaloneController)
        {
            this.plugin = plugin;
            this.standaloneController = standaloneController;
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
                
                //if (!asFolder)
                //{
                //    documentController.addToRecentDocuments(filename);
                //}
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

            //if (projectPath.EndsWith(".tl"))
            //{
            //    openTimelineFile(projectPath);
            //}
        }

        internal void openFile(String file)
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

        public ResourceProvider ResourceProvider { get; private set; }

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
