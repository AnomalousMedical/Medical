using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SlideshowEditController
    {
        public event Action<Slideshow> SlideshowLoaded;

        //Editor Contexts
        private ShowEditorContext showEditorContext;

        private StandaloneController standaloneController;
        private PropEditController propEditController;
        private EditorController editorController;
        private ShowTypeController showTypeController;
        private EditorUICallback uiCallback;

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
        }

        public void editSlide(MedicalRmlSlide slide)
        {
            showEditorContext = new ShowEditorContext(slide, uiCallback);
            showEditorContext.Focus += (obj) =>
            {
                showEditorContext = obj;
            };
            showEditorContext.Blur += obj =>
            {
                //rmlTypeController.updateCachedText(obj.CurrentFile, obj.CurrentText);
                if (showEditorContext == obj)
                {
                    showEditorContext = null;
                }
            };
            editorController.runEditorContext(showEditorContext.MvcContext);
        }

        void editorController_ProjectChanged(EditorController editorController, string fullFilePath)
        {
            if (showEditorContext != null)
            {
                showEditorContext.close();
            }

            if (editorController.ResourceProvider != null)
            {
                //standaloneController.DocumentController.addToRecentDocuments(editorController.ResourceProvider.BackingLocation);
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
        }

        void loadSlideshow(String file)
        {
            Slideshow slideshow = editorController.loadFile<Slideshow>(file);
            if (SlideshowLoaded != null)
            {
                SlideshowLoaded.Invoke(slideshow);
            }
        }
    }
}
