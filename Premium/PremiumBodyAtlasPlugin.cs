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
        private StateListPopup stateList;
        private WindowLayout windowLayout;
        private SequencePlayer sequencePlayer;
        private AnatomyFinder anatomyFinder;
        private BookmarksGUI bookmarks;
        
        //Controllers
        private AnatomyController anatomyController;
        private BookmarksController bookmarksController;

        public PremiumBodyAtlasPlugin(StandaloneController standaloneController)
        {
            this.licenseManager = standaloneController.App.LicenseManager;
            anatomyController = new AnatomyController(standaloneController.ImageRenderer);
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
            sequencePlayer.Dispose();
            windowLayout.Dispose();
            mandibleMovementDialog.Dispose();
            notesDialog.Dispose();
            stateList.Dispose();
            anatomyFinder.Dispose();
            bookmarks.Dispose();
            bookmarksController.Dispose();
            anatomyController.Dispose();
        }

        public void initialize(StandaloneController standaloneController)
        {
            standaloneController.SceneViewController.AllowRotation = true;
            standaloneController.SceneViewController.AllowZoom = true;

            Gui.Instance.load("Medical.Resources.PremiumImagesets.xml");

            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;

            //Dialogs
            //BodyAtlas
            mandibleMovementDialog = new MandibleMovementDialog(standaloneController.MovementSequenceController);
            guiManager.addManagedDialog(mandibleMovementDialog);

            notesDialog = new NotesDialog(standaloneController.MedicalStateController);
            guiManager.addManagedDialog(notesDialog);

            stateList = new StateListPopup(standaloneController.MedicalStateController);
            guiManager.addManagedDialog(stateList);

            windowLayout = new WindowLayout(standaloneController);
            guiManager.addManagedDialog(windowLayout);

            sequencePlayer = new SequencePlayer(standaloneController.MovementSequenceController);
            guiManager.addManagedDialog(sequencePlayer);

            anatomyFinder = new AnatomyFinder(anatomyController, standaloneController.SceneViewController);
            guiManager.addManagedDialog(anatomyFinder);

            bookmarks = new BookmarksGUI(bookmarksController);

            //Taskbar
            Taskbar taskbar = guiManager.Taskbar;
            taskbar.addItem(new ShowPopupTaskbarItem(bookmarks, "Bookmarks", "FavoritesIcon"));

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(anatomyFinder, "Medical.AnatomyFinder", "Anatomy Finder", "SearchIcon", TaskMenuCategories.Navigation));
            taskController.addTask(new ShowToothContactsTask());
            taskController.addTask(new MDIDialogOpenTask(stateList, "Medical.StateList", "States", "StatesIcon", TaskMenuCategories.Patient));
            taskController.addTask(new MDIDialogOpenTask(notesDialog, "Medical.Notes", "Notes", "NotesIcon", TaskMenuCategories.Patient));
            taskController.addTask(new MDIDialogOpenTask(sequencePlayer, "Medical.Sequences", "Sequences", "SequenceIcon", TaskMenuCategories.Simulation));
            taskController.addTask(new MDIDialogOpenTask(mandibleMovementDialog, "Medical.ManualMovement", "Manual Movement", "MovementIcon", TaskMenuCategories.Simulation));
            taskController.addTask(new DialogOpenTask(windowLayout, "Medical.WindowLayout", "Window Layout", "WindowLayoutIcon", TaskMenuCategories.System));
        }

        public void sceneLoaded(SimScene scene)
        {
            mandibleMovementDialog.sceneLoaded(scene);
            anatomyController.sceneLoaded();

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
            anatomyController.sceneUnloading();
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
    }
}
