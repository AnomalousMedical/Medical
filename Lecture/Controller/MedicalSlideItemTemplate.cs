using Engine.Editing;
using Logging;
using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using Medical.SlideshowActions;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lecture
{
    class MedicalSlideItemTemplate : ProjectItemTemplate
    {
        public event Action<Slide> SlideCreated;

        private SceneViewController sceneViewController;
        private MedicalStateController medicalStateController;

        public MedicalSlideItemTemplate(SceneViewController sceneViewController, MedicalStateController medicalStateController)
        {
            this.sceneViewController = sceneViewController;
            this.medicalStateController = medicalStateController;
        }

        public void createItem(string path, EditorController editorController)
        {
            Slide slide = new Slide();
            applySceneStateToSlide(slide);
            RmlSlidePanel panel = new RmlSlidePanel()
            {
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
                Size = 480,
            };
            slide.addPanel(panel);
            if (!editorController.ResourceProvider.directoryExists(slide.UniqueName))
            {
                editorController.ResourceProvider.createDirectory("", slide.UniqueName);
            }
            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.SimpleSlide_rml, panel.getRmlFilePath(slide), editorController.ResourceProvider, EmbeddedTemplateNames.Assembly);
            using (editorController.ResourceProvider.openWriteStream(Path.Combine(slide.UniqueName, Slide.StyleSheetName))) { }
            if (SlideCreated != null)
            {
                SlideCreated.Invoke(slide);
            }
        }

        public void applySceneStateToSlide(Slide slide)
        {
            CameraPosition cameraPos = new CameraPosition();
            if (sceneViewController.ActiveWindow != null)
            {
                SceneViewWindow window = sceneViewController.ActiveWindow;
                cameraPos.Translation = window.Translation;
                cameraPos.LookAt = window.LookAt;
                window.calculateIncludePoint(cameraPos);
            }
            LayerState layers = new LayerState("");
            layers.captureState();
            PresetState medicalState = medicalStateController.createPresetState("");
            MusclePosition musclePosition = new MusclePosition();
            musclePosition.captureState();
            slide.StartupAction = new SetupSceneAction("Show", cameraPos, layers, musclePosition, medicalState, true, TeethController.HighlightContacts);
        }

        public string TypeName
        {
            get
            {
                return "Slide";
            }
        }

        public string ImageName
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        public string Group
        {
            get
            {
                return "Slideshow";
            }
        }

        public bool isValid(out string errorMessage)
        {
            errorMessage = null;
            return true;
        }

        public void reset()
        {
            
        }

        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, TypeName, null);
                }
                return editInterface;
            }
        }

        private static void createView(AnomalousMvcContext mvcContext, String name)
        {
            RmlView view = new RmlView(name);
            mvcContext.Views.add(view);
        }
    }
}
