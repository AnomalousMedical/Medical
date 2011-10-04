using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using OgreWrapper;
using System.Diagnostics;
using Medical.GUI;

namespace Medical
{
    public class PremiumBodyAtlasPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private LicenseManager licenseManager;
        private List<String> movementSequenceDirectories = new List<string>();

        //Dialogs
        private MandibleMovementDialog mandibleMovementDialog;
        private NotesDialog notesDialog;
        private SequencePlayer sequencePlayer;
        private BookmarksGUI bookmarks;
        private StateListDialog stateList;
        private SavePatientDialog savePatientDialog;
        private OpenPatientDialog openPatientDialog;

        //Tasks
        private ChangeWindowLayoutTask windowLayout;
        
        //Controllers
        private BookmarksController bookmarksController;

        public PremiumBodyAtlasPlugin(StandaloneController standaloneController)
        {
            this.licenseManager = standaloneController.App.LicenseManager;
            bookmarksController = new BookmarksController(standaloneController);

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
            stateList.Dispose();
            sequencePlayer.Dispose();
            windowLayout.Dispose();
            mandibleMovementDialog.Dispose();
            notesDialog.Dispose();
            bookmarks.Dispose();
            bookmarksController.Dispose();
            savePatientDialog.Dispose();
            openPatientDialog.Dispose();
        }

        public void initialize(StandaloneController standaloneController)
        {
            standaloneController.DocumentController.addDocumentHandler(new PatientDocumentHandler(standaloneController, this));

            Gui.Instance.load("Medical.Resources.PremiumImagesets.xml");

            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;

            //Dialogs
            //BodyAtlas
            mandibleMovementDialog = new MandibleMovementDialog(standaloneController.MovementSequenceController);
            guiManager.addManagedDialog(mandibleMovementDialog);

            notesDialog = new NotesDialog(standaloneController.MedicalStateController);
            guiManager.addManagedDialog(notesDialog);

            sequencePlayer = new SequencePlayer(standaloneController.MovementSequenceController);
            guiManager.addManagedDialog(sequencePlayer);

            bookmarks = new BookmarksGUI(bookmarksController, standaloneController.GUIManager);

            stateList = new StateListDialog(standaloneController.MedicalStateController);
            guiManager.addManagedDialog(stateList);

            savePatientDialog = new SavePatientDialog(guiManager);
            savePatientDialog.SaveFile += new EventHandler(savePatientDialog_SaveFile);

            openPatientDialog = new OpenPatientDialog(guiManager);
            openPatientDialog.OpenFile += new EventHandler(openPatientDialog_OpenFile);

            //Tasks
            windowLayout = new ChangeWindowLayoutTask(standaloneController);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new ShowPopupTask(openPatientDialog, "Medical.OpenPatient", "Open", "FileToolstrip/Open", TaskMenuCategories.Patient, 1));

            CallbackTask saveTaskItem = new CallbackTask("Medical.SavePatient", "Save", "FileToolstrip/Save", TaskMenuCategories.Patient, 2, false);
            saveTaskItem.OnClicked += new CallbackTask.ClickedCallback(saveTaskItem_OnClicked);
            taskController.addTask(saveTaskItem);

            CallbackTask saveAsTaskItem = new CallbackTask("Medical.SavePatientAs", "Save As", "FileToolstrip/SaveAs", TaskMenuCategories.Patient, 3, false);
            saveAsTaskItem.OnClicked += new CallbackTask.ClickedCallback(saveAsTaskItem_OnClicked);
            taskController.addTask(saveAsTaskItem);

            taskController.addTask(new ShowPopupTask(bookmarks, "Medical.Bookmarks", "Bookmarks", "FavoritesIcon", TaskMenuCategories.Navigation));
            taskController.addTask(new ShowToothContactsTask(0));
            taskController.addTask(new MDIDialogOpenTask(stateList, "Medical.StateList", "States", "StatesIcon", TaskMenuCategories.Patient));
            taskController.addTask(new MDIDialogOpenTask(notesDialog, "Medical.Notes", "Notes", "NotesIcon", TaskMenuCategories.Patient));
            taskController.addTask(new MDIDialogOpenTask(sequencePlayer, "Medical.Sequences", "Sequences", "SequenceIcon", TaskMenuCategories.Simulation, 1));
            taskController.addTask(new MDIDialogOpenTask(mandibleMovementDialog, "Medical.ManualMovement", "Manual Movement", "MovementIcon", TaskMenuCategories.Simulation, 2));
            taskController.addTask(new ChangeBackgroundColorTask(standaloneController.SceneViewController));
            taskController.addTask(windowLayout);
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

        public void sceneUnloading(SimScene scene)
        {
            mandibleMovementDialog.sceneUnloading(scene);
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            
        }

        public void sceneRevealed()
        {
            bookmarksController.loadSavedBookmarks();
        }

        public long PluginId
        {
            get
            {
                return (long)Features.Premium;
            }
        }

        public String PluginName
        {
            get
            {
                return "Premium Features";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return "";
            }
        }

        public void open()
        {
            openPatientDialog.show(0, 0);
        }

        public void save()
        {
            if (standaloneController.MedicalStateController.getNumStates() == 0 && standaloneController.ExamController.Count == 0)
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
            if (standaloneController.MedicalStateController.getNumStates() == 0 && standaloneController.ExamController.Count == 0)
            {
                MessageBox.show("No information to save. Please create some states using the wizards first.", "Nothing to save.", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
            }
            else
            {
                savePatientDialog.saveAs();
            }
        }

        public void changeActiveFile(PatientDataFile patientData)
        {
            if (patientData != null)
            {
                MainWindow.Instance.updateWindowTitle(String.Format("{0} {1}", patientData.FirstName, patientData.LastName));
                standaloneController.DocumentController.addToRecentDocuments(patientData.BackingFile);
            }
        }

        private void savePatientDialog_SaveFile(object sender, EventArgs e)
        {
            PatientDataFile patientData = savePatientDialog.PatientData;
            changeActiveFile(patientData);
            standaloneController.saveMedicalState(patientData);
        }

        private void openPatientDialog_OpenFile(object sender, EventArgs e)
        {
            PatientDataFile patientData = openPatientDialog.CurrentFile;
            changeActiveFile(patientData);
            standaloneController.openPatientFile(patientData);
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
