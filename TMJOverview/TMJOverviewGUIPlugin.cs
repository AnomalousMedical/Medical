using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.ObjectManagement;
using MyGUIPlugin;
using OgreWrapper;
using Engine;

namespace Medical
{
    class TMJOverviewGUIPlugin : GUIPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;

        private DopplerAppMenu appMenu;
        private OptionsDialog options;
        private SavePatientDialog savePatientDialog;
        private QuickView quickView;
        private SequencePlayer sequencePlayer;
        private AboutDialog aboutDialog;
        private Intro intro;
        private SystemMenu systemMenu;
        private ChooseSceneDialog chooseSceneDialog;
        private PredefinedLayersDialog predefinedLayers;

        public TMJOverviewGUIPlugin()
        {
            
        }

        public void Dispose()
        {
            chooseSceneDialog.Dispose();
            predefinedLayers.Dispose();
            aboutDialog.Dispose();
            appMenu.Dispose();
            options.Dispose();
            savePatientDialog.Dispose();
            quickView.Dispose();
            sequencePlayer.Dispose();
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            this.standaloneController = standaloneController;
            this.guiManager = guiManager;

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/TMJOverview/Imagesets", "EngineArchive", "MyGUI", true);
            Gui.Instance.load("Imagesets.xml");

            appMenu = new DopplerAppMenu(this, standaloneController);
            guiManager.setAppMenu(appMenu);

            standaloneController.TimelineController.PlaybackStarted += new EventHandler(TimelineController_PlaybackStarted);
            standaloneController.TimelineController.PlaybackStopped += new EventHandler(TimelineController_PlaybackStopped);
        }

        public void createDialogs(DialogManager dialogManager)
        {
            chooseSceneDialog = new ChooseSceneDialog();
            chooseSceneDialog.ChooseScene += new EventHandler(chooseSceneDialog_ChooseScene);

            options = new OptionsDialog();
            options.VideoOptionsChanged += new EventHandler(options_VideoOptionsChanged);

            savePatientDialog = new SavePatientDialog();
            savePatientDialog.SaveFile += new EventHandler(savePatientDialog_SaveFile);

            quickView = new QuickView(standaloneController.NavigationController, standaloneController.SceneViewController, standaloneController.LayerController);
            dialogManager.addManagedDialog(quickView);

            sequencePlayer = new SequencePlayer(standaloneController.MovementSequenceController);
            dialogManager.addManagedDialog(sequencePlayer);

            aboutDialog = new AboutDialog("N/A");
            dialogManager.addManagedDialog(aboutDialog);

            predefinedLayers = new PredefinedLayersDialog(standaloneController.LayerController);
            dialogManager.addManagedDialog(predefinedLayers);

            intro = new Intro(standaloneController.App.WindowTitle, this);
        }

        public void addToTaskbar(Taskbar taskbar)
        {
            taskbar.addItem(new DialogOpenTaskbarItem(quickView, "Quick View", "Camera"));
            taskbar.addItem(new DialogOpenTaskbarItem(predefinedLayers, "Predefined Layers", "PreDefinedLayersIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(sequencePlayer, "Sequences", "SequenceIconLarge"));
        }

        public void finishInitialization()
        {
            setInterfaceEnabled(false);
            intro.center();
            intro.open(true);
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
            systemMenu = new SystemMenu(menu, this, standaloneController);
            systemMenu.FileMenuEnabled = false;
        }

        public void openNewScene()
        {
            chooseSceneDialog.open(true);
        }

        void chooseSceneDialog_ChooseScene(object sender, EventArgs e)
        {
            standaloneController.openNewScene(chooseSceneDialog.SelectedFile);
        }

        public void showOptions()
        {
            options.Visible = true;
        }

        public void showAboutDialog()
        {
            aboutDialog.open(true);
        }

        public void export()
        {
            if (standaloneController.MedicalStateController.getNumStates() == 0)
            {
                MessageBox.show("No information to save. Please run the diagnosis first.", "Nothing to save.", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
            }
            else
            {
                savePatientDialog.saveAs();
            }
        }

        public void runDetailedDiagnosis()
        {
            standaloneController.MedicalStateController.clearStates();
            standaloneController.MedicalStateController.createNormalStateFromScene();
            Timeline tl = standaloneController.TimelineController.openTimeline("A Startup.tl");
            standaloneController.TimelineController.startPlayback(tl);
        }

        public void runQuickDiagnosis()
        {
            standaloneController.MedicalStateController.clearStates();
            standaloneController.MedicalStateController.createNormalStateFromScene();
            Timeline tl = standaloneController.TimelineController.openTimeline("A Startup.tl");
            standaloneController.TimelineController.startPlayback(tl);
        }

        public void startSandboxMode()
        {
            setInterfaceEnabled(true);
        }

        void options_VideoOptionsChanged(object sender, EventArgs e)
        {
            standaloneController.recreateMainWindow();
        }

        private void savePatientDialog_SaveFile(object sender, EventArgs e)
        {
            PatientDataFile patientData = savePatientDialog.PatientData;
            //changeActiveFile(patientData);
            standaloneController.saveMedicalState(patientData);
        }

        void TimelineController_PlaybackStopped(object sender, EventArgs e)
        {
            standaloneController.MedicalStateController.createAndAddState("Doppler Results");
            setInterfaceEnabled(true);
        }

        void TimelineController_PlaybackStarted(object sender, EventArgs e)
        {
            setInterfaceEnabled(false);
        }

        void setInterfaceEnabled(bool enable)
        {
            guiManager.setMainInterfaceEnabled(enable);
            if(systemMenu != null)
            {
                systemMenu.FileMenuEnabled = enable;
            }
        }
    }
}
