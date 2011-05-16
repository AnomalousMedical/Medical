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
    public class PremiumBodyAtlasPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private LicenseManager licenseManager;

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

        public PremiumBodyAtlasPlugin(LicenseManager licenseManager, StandaloneController standaloneController, AnatomyController anatomyController)
        {
            this.licenseManager = licenseManager;
            this.anatomyController = anatomyController;
            bookmarksController = new BookmarksController(standaloneController);
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
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            this.guiManager = guiManager;
            this.standaloneController = standaloneController;

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/BodyAtlas/Imagesets", "EngineArchive", "MyGUI", true);
            Gui.Instance.load("Imagesets.xml");
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
            taskbar.addItem(new DialogOpenTaskbarItem(stateList, "States", "Joint"));
            taskbar.addItem(new DialogOpenTaskbarItem(notesDialog, "Notes", "Notes"));
            taskbar.addItem(new DialogOpenTaskbarItem(sequencePlayer, "Sequences", "SequenceIconLarge"));
            taskbar.addItem(new DialogOpenTaskbarItem(mandibleMovementDialog, "Manual Movement", "MovementIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(windowLayout, "Window Layout", "WindowLayoutIconLarge"));

            //cloneWindow = new CloneWindowTaskbarItem(standaloneController);
            //if (PlatformConfig.AllowCloneWindows && licenseManager.allowFeature((int)Features.PIPER_JBO_FEATURE_CLONE_WINDOW))
            //{
            //    taskbar.addItem(cloneWindow);
            //}
        }

        public void finishInitialization()
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
