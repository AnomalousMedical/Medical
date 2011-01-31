using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class PiperJBOGUIPlugin : GUIPlugin
    {
        private StandaloneController standaloneController;

        private MandibleMovementDialog mandibleMovementDialog;
        private LayersDialog layers;

        private NotesDialog notesDialog;
        private StateListPopup stateList;
        private ChooseSceneDialog chooseSceneDialog;
        private SavePatientDialog savePatientDialog;
        private OpenPatientDialog openPatientDialog;
        private OptionsDialog options;
        private RenderPropertiesDialog renderDialog;
        private CameraControls cameraControlDialog;

        private DistortionsPopup distortionsPopup;
        private QuickViewPopup quickViewPopup;
        private ColorMenu colorMenu;

        private AboutDialog aboutDialog;

        private PiperJBOAppMenu appMenu;
        private GUIManager guiManager;
        private PiperJBOWizards wizards;
        private CloneWindowTaskbarItem cloneWindow;

        public PiperJBOGUIPlugin()
        {

        }

        public void Dispose()
        {
            cameraControlDialog.Dispose();
            colorMenu.Dispose();
            quickViewPopup.Dispose();
            distortionsPopup.Dispose();
            renderDialog.Dispose();
            options.Dispose();
            chooseSceneDialog.Dispose();
            savePatientDialog.Dispose();
            openPatientDialog.Dispose();
            appMenu.Dispose();
            aboutDialog.Dispose();
            mandibleMovementDialog.Dispose();
            layers.Dispose();
            notesDialog.Dispose();
            stateList.Dispose();
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            this.guiManager = guiManager;
            this.standaloneController = standaloneController;
            appMenu = new PiperJBOAppMenu(this, standaloneController);
            guiManager.setAppMenu(appMenu);
        }

        public void createDialogs(DialogManager dialogManager)
        {
            //PiperJBO
            mandibleMovementDialog = new MandibleMovementDialog(standaloneController.MovementSequenceController);
            dialogManager.addManagedDialog(mandibleMovementDialog);

            layers = new LayersDialog(standaloneController.LayerController);
            dialogManager.addManagedDialog(layers);

            //Common
            notesDialog = new NotesDialog(standaloneController.MedicalStateController);
            dialogManager.addManagedDialog(notesDialog);

            stateList = new StateListPopup(standaloneController.MedicalStateController);
            dialogManager.addManagedDialog(stateList);

            aboutDialog = new AboutDialog();

            chooseSceneDialog = new ChooseSceneDialog();
            chooseSceneDialog.ChooseScene += new EventHandler(chooseSceneDialog_ChooseScene);

            savePatientDialog = new SavePatientDialog();
            savePatientDialog.SaveFile += new EventHandler(savePatientDialog_SaveFile);

            openPatientDialog = new OpenPatientDialog();
            openPatientDialog.OpenFile += new EventHandler(openPatientDialog_OpenFile);

            options = new OptionsDialog();
            options.VideoOptionsChanged += new EventHandler(options_VideoOptionsChanged);

            renderDialog = new RenderPropertiesDialog(standaloneController.SceneViewController, standaloneController.ImageRenderer);
            dialogManager.addManagedDialog(renderDialog);

            quickViewPopup = new QuickViewPopup(standaloneController.NavigationController, standaloneController.SceneViewController, standaloneController.LayerController);

            colorMenu = new ColorMenu();
            colorMenu.ColorChanged += new EventHandler(colorMenu_ColorChanged);

            cameraControlDialog = new CameraControls(standaloneController.SceneViewController);
            dialogManager.addManagedDialog(cameraControlDialog);

            //Wizards
            wizards = new PiperJBOWizards(guiManager.StateWizardPanelController, guiManager.StateWizardController);

            //Distortions Popup, must come after wizards
            distortionsPopup = new DistortionsPopup(guiManager.StateWizardController, guiManager);
        }

        public void addToTaskbar(Taskbar taskbar)
        {
            taskbar.addItem(new ShowToothContactsTaskbarItem());
            taskbar.addItem(new ShowPopupTaskbarItem(quickViewPopup, "Quick View", "Camera"));
            taskbar.addItem(new DialogOpenTaskbarItem(layers, "Custom Layers", "ManualObject"));
            taskbar.addItem(new ShowPopupTaskbarItem(distortionsPopup, "Distortions", "RigidBody"));
            taskbar.addItem(new DialogOpenTaskbarItem(stateList, "States", "Joint"));
            taskbar.addItem(new DialogOpenTaskbarItem(notesDialog, "Notes", "Notes"));
            taskbar.addItem(new SequencesTaskbarItem(standaloneController.MovementSequenceController));
            taskbar.addItem(new DialogOpenTaskbarItem(mandibleMovementDialog, "Manual Movement", "MovementIcon"));
            taskbar.addItem(new WindowLayoutTaskbarItem(standaloneController));
            taskbar.addItem(new DialogOpenTaskbarItem(cameraControlDialog, "Camera Controls", "Camera"));

            DialogOpenTaskbarItem renderTaskbarItem = new DialogOpenTaskbarItem(renderDialog, "Render", "RenderIconLarge");
            renderTaskbarItem.RightClicked += new EventHandler(renderTaskbarItem_RightClicked);
            taskbar.addItem(renderTaskbarItem);
            taskbar.addItem(new ShowPopupTaskbarItem(colorMenu, "Background Color", "BackgroundColorIconLarge"));

            cloneWindow = new CloneWindowTaskbarItem(standaloneController);

#if ALLOW_CLONE_WINDOWS
            taskbar.addItem(cloneWindow);
#endif
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
            layers.AllowShortcuts = enabled;
#if CREATE_MAINWINDOW_MENU
            systemMenu.FileMenuEnabled = enabled;
#endif
        }

#if CREATE_MAINWINDOW_MENU
        private SystemMenu systemMenu;
#endif

        public void createMenuBar(NativeMenuBar menu)
        {
#if CREATE_MAINWINDOW_MENU
            systemMenu = new SystemMenu(menu, this, standaloneController);
#endif
        }

        public void showOptions()
        {
            options.Visible = true;
        }

        public void showAboutDialog()
        {
            aboutDialog.open(true);
        }

        internal void toggleCloneWindow()
        {
            cloneWindow.toggleCloneWindow();
        }

        public void showChooseSceneDialog()
        {
            chooseSceneDialog.open(true);
        }

        public void open()
        {
            openPatientDialog.open(true);
        }

        public void save()
        {
            if (standaloneController.MedicalStateController.getNumStates() == 0)
            {
                MessageBox.show("No information to save. Please create some states using the wizards first.", "Nothing to save.", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
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

        public void changeActiveFile(PatientDataFile patientData)
        {
            if (patientData != null)
            {
                MainWindow.Instance.updateWindowTitle(String.Format("{0} {1}", patientData.FirstName, patientData.LastName));
                MedicalConfig.RecentDocuments.addDocument(patientData.BackingFile);
            }
            else
            {
                MainWindow.Instance.clearWindowTitle();
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

        private void options_VideoOptionsChanged(object sender, EventArgs e)
        {
            standaloneController.recreateMainWindow();
        }

        private void chooseSceneDialog_ChooseScene(object sender, EventArgs e)
        {
            changeActiveFile(null);
            standaloneController.openNewScene(chooseSceneDialog.SelectedFile);
        }

        void renderTaskbarItem_RightClicked(object sender, EventArgs e)
        {
            renderDialog.render();
        }

        void colorMenu_ColorChanged(object sender, EventArgs e)
        {
            standaloneController.SceneViewController.ActiveWindow.BackColor = colorMenu.SelectedColor;
        }
    }
}
