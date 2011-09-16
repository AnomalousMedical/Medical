using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using OgreWrapper;
using System.Diagnostics;

namespace Medical.GUI
{
    class PiperJBOAtlasPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private LicenseManager licenseManager;

        //Timeline Wizard
        private TimelineWizard timelineWizard;

        public PiperJBOAtlasPlugin(LicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
        }

        public void Dispose()
        {
            timelineWizard.Dispose();
        }

        public void initialize(StandaloneController standaloneController)
        {
            Gui.Instance.load("Medical.Resources.PiperJBOImagesets.xml");

            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;

            standaloneController.SceneViewController.ActiveWindowChanged += new SceneViewWindowEvent(SceneViewController_ActiveWindowChanged);
            SceneViewController_ActiveWindowChanged(standaloneController.SceneViewController.ActiveWindow);

            //Timeline GUIs
            timelineWizard = new TimelineWizard(standaloneController);
            standaloneController.TimelineGUIFactory.addPrototype(new RemoveTopTeethGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RemoveBottomTeethGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new DisclaimerGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightDiscSpaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftDiscSpaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftDopplerGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightDopplerGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new FossaGUILeftPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new FossaGUIRightPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftCondylarDegenerationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightCondylarDegenerationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftCondylarGrowthGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightCondylarGrowthGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftDiscClockFaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightDiscClockFaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new ProfileDistortionGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new TeethAdaptationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new TeethHeightAdaptationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new NotesGUIPrototype(timelineWizard));

            //Timeline Wizard Tasks
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.Cephalometric", "Cephalometric", "DistortionsToolstrip/Cephalometric", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_Cephalometric.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.CephalometricAndDentition", "Cephalometric and Dentition", "DistortionsToolstrip/CephalometricAndDentition", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_CephalometricAndDentition.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.ClinicalAndDoppler", "Clinical and Doppler", "DistortionsToolstrip/ClinicalAndDoppler", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_ClinicalAndDoppler.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.ClinicalAndMRI", "Clinical and MRI", "DistortionsToolstrip/ClinicalAndMRI", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_ClinicalAndMRI.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.ClinicalAndRadiography", "Clinical and Radiography", "DistortionsToolstrip/ClinicalAndRadiography", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_ClinicalAndRadiography.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.Dentition", "Dentition", "DistortionsToolstrip/Dentition", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_Dentition.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.DiscClockFace", "Disc Clock Face", "DistortionsToolstrip/DiscClockFace", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_DiscClockFace.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.DiscSpace", "Disc Space", "DistortionsToolstrip/DiscSpace", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_DiscSpace.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.Doppler", "Doppler", "DistortionsToolstrip/Doppler", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_Doppler.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.Eminence", "Eminence", "DistortionPanelIcons/LeftFossa", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_Eminence.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.Mandible", "Mandible", "DistortionsToolstrip/Mandible", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_Mandible.tl", standaloneController.TimelineController));
        }

        public void finishInitialization()
        {
            
        }

        public void sceneRevealed()
        {

        }

        public void sceneLoaded(SimScene scene)
        {

        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            
        }

        void SceneViewController_ActiveWindowChanged(SceneViewWindow window)
        {
            
        }
    }
}
