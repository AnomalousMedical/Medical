using Engine.Editing;
using Logging;
using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
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
        public event Action<MedicalRmlSlide> SlideCreated;

        private SceneViewController sceneViewController;
        private MedicalStateController medicalStateController;

        public MedicalSlideItemTemplate(SceneViewController sceneViewController, MedicalStateController medicalStateController)
        {
            this.sceneViewController = sceneViewController;
            this.medicalStateController = medicalStateController;
        }

        public void createItem(string path, EditorController editorController)
        {
            MedicalRmlSlide slide = new MedicalRmlSlide();
            applySceneStateToSlide(slide);
            slide.addPanel(defaultSlide, ViewLocations.Left, ViewSizeStrategy.Auto, 30);
            if (SlideCreated != null)
            {
                SlideCreated.Invoke(slide);
            }
        }

        public void applySceneStateToSlide(MedicalRmlSlide slide)
        {
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

        private const String defaultSlide = @"<rml>
	<head>
		<link type=""text/template"" href=""/MasterTemplate.trml"" />
	</head>
	<body template=""MasterTemplate"">
        <h1>Click to Change Title</h1>
        <p>Click to change text.</p>
    </body>
</rml>
";
    }
}
