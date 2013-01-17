using Engine.Editing;
using Logging;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    class SlideItemTemplate : ProjectItemTemplate
    {
        private SlideTypeController slideTypeController;
        private MvcTypeController mvcTypeController;
        private SceneViewController sceneViewController;
        private MedicalStateController medicalStateController;

        public SlideItemTemplate(SlideTypeController slideTypeController, MvcTypeController mvcTypeController, SceneViewController sceneViewController, MedicalStateController medicalStateController)
        {
            this.slideTypeController = slideTypeController;
            this.mvcTypeController = mvcTypeController;
            this.sceneViewController = sceneViewController;
            this.medicalStateController = medicalStateController;
        }

        public void createItem(string path, EditorController editorController)
        {
            String slideName = Path.Combine(path, Path.ChangeExtension(Name, slideTypeController.Extension));
            slideTypeController.createSlideFileSafely(slideName, (fileName) =>
                {
                    MedicalRmlSlide slide = new MedicalRmlSlide();
                    CameraPosition cameraPos = new CameraPosition();
                    if (sceneViewController.ActiveWindow != null)
                    {
                        SceneViewWindow window = sceneViewController.ActiveWindow;
                        cameraPos.Translation = window.Translation;
                        cameraPos.LookAt = window.LookAt;
                        cameraPos.calculateIncludePoint(window);
                    }
                    slide.CameraPosition = cameraPos;
                    slide.Layers = new LayerState("");
                    slide.Layers.captureState();
                    slide.MedicalState = medicalStateController.createPresetState("");
                    slide.MusclePosition = new MusclePosition();
                    slide.MusclePosition.captureState();
                    return slide;
                });
            editorController.saveAllCachedResources();
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

        [Editable]
        public String Name { get; set; }

        public bool isValid(out string errorMessage)
        {
            if (String.IsNullOrEmpty(Name))
            {
                errorMessage = "Please enter a name.";
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }

        public void reset()
        {
            Name = null;
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
