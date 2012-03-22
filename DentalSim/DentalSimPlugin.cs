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
            ResourceManager.Instance.load("DentalSim.Resources.Imagesets.xml");
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

            //Movement Sequences
            MovementSequenceController movementSequenceController = standaloneController.MovementSequenceController;
            Assembly assembly = GetType().Assembly;

            //Border Movements
            movementSequenceController.addMovementSequence("Border Movements", new EmbeddedMovementSequenceInfo(assembly, "Posselt Sagittal", "DentalSim.Sequences.BorderMovements.Posselt Sagittal.seq"));

            //Excursion
            movementSequenceController.addMovementSequence("Excursion", new EmbeddedMovementSequenceInfo(assembly, "Left Tooth Contact Bruxism", "DentalSim.Sequences.Excursion.Left Tooth Contact Bruxism.seq"));
            movementSequenceController.addMovementSequence("Excursion", new EmbeddedMovementSequenceInfo(assembly, "Left Tooth Contact Maximal", "DentalSim.Sequences.Excursion.Left Tooth Contact Maximal.seq"));
            movementSequenceController.addMovementSequence("Excursion", new EmbeddedMovementSequenceInfo(assembly, "Left Tooth Contact", "DentalSim.Sequences.Excursion.Left Tooth Contact.seq"));
            movementSequenceController.addMovementSequence("Excursion", new EmbeddedMovementSequenceInfo(assembly, "Open Bilateral", "DentalSim.Sequences.Excursion.Open Bilateral.seq"));
            movementSequenceController.addMovementSequence("Excursion", new EmbeddedMovementSequenceInfo(assembly, "Open Left", "DentalSim.Sequences.Excursion.Open Left.seq"));
            movementSequenceController.addMovementSequence("Excursion", new EmbeddedMovementSequenceInfo(assembly, "Open Right", "DentalSim.Sequences.Excursion.Open Right.seq"));
            movementSequenceController.addMovementSequence("Excursion", new EmbeddedMovementSequenceInfo(assembly, "Right Tooth Contact Bruxism", "DentalSim.Sequences.Excursion.Right Tooth Contact Bruxism.seq"));
            movementSequenceController.addMovementSequence("Excursion", new EmbeddedMovementSequenceInfo(assembly, "Right Tooth Contact Maximal", "DentalSim.Sequences.Excursion.Right Tooth Contact Maximal.seq"));
            movementSequenceController.addMovementSequence("Excursion", new EmbeddedMovementSequenceInfo(assembly, "Right Tooth Contact", "DentalSim.Sequences.Excursion.Right Tooth Contact.seq"));

            //Normal Chewing
            movementSequenceController.addMovementSequence("Normal Chewing", new EmbeddedMovementSequenceInfo(assembly, "Chewing Left Side", "DentalSim.Sequences.NormalChewing.Chewing Left Side.seq"));
            movementSequenceController.addMovementSequence("Normal Chewing", new EmbeddedMovementSequenceInfo(assembly, "Chewing Right Side", "DentalSim.Sequences.NormalChewing.Chewing Right Side.seq"));

            //Protrusion
            movementSequenceController.addMovementSequence("Protrusion", new EmbeddedMovementSequenceInfo(assembly, "Open Protrusion", "DentalSim.Sequences.Protrusion.Open Protrusion.seq"));
            movementSequenceController.addMovementSequence("Protrusion", new EmbeddedMovementSequenceInfo(assembly, "Protrusion Maximal", "DentalSim.Sequences.Protrusion.Protrusion Maximal.seq"));
            movementSequenceController.addMovementSequence("Protrusion", new EmbeddedMovementSequenceInfo(assembly, "Protrusion Tooth Contact Edge to Edge", "DentalSim.Sequences.Protrusion.Protrusion Tooth Contact Edge to Edge.seq"));

            //Vertical Opening
            movementSequenceController.addMovementSequence("Vertical Opening", new EmbeddedMovementSequenceInfo(assembly, "Hinge Opening", "DentalSim.Sequences.VerticalOpening.Hinge Opening.seq"));
            movementSequenceController.addMovementSequence("Vertical Opening", new EmbeddedMovementSequenceInfo(assembly, "Maximal Opening", "DentalSim.Sequences.VerticalOpening.Maximal Opening.seq"));
            movementSequenceController.addMovementSequence("Vertical Opening", new EmbeddedMovementSequenceInfo(assembly, "Tapping Teeth", "DentalSim.Sequences.VerticalOpening.Tapping Teeth.seq"));
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
