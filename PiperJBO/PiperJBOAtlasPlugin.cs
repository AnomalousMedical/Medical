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

        public PiperJBOAtlasPlugin(LicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
        }

        public void Dispose()
        {
            
        }

        public void initialize(StandaloneController standaloneController)
        {
            

            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;

            standaloneController.SceneViewController.ActiveWindowChanged += new SceneViewWindowEvent(SceneViewController_ActiveWindowChanged);
            SceneViewController_ActiveWindowChanged(standaloneController.SceneViewController.ActiveWindow);

            //Timeline GUIs
            

            //Timeline Wizard Tasks
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.Cephalometric", "Cephalometric", "DistortionsToolstrip/Cephalometric", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_Cephalometric.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.CephalometricAndDentition", "Cephalometric and Dentition", "DistortionsToolstrip/CephalometricAndDentition", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_CephalometricAndDentition.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.ClinicalAndDoppler", "Clinical and Doppler", "DistortionsToolstrip/ClinicalAndDoppler", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_ClinicalAndDoppler.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.ClinicalAndMRI", "Clinical and MRI", "DistortionsToolstrip/ClinicalAndMRI", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_ClinicalAndMRI.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.ClinicalAndRadiography", "Clinical and Radiography", "DistortionsToolstrip/ClinicalAndRadiography", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_ClinicalAndRadiography.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.Doppler", "Doppler", "DistortionsToolstrip/Doppler", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_Doppler.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.DiscSpace", "Disc Space", "DistortionsToolstrip/DiscSpace", "Piper's Joint Based Occlusion", this.GetType(), "Medical.Timelines.", "Disclaimer_DiscSpace.tl", standaloneController.TimelineController, 502));

            //taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.Dentition", "Dentition", "DistortionsToolstrip/Dentition", "Distortion", this.GetType(), "Medical.Timelines.", "Disclaimer_Dentition.tl", standaloneController.TimelineController, 500));
            //taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.DiscClockFace", "Disc Clock Face", "DistortionsToolstrip/DiscClockFace", "Distortion", this.GetType(), "Medical.Timelines.", "Disclaimer_DiscClockFace.tl", standaloneController.TimelineController, 501));
            //taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.Eminence", "Eminence", "DistortionPanelIcons/LeftFossa", "Distortion", this.GetType(), "Medical.Timelines.", "Disclaimer_Eminence.tl", standaloneController.TimelineController, 503));
            //taskController.addTask(new StartEmbeddedTimelineTask("PiperJBO.Mandible", "Mandible", "DistortionsToolstrip/Mandible", "Distortion", this.GetType(), "Medical.Timelines.", "Disclaimer_Mandible.tl", standaloneController.TimelineController, 504));
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
