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
using Medical.Controller.AnomalousMvc;

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

        private StandaloneController standaloneController;
        private Dictionary<String, EditorTypeController> typeControllers = new Dictionary<String, EditorTypeController>();
        private EditorResourceProvider resourceProvider;
        private TimelineController timelineController;

        public event EditorControllerEvent ProjectChanged;

        public EditorController(StandaloneController standaloneController, TimelineController timelineController)
        {
            this.timelineController = timelineController;
            this.standaloneController = standaloneController;
            standaloneController.DocumentController.addDocumentHandler(new ProjectDocumentHandler(this));
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

        public void createNewProject(String filename, bool deleteOld, ProjectTemplate projectTemplate)
        {
            try
            {
                if (deleteOld)
                {
                    //Make sure the old project is closed first, this prevents problems deleting the currently open project.
                    closeResourceProvider();
                    Directory.Delete(filename, true);
                }
                if (!Directory.Exists(filename))
                {
                    Directory.CreateDirectory(filename);
                }
                projectChanged(filename);
                projectTemplate.createProject(ResourceProvider, Path.GetFileName(filename));
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
        }

        public void closeProject()
        {
            closeResourceProvider();
            timelineController.setResourceProvider(null);
            BrowserWindowController.setResourceProvider(null);

            if (ProjectChanged != null)
            {
                ProjectChanged.Invoke(this);
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
                NotificationManager.showNotification("All files saved", "Editor/SaveAllIcon", 2);
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

        public void closeFile(String file)
        {
            String extension = Path.GetExtension(file).ToLowerInvariant();

            EditorTypeController typeController;
            if (typeControllers.TryGetValue(extension, out typeController))
            {
                typeController.closeFile(file);
            }
        }

        public void stopPlayingTimelines()
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback();
            }
        }

        public void addCachedResource(CachedResource cachedResource)
        {
            resourceProvider.ResourceCache.add(cachedResource);
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

        public NotificationGUIManager NotificationManager
        {
            get
            {
                return standaloneController.GUIManager.NotificationManager;
            }
        }

        public IEnumerable<String> OpenFiles
        {
            get
            {
                if (resourceProvider != null)
                {
                    return resourceProvider.ResourceCache.OpenFiles;
                }
                return new String[0];
            }
        }

        public int OpenFileCount
        {
            get
            {
                if (resourceProvider != null)
                {
                    return resourceProvider.ResourceCache.Count;
                }
                return 0;
            }
        }

        public void runEditorContext(AnomalousMvcContext mvcContext)
        {
            mvcContext.setResourceProvider(resourceProvider);
            mvcContext.RuntimeName = "Editor.CurrentEditor";
            standaloneController.MvcCore.startRunningContext(mvcContext);
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

        private void projectChanged(String projectPath)
        {
            closeResourceProvider();
            resourceProvider = new EditorResourceProvider(projectPath);
            timelineController.setResourceProvider(ResourceProvider);
            BrowserWindowController.setResourceProvider(ResourceProvider);

            if (ProjectChanged != null)
            {
                ProjectChanged.Invoke(this);
            }
        }

        private void closeResourceProvider()
        {
            if (resourceProvider != null)
            {
                resourceProvider.Dispose();
                resourceProvider = null;
            }
        }
    }
}
