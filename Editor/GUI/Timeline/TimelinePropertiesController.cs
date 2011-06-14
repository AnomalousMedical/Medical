using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Engine.Saving.XMLSaver;
using MyGUIPlugin;
using Logging;
using Engine.Editing;

namespace Medical.GUI
{
    class TimelinePropertiesController : IDisposable
    {
        private TimelineProperties timelineProperties;
        private TimelineObjectProperties timelineObjectProperties;
        private TimelineFileExplorer timelineFileExplorer;
        private TimelineObjectExplorer timelineObjectExplorer;
        private MedicalUICallback medicalUICallback;
        private TimelineUICallbackExtensions uiCallbackExtensions;
        private ObjectEditor timelineObjectEditor;
        private BrowserWindow browserWindow;
        private QuestionEditor questionEditor;
        private TimelineFileBrowserDialog fileBrowserDialog;

        private TimelineController editorTimelineController;
        private TimelineController mainTimelineController;
        private TimelineDocumentHandler documentHandler;
        private DocumentController documentController;

        private Timeline currentTimeline;

        private bool visible = false;

        public TimelinePropertiesController(StandaloneController standaloneController, EditorPlugin editorPlugin)
        {
            mainTimelineController = standaloneController.TimelineController;

            GUIManager guiManager = standaloneController.GUIManager;
            editorTimelineController = editorPlugin.TimelineController;

            this.documentController = standaloneController.DocumentController;
            documentHandler = new TimelineDocumentHandler(this);
            documentController.addDocumentHandler(documentHandler);

            fileBrowserDialog = new TimelineFileBrowserDialog(editorTimelineController, "TimelineFileBrowserDialog__Main");
            editorTimelineController.FileBrowser = fileBrowserDialog;
            guiManager.addManagedDialog(fileBrowserDialog);

            timelineProperties = new TimelineProperties(editorTimelineController, editorPlugin, guiManager, this, fileBrowserDialog);
            guiManager.addManagedDialog(timelineProperties);

            timelineFileExplorer = new TimelineFileExplorer(editorTimelineController, standaloneController.DocumentController, this);
            guiManager.addManagedDialog(timelineFileExplorer);

            browserWindow = new BrowserWindow();
            guiManager.addManagedDialog(browserWindow);

            questionEditor = new QuestionEditor(fileBrowserDialog, editorTimelineController);
            guiManager.addManagedDialog(questionEditor);

            medicalUICallback = new MedicalUICallback(browserWindow);
            uiCallbackExtensions = new TimelineUICallbackExtensions(medicalUICallback, editorTimelineController, browserWindow, questionEditor);

            timelineObjectExplorer = new TimelineObjectExplorer(medicalUICallback);
            guiManager.addManagedDialog(timelineObjectExplorer);

            timelineObjectProperties = new TimelineObjectProperties();
            guiManager.addManagedDialog(timelineObjectProperties);

            timelineObjectEditor = new ObjectEditor(timelineObjectExplorer.EditInterfaceTree, timelineObjectProperties.PropertiesTable, medicalUICallback);

            createNewTimeline();
        }

        public void Dispose()
        {
            documentController.removeDocumentHandler(documentHandler);
            timelineObjectProperties.Dispose();
            timelineFileExplorer.Dispose();
            timelineProperties.Dispose();
            timelineObjectExplorer.Dispose();
            questionEditor.Dispose();
            browserWindow.Dispose();
            fileBrowserDialog.Dispose();
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
                CurrentTimeline = editorTimelineController.openTimeline(filename);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error loading timeline {0}.\n{1}", filename, ex.Message), "Load Timeline Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        public void saveTimeline(Timeline timeline, String filename)
        {
            editorTimelineController.saveTimeline(timeline, filename);
            updateWindowCaption();
        }

        public void createNewTimeline()
        {
            CurrentTimeline = new Timeline();
        }

        public void playPreview(float startTime)
        {
            if (editorTimelineController.Playing)
            {
                editorTimelineController.stopPlayback(false);
            }
            else if (currentTimeline != null)
            {
                editorTimelineController.startPlayback(currentTimeline, startTime, false);
            }
        }

        public void playFull()
        {
            if (currentTimeline != null)
            {
                mainTimelineController.ResourceProvider = editorTimelineController.ResourceProvider.clone();
                mainTimelineController.startPlayback(currentTimeline, true);
            }
        }

        public void copy()
        {
            timelineProperties.copy();
        }

        public void paste()
        {
            timelineProperties.paste();
        }

        public Timeline CurrentTimeline
        {
            get
            {
                return currentTimeline;
            }
            set
            {
                if (currentTimeline != value)
                {
                    if (currentTimeline != null)
                    {
                        currentTimeline.ActionAdded -= currentTimeline_ActionAdded;
                        currentTimeline.ActionRemoved -= currentTimeline_ActionRemoved;
                    }
                    currentTimeline = value;
                    editorTimelineController.EditingTimeline = currentTimeline;
                    timelineProperties.setCurrentTimeline(currentTimeline);
                    timelineObjectEditor.EditInterface = currentTimeline.getEditInterface();
                    if (currentTimeline != null)
                    {
                        currentTimeline.ActionAdded += currentTimeline_ActionAdded;
                        currentTimeline.ActionRemoved += currentTimeline_ActionRemoved;
                    }
                }
            }
        }

        public String CurrentTimelineFile
        {
            get
            {
                return currentTimeline != null ? currentTimeline.SourceFile : null;
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    timelineProperties.Visible = value;
                    timelineFileExplorer.Visible = value;
                    timelineObjectExplorer.Visible = value;
                    timelineObjectProperties.Visible = value;
                }
            }
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
            timelineFileExplorer.updateWindowCaption();
            timelineProperties.updateWindowCaption();
        }

        void currentTimeline_ActionRemoved(object sender, TimelineActionEventArgs e)
        {
            timelineProperties.removeActionFromTimeline(e.Action);
        }

        void currentTimeline_ActionAdded(object sender, TimelineActionEventArgs e)
        {
            timelineProperties.addActionToTimeline(e.Action);
        }
    }
}
