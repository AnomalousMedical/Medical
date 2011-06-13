using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Engine.Saving.XMLSaver;
using MyGUIPlugin;
using Logging;

namespace Medical.GUI
{
    class TimelinePropertiesController : IDisposable
    {
        private TimelineProperties timelineProperties;
        private TimelineObjectEditor timelineObjectEditor;
        private TimelineFileExplorer timelineFileExplorer;

        private TimelineController editorTimelineController;
        private TimelineDocumentHandler documentHandler;
        private DocumentController documentController;

        public TimelinePropertiesController(StandaloneController standaloneController, EditorPlugin editorPlugin)
        {
            GUIManager guiManager = standaloneController.GUIManager;
            editorTimelineController = editorPlugin.TimelineController;

            this.documentController = standaloneController.DocumentController;
            documentHandler = new TimelineDocumentHandler(this);
            documentController.addDocumentHandler(documentHandler);

            timelineProperties = new TimelineProperties(editorTimelineController, standaloneController.TimelineController, editorPlugin, guiManager, standaloneController.DocumentController);
            guiManager.addManagedDialog(timelineProperties);

            timelineObjectEditor = new TimelineObjectEditor();
            guiManager.addManagedDialog(timelineObjectEditor);

            timelineFileExplorer = new TimelineFileExplorer(editorTimelineController, standaloneController.DocumentController, this);
            guiManager.addManagedDialog(timelineFileExplorer);
        }

        public void Dispose()
        {
            documentController.removeDocumentHandler(documentHandler);
            timelineObjectEditor.Dispose();
            timelineFileExplorer.Dispose();
            timelineProperties.Dispose();
        }

        /// <summary>
        /// Create a new project. You can optionally delete the old project.
        /// </summary>
        /// <param name="filename">The file name of the new project.</param>
        /// <param name="deleteOld">True to delete any existing project first.</param>
        public void createNewProject(String filename, bool deleteOld)
        {
            try
            {
                if (deleteOld)
                {
                    File.Delete(filename);
                }
                createProject(filename);
                updateWindowCaption();
                documentController.addToRecentDocuments(filename);
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
            if (projectPath.EndsWith(".tix"))
            {
                editorTimelineController.ResourceProvider = new FilesystemTimelineResourceProvider(Path.GetDirectoryName(projectPath));
            }
            else if (projectPath.EndsWith(".tl"))
            {
                editorTimelineController.ResourceProvider = new FilesystemTimelineResourceProvider(Path.GetDirectoryName(projectPath));
                openTimelineFile(projectPath);
            }
            else
            {
                editorTimelineController.ResourceProvider = new TimelineZipResources(projectPath);
            }
            updateWindowCaption();
        }

        public void openTimelineFile(String filename)
        {
            try
            {
                Timeline timeline = editorTimelineController.openTimeline(filename);
                setCurrentTimeline(timeline);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error loading timeline {0}.\n{1}", filename, ex.Message), "Load Timeline Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        public void saveTimeline(Timeline timeline, String filename)
        {
            editorTimelineController.saveTimeline(timeline, filename);
            //changeTimelineFile(filename);
        }

        public void createNewTimeline()
        {
            Timeline timeline = new Timeline();
            setCurrentTimeline(timeline);
        }

        public TimelineProperties TimelineProperties
        {
            get
            {
                return timelineProperties;
            }
        }

        public TimelineObjectEditor TimelineObjectEditor
        {
            get
            {
                return timelineObjectEditor;
            }
        }

        public TimelineFileExplorer TimelineFileExplorer
        {
            get
            {
                return timelineFileExplorer;
            }
        }

        private void setCurrentTimeline(Timeline timeline)
        {
            timelineProperties.setCurrentTimeline(timeline);
            timelineFileExplorer.setCurrentTimeline(timeline);
        }

        private void createProject(string projectName)
        {
            using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(projectName))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    XmlTextWriter xmlWriter = new XmlTextWriter(memStream, Encoding.Default);
                    xmlWriter.Formatting = Formatting.Indented;
                    TimelineIndex index = new TimelineIndex();
                    XmlSaver xmlSaver = new XmlSaver();
                    xmlSaver.saveObject(index, xmlWriter);
                    xmlWriter.Flush();
                    memStream.Seek(0, SeekOrigin.Begin);
                    ionicZip.AddEntry(TimelineController.INDEX_FILE_NAME, memStream);
                    ionicZip.Save();
                }
            }
            editorTimelineController.ResourceProvider = new TimelineZipResources(projectName);
        }

        private void updateWindowCaption()
        {
            //if (timelineController.ResourceProvider != null)
            //{
            //    if (currentTimelineFile != null)
            //    {
            //        window.Caption = String.Format("Timeline - {0} - {1}", Path.GetFileName(currentTimelineFile), timelineController.ResourceProvider.BackingLocation);
            //    }
            //    else
            //    {
            //        window.Caption = String.Format("Timeline - {0}", timelineController.ResourceProvider.BackingLocation);
            //    }
            //}
            //else
            //{
            //    window.Caption = "Timeline";
            //}
        }
    }
}
