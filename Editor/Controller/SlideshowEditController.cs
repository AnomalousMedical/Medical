using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SlideshowEditController
    {
        public event Action<Slideshow> SlideshowLoaded;
        public event Action SlideshowClosed;
        public event Action<Slide> SlideAdded;
        public event Action<Slide> SlideRemoved;

        private UndoRedoBuffer undoBuffer = new UndoRedoBuffer(50);

        //Editor Contexts
        private SlideEditorContext slideEditorContext;

        private StandaloneController standaloneController;
        private PropEditController propEditController;
        private EditorController editorController;
        private ShowTypeController showTypeController;
        private EditorUICallback uiCallback;
        private Slideshow slideshow;

        public SlideshowEditController(StandaloneController standaloneController, EditorUICallback uiCallback, PropEditController propEditController, EditorController editorController)
        {
            this.standaloneController = standaloneController;
            this.uiCallback = uiCallback;
            this.propEditController = propEditController;
            this.editorController = editorController;
            editorController.ProjectChanged += editorController_ProjectChanged;

            //Show Type Controller
            showTypeController = new ShowTypeController(editorController);
            editorController.addTypeController(showTypeController);

            MedicalSlideItemTemplate medicalSlideTemplate = new MedicalSlideItemTemplate(standaloneController.SceneViewController, standaloneController.MedicalStateController);
            medicalSlideTemplate.SlideCreated += (slide) =>
                {
                    if (slideshow != null)
                    {
                        slideshow.addSlide(slide);
                        if (SlideAdded != null)
                        {
                            SlideAdded.Invoke(slide);
                        }
                    }
                };
            editorController.addItemTemplate(medicalSlideTemplate);
        }

        public void editSlide(Slide slide)
        {
            if (slide is MedicalRmlSlide)
            {
                MedicalRmlSlide medicalSlide = (MedicalRmlSlide)slide;
                slideEditorContext = new SlideEditorContext(medicalSlide, uiCallback, undoBuffer, (rml) =>
                {
                    slideEditorContext.setWysiwygRml(rml, true);
                });
                slideEditorContext.Focus += (obj) =>
                {
                    slideEditorContext = obj;
                };
                slideEditorContext.Blur += obj =>
                {
                    medicalSlide.Rml = obj.CurrentText;
                    if (slideEditorContext == obj)
                    {
                        slideEditorContext = null;
                    }
                };
                editorController.runEditorContext(slideEditorContext.MvcContext);
            }
        }

        public void removeSlide(Slide slide)
        {
            slideshow.removeSlide(slide);
            if (SlideRemoved != null)
            {
                SlideRemoved.Invoke(slide);
            }
        }

        public UndoRedoBuffer UndoBuffer
        {
            get
            {
                return undoBuffer;
            }
        }

        void editorController_ProjectChanged(EditorController editorController, string fullFilePath)
        {
            if (slideEditorContext != null)
            {
                slideEditorContext.close();
            }

            if (editorController.ResourceProvider != null)
            {
                //Try to open a default mvc context
                String file = "Slides.show";
                if (editorController.ResourceProvider.exists(file))
                {
                    loadSlideshow(file);
                }
                else
                {
                    IEnumerable<String> files = editorController.ResourceProvider.listFiles("*.show", "", true);
                    String firstFile = files.FirstOrDefault();
                    if (firstFile != null)
                    {
                        loadSlideshow(firstFile);
                    }
                }
            }
            else if (SlideshowClosed != null)
            {
                SlideshowClosed.Invoke();
            }
        }

        void loadSlideshow(String file)
        {
            standaloneController.DocumentController.addToRecentDocuments(editorController.ResourceProvider.getFullFilePath(file));
            slideshow = editorController.loadFile<Slideshow>(file);
            if (SlideshowLoaded != null)
            {
                SlideshowLoaded.Invoke(slideshow);
            }
        }
    }
}
