using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Medical.Presentation;
using System.IO;
using Medical.Controller.AnomalousMvc;

namespace PresentationEditor
{
    class PresentationEditor
    {
        public event Action<PresentationEntry> SlideAdded;
        public event Action<PresentationEntry> EntryRemoved;

        private static readonly PresentationProjectTemplate template = new PresentationProjectTemplate();

        private EditorController editorController;
        private PresentationIndex currentPresentation;
        private PresentationEntry selectedEntry;
        private StandaloneController standaloneController;

        public event Action<PresentationEditor> CurrentPresentationChanged;
        public event Action<PresentationEditor> SelectedEntryChanged;

        public PresentationEditor(EditorController editorController, StandaloneController standaloneController)
        {
            this.editorController = editorController;
            this.standaloneController = standaloneController;
            editorController.ProjectChanged += editorController_ProjectChanged;
        }

        public EditorResourceProvider ResourceProvider
        {
            get
            {
                return editorController.ResourceProvider;
            }
        }

        public bool HasPresentation
        {
            get
            {
                return currentPresentation != null;
            }
        }

        public IEnumerable<PresentationEntry> Entries
        {
            get
            {
                if (currentPresentation != null)
                {
                    return currentPresentation.Entries;
                }
                return EmptyIterator.Empty<PresentationEntry>();
            }
        }

        public PresentationEntry SelectedEntry
        {
            get
            {
                return selectedEntry;
            }
            set
            {
                if (selectedEntry != value)
                {
                    selectedEntry = value;
                    if (selectedEntry != null)
                    {
                        editorController.openEditor(value.File);
                    }
                    if (SelectedEntryChanged != null)
                    {
                        SelectedEntryChanged.Invoke(this);
                    }
                }
            }
        }

        void editorController_ProjectChanged(EditorController editorController, string fullFilePath)
        {
            selectedEntry = null;
            if (editorController.ResourceProvider != null)
            {
                this.currentPresentation = editorController.loadFile<PresentationIndex>(Path.GetFileName(fullFilePath));
                standaloneController.DocumentController.addToRecentDocuments(fullFilePath);
            }
            else
            {
                this.currentPresentation = null;
            }
            
            if (CurrentPresentationChanged != null)
            {
                CurrentPresentationChanged.Invoke(this);
            }

            if (currentPresentation != null && currentPresentation.Count > 0)
            {
                SelectedEntry = currentPresentation[0];
            }
        }

        public void stopPlayingTimelines()
        {
            editorController.stopPlayingTimelines();
        }

        public void saveAllCachedResources()
        {
            editorController.saveAllCachedResources();
        }

        public void createNewProject(string fullProjectName, bool deleteOld)
        {
            editorController.createNewProject(fullProjectName, deleteOld, template);
        }

        public void openProject(string projectPath, string fullFilePath)
        {
            editorController.openProject(projectPath, fullFilePath);
        }

        public void closeProject()
        {
            editorController.closeProject();
        }

        public void addSlide()
        {
            SlideEntry slide = AddSlide(currentPresentation, editorController.ResourceProvider);
            if (SlideAdded != null)
            {
                SlideAdded.Invoke(slide);
            }
        }

        public void removeSelectedEntry()
        {
            if (selectedEntry != null)
            {
                int index = currentPresentation.indexOf(selectedEntry);
                currentPresentation.removeEntry(selectedEntry);
                if (EntryRemoved != null)
                {
                    EntryRemoved.Invoke(selectedEntry);
                }
                if (index >= currentPresentation.Count)
                {
                    index = currentPresentation.Count - 1;
                }
                if (index == -1) //All slides removed
                {
                    addSlide();
                    index = 0;
                }
                SelectedEntry = currentPresentation[index];
            }
        }

        public static SlideEntry AddSlide(PresentationIndex presentationIndex, EditorResourceProvider resourceProvider, String defaultRml = DefaultRml)
        {
            SlideEntry slide = new SlideEntry();
            presentationIndex.addEntry(slide);
            if (!resourceProvider.exists(slide.UniqueName))
            {
                resourceProvider.createDirectory("", slide.UniqueName);
            }
            using (StreamWriter streamWriter = new StreamWriter(resourceProvider.openWriteStream(slide.File)))
            {
                streamWriter.Write(defaultRml);
            }
            return slide;            
        }

        public void preview()
        {
            AnomalousMvcContext presentationContext = currentPresentation.buildMvcContext();
            standaloneController.TimelineController.setResourceProvider(editorController.ResourceProvider);
            presentationContext.setResourceProvider(editorController.ResourceProvider);
            presentationContext.RuntimeName = "Editor.PreviewMvcContext";
            standaloneController.MvcCore.startRunningContext(presentationContext);
        }

        private const String DefaultRml = @"<rml>
	<head>
		<link type=""text/template"" href=""/MasterTemplate.trml"" />
	</head>
	<body template=""MasterTemplate"">
        <h1>Click here to add title</h1>
        <p style=""white-space: pre-wrap;"">Click here to add text</p>
    </body>
</rml>
";
    }
}
