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
using Engine;
using Anomalous.GuiFramework;

namespace DentalSim
{
    public class DentalSimPlugin : AtlasPlugin
    {
        //Dialogs
        private MandibleMovementDialog mandibleMovementDialog;
        private NotesDialog notesDialog;
        private StateListDialog stateList;
        private SavePatientDialog savePatientDialog;
        private OpenPatientDialog openPatientDialog;

        private StandaloneController standaloneController;

        public DentalSimPlugin()
        {
            this.AllowUninstall = true;
        }

        public void Dispose()
        {
            mandibleMovementDialog.Dispose();
            stateList.Dispose();
            notesDialog.Dispose();
            savePatientDialog.Dispose();
            openPatientDialog.Dispose();
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("DentalSim.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            standaloneController.DocumentController.addDocumentHandler(new PatientDocumentHandler(standaloneController));

            this.standaloneController = standaloneController;
            standaloneController.PatientDataController.PatientDataChanged += new Action<PatientDataFile>(PatientDataController_PatientDataChanged);

            GUIManager guiManager = standaloneController.GUIManager;

            //Dialogs
            mandibleMovementDialog = new MandibleMovementDialog(standaloneController.MovementSequenceController, standaloneController.MusclePositionController);
            guiManager.addManagedDialog(mandibleMovementDialog);

            notesDialog = new NotesDialog(standaloneController.MedicalStateController);
            guiManager.addManagedDialog(notesDialog);

            stateList = new StateListDialog(standaloneController.MedicalStateController);
            guiManager.addManagedDialog(stateList);

            savePatientDialog = new SavePatientDialog(guiManager);
            savePatientDialog.SaveFile += new EventHandler(savePatientDialog_SaveFile);

            openPatientDialog = new OpenPatientDialog(guiManager);
            openPatientDialog.OpenFile += new EventHandler(openPatientDialog_OpenFile);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;
            AnatomyTaskManager anatomyTasks = standaloneController.AnatomyTaskManager;

            taskController.addTask(new ShowToothContactsTask(0));

            PinableMDIDialogOpenTask mandibleMovementTask = new PinableMDIDialogOpenTask(mandibleMovementDialog, "Medical.ManualMovement", "Manual Movement", "DentalSimIcons/ManualMovement", "Dental Simulation", 2);
            taskController.addTask(mandibleMovementTask);

            anatomyTasks.addTask(new StartEmbeddedMvcTask("DentalSim.Eminence", "Eminence", "DentalSimIcons/Eminence", "Dental Simulation", GetType(), "DentalSim.Wizards.", "Eminence.mvc", standaloneController.TimelineController, standaloneController.MvcCore), new String[]{ "Outer Skull", "Inner Skull"});
            anatomyTasks.addTask(new StartEmbeddedMvcTask("DentalSim.Dentition", "Dentition", "DentalSimIcons/Dentition", "Dental Simulation", GetType(), "DentalSim.Wizards.", "Dentition.mvc", standaloneController.TimelineController, standaloneController.MvcCore), TeethNames);
            anatomyTasks.addTask(new StartEmbeddedMvcTask("DentalSim.DiscClockFace", "Disc Clock Face", "DentalSimIcons/DiscClockFace", "Dental Simulation", GetType(), "DentalSim.Wizards.DiscClock.", "DiscClockFace.mvc", standaloneController.TimelineController, standaloneController.MvcCore, true), new String[] { "Left TMJ Disc", "Right TMJ Disc" });
            anatomyTasks.addTask(new StartEmbeddedMvcTask("DentalSim.Mandible", "Mandible", "DentalSimIcons/Mandible", "Dental Simulation", GetType(), "DentalSim.Wizards.", "Mandible.mvc", standaloneController.TimelineController, standaloneController.MvcCore), new String[] { "Mandible" });
            anatomyTasks.addTask(new StartEmbeddedMvcTask("DentalSim.ClinicalDoppler", "Clinical Doppler", "DentalSimIcons/ClinicalDoppler", "Dental Simulation", GetType(), "DentalSim.Wizards.ClinicalDoppler.", "ClinicalDoppler.mvc", standaloneController.TimelineController, standaloneController.MvcCore), new String[] { "Left TMJ Disc", "Right TMJ Disc", "Mandible" });
            anatomyTasks.addTask(new StartEmbeddedMvcTask("DentalSim.ClinicalCT", "Clinical CT", "DentalSimIcons/ClinicalCT", "Dental Simulation", GetType(), "DentalSim.Wizards.ClinicalCT.", "ClinicalCT.mvc", standaloneController.TimelineController, standaloneController.MvcCore), new String[] { "Left TMJ Disc", "Right TMJ Disc", "Mandible" });
            anatomyTasks.addTask(new StartEmbeddedMvcTask("DentalSim.ClinicalMRI", "Clinical MRI", "DentalSimIcons/ClinicalMRI", "Dental Simulation", GetType(), "DentalSim.Wizards.ClinicalMRI.", "ClinicalMRI.mvc", standaloneController.TimelineController, standaloneController.MvcCore), new String[] { "Left TMJ Disc", "Right TMJ Disc", "Mandible" });
            anatomyTasks.addTask(new StartEmbeddedMvcTask("DentalSim.ClinicalOrthoAndSkeletal", "Clinical Orthodontic and Skeletal", "DentalSimIcons/ClinicalOrthodonticAndSkeletal", "Dental Simulation", GetType(), "DentalSim.Wizards.", "ClinicalOrthoAndSkeletal.mvc", standaloneController.TimelineController, standaloneController.MvcCore), new String[] { "Left TMJ Disc", "Right TMJ Disc", "Mandible" });

            taskController.addTask(new ShowPopupTask(openPatientDialog, "Medical.OpenPatient", "Open", "DentalSimIcons/Open", TaskMenuCategories.Patient, 1));

            PinableMDIDialogOpenTask statesTask = new PinableMDIDialogOpenTask(stateList, "Medical.StateList", "States", "DentalSimIcons/StatesIcon", TaskMenuCategories.Patient);
            taskController.addTask(statesTask);

            PinableMDIDialogOpenTask notesTask = new PinableMDIDialogOpenTask(notesDialog, "Medical.Notes", "Notes", "DentalSimIcons/NotesIcon", TaskMenuCategories.Patient);
            taskController.addTask(notesTask);

            CallbackTask saveTaskItem = new CallbackTask("Medical.SavePatient", "Save", "CommonToolstrip/Save", TaskMenuCategories.Patient, 2, false);
            saveTaskItem.OnClicked += new CallbackTask.ClickedCallback(saveTaskItem_OnClicked);
            taskController.addTask(saveTaskItem);

            CallbackTask saveAsTaskItem = new CallbackTask("Medical.SavePatientAs", "Save As", "CommonToolstrip/SaveAs", TaskMenuCategories.Patient, 3, false);
            saveAsTaskItem.OnClicked += new CallbackTask.ClickedCallback(saveAsTaskItem_OnClicked);
            taskController.addTask(saveAsTaskItem);

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

        private IEnumerable<String> TeethNames
        {
            get
            {
                for(int i = 1; i < 33; ++i)
                {
                    yield return "Tooth " + i;
                }
            }
        }

        public void unload(StandaloneController standaloneController, bool willReload, bool shuttingDown)
        {

        }

        public void sceneLoaded(SimScene scene)
        {
            mandibleMovementDialog.sceneLoaded(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            mandibleMovementDialog.sceneUnloading(scene);
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

        public bool AllowUninstall { get; set; }

        public bool AllowRuntimeUninstall
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<long> DependencyPluginIds
        {
            get
            {
                return IEnumerableUtil<long>.EmptyIterator;
            }
        }

        public void save()
        {
            if (standaloneController.MedicalStateController.getNumStates() == 0)
            {
                MessageBox.show("No information to save. Please create some states or perform an exam.", "Nothing to save.", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
            }
            else
            {
                savePatientDialog.save();
            }
        }

        public void saveAs()
        {
            if (standaloneController.MedicalStateController.getNumStates() == 0)
            {
                MessageBox.show("No information to save. Please create some states using the wizards first.", "Nothing to save.", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
            }
            else
            {
                savePatientDialog.saveAs();
            }
        }

        public void PatientDataController_PatientDataChanged(PatientDataFile patientData)
        {
            if (patientData != null)
            {
                MainWindow.Instance.updateWindowTitle(String.Format("{0} {1}", patientData.FirstName, patientData.LastName));
                standaloneController.DocumentController.addToRecentDocuments(patientData.BackingFile);
            }
            else
            {
                MainWindow.Instance.clearWindowTitle();
            }
            savePatientDialog.PatientData = patientData;
        }

        private void savePatientDialog_SaveFile(object sender, EventArgs e)
        {
            PatientDataFile patientData = savePatientDialog.PatientData;
            standaloneController.PatientDataController.saveMedicalState(patientData);
        }

        private void openPatientDialog_OpenFile(object sender, EventArgs e)
        {
            PatientDataFile patientData = openPatientDialog.CurrentFile;
            standaloneController.PatientDataController.openPatientFile(patientData);
        }

        void saveAsTaskItem_OnClicked(Task item)
        {
            saveAs();
        }

        void saveTaskItem_OnClicked(Task item)
        {
            save();
        }
    }
}
