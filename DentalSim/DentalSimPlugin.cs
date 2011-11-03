using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using Medical.GUI;
using MyGUIPlugin;
using DentalSim.GUI;
using Medical.Controller;

namespace DentalSim
{
    class DentalSimPlugin : AtlasPlugin
    {
        //Dialogs
        private MandibleMovementDialog mandibleMovementDialog;

        private List<String> movementSequenceDirectories = new List<string>();
        private StandaloneController standaloneController;

        public DentalSimPlugin()
        {
            //This is temporary cruft from the old system.
            movementSequenceDirectories.Add("/Graphics");
            movementSequenceDirectories.Add("/MRI");
            movementSequenceDirectories.Add("/RadiographyCT");
            movementSequenceDirectories.Add("/Clinical");
            movementSequenceDirectories.Add("/DentitionProfile");
            movementSequenceDirectories.Add("/Doppler");
        }

        public void Dispose()
        {
            mandibleMovementDialog.Dispose();
        }

        public void createMenuBar(NativeMenuBar menu)
        {

        }

        public void initialize(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;

            GUIManager guiManager = standaloneController.GUIManager;
            Gui.Instance.load("DentalSim.Resources.Imagesets.xml");

            //Dialogs
            mandibleMovementDialog = new MandibleMovementDialog(standaloneController.MovementSequenceController);
            guiManager.addManagedDialog(mandibleMovementDialog);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new ShowToothContactsTask(0));

            MDIDialogOpenTask mandibleMovementTask = new MDIDialogOpenTask(mandibleMovementDialog, "Medical.ManualMovement", "Manual Movement", "DentalSimIcons/ManualMovement", "Dental Simulation", 2);
            mandibleMovementTask.ShowOnTimelineTaskbar = true;
            taskController.addTask(mandibleMovementTask);

            taskController.addTask(new StartEmbeddedTimelineTask("DentalSim.Eminence", "Eminence", "DistortionPanelIcons/LeftFossa", "Dental Simulation", GetType(), "DentalSim.Timeline.", "Disclaimer_Eminence.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("DentalSim.Dentition", "Dentition", "DentalSimIcons/Dentition", "Dental Simulation", GetType(), "DentalSim.Timeline.", "Disclaimer_Dentition.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("DentalSim.DiscClockFace", "Disc Clock Face", "DentalSimIcons/DiscClockFace", "Dental Simulation", GetType(), "DentalSim.Timeline.", "Disclaimer_DiscClockFace.tl", standaloneController.TimelineController));
            taskController.addTask(new StartEmbeddedTimelineTask("DentalSim.Mandible", "Mandible", "DentalSimIcons/Mandible", "Dental Simulation", GetType(), "DentalSim.Timeline.", "Disclaimer_Mandible.tl", standaloneController.TimelineController));
        }

        public void sceneLoaded(SimScene scene)
        {
            mandibleMovementDialog.sceneLoaded(scene);

            //Load sequences
            MedicalController medicalController = standaloneController.MedicalController;
            SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
            SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
            StandaloneApp app = standaloneController.App;

            String sequenceDirectory = medicalController.CurrentSceneDirectory + "/" + medicalScene.SequenceDirectory;
            standaloneController.MovementSequenceController.loadSequenceDirectories(sequenceDirectory, movementSequenceDirectories);
        }

        public void sceneRevealed()
        {

        }

        public void sceneUnloading(SimScene scene)
        {
            mandibleMovementDialog.sceneUnloading(scene);
        }

        public void setMainInterfaceEnabled(bool enabled)
        {

        }

        public string BrandingImageKey
        {
            get
            {
                return "DentalSim/BrandingImage";
            }
        }

        public long PluginId
        {
            get
            {
                return 2;
            }
        }

        public string PluginName
        {
            get
            {
                return "Dental Simulation";
            }
        }

        public String Location
        {
            get
            {
                return GetType().Assembly.Location;
            }
        }

        public Version Version
        {
            get
            {
                return GetType().Assembly.GetName().Version;
            }
        }
    }
}
