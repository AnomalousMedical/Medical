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

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            Gui.Instance.load("Medical.Resources.PremiumImagesets.xml");

            this.guiManager = guiManager;
            this.standaloneController = standaloneController;
        }

        public void createDialogs(DialogManager dialogManager)
        {
            //BodyAtlas
            mandibleMovementDialog = new MandibleMovementDialog(standaloneController.MovementSequenceController);
            dialogManager.addManagedDialog(mandibleMovementDialog);

            notesDialog = new NotesDialog(standaloneController.MedicalStateController);
            dialogManager.addManagedDialog(notesDialog);

            stateList = new StateListPopup(standaloneController.MedicalStateController);
            dialogManager.addManagedDialog(stateList);

            windowLayout = new WindowLayout(standaloneController);
            dialogManager.addManagedDialog(windowLayout);

            sequencePlayer = new SequencePlayer(standaloneController.MovementSequenceController);
            dialogManager.addManagedDialog(sequencePlayer);

            anatomyFinder = new AnatomyFinder(anatomyController, standaloneController.SceneViewController);
            dialogManager.addManagedDialog(anatomyFinder);

            bookmarks = new BookmarksGUI(bookmarksController);
        }

        public void addToTaskbar(Taskbar taskbar)
        {
            taskbar.addItem(new ShowPopupTaskbarItem(bookmarks, "Bookmarks", "FavoritesIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(anatomyFinder, "Anatomy Finder", "SearchIcon"));
            taskbar.addItem(new ShowToothContactsTaskbarItem());
            taskbar.addItem(new DialogOpenTaskbarItem(stateList, "States", "StatesIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(notesDialog, "Notes", "NotesIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(sequencePlayer, "Sequences", "SequenceIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(mandibleMovementDialog, "Manual Movement", "MovementIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(windowLayout, "Window Layout", "WindowLayoutIcon"));
        }

        public void finishInitialization()
        {
            
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
