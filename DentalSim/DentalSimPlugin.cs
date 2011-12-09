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
using System.Reflection;

namespace DentalSim
{
    class DentalSimPlugin : AtlasPlugin
    {
        //Dialogs
        private MandibleMovementDialog mandibleMovementDialog;
        private StandaloneController standaloneController;

        public DentalSimPlugin()
        {

        }

        public void Dispose()
        {
            mandibleMovementDialog.Dispose();
        }

        public void createMenuBar(NativeMenuBar menu)
        {

        }

        public void loadGUIResources()
        {
            Gui.Instance.load("DentalSim.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;

            GUIManager guiManager = standaloneController.GUIManager;

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
