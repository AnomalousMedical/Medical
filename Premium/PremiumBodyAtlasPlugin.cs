﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using OgrePlugin;
using System.Diagnostics;
using Medical.GUI;
using Engine;
using Anomalous.GuiFramework;

namespace Medical
{
    public class PremiumBodyAtlasPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private LicenseManager licenseManager;
        
        //Dialogs
        private NotesDialog notesDialog;
        private StateListDialog stateList;
        private SavePatientDialog savePatientDialog;
        private OpenPatientDialog openPatientDialog;
        private CloneWindowDialog cloneWindowDialog;

        //Tasks
        private ChangeWindowLayoutTask windowLayout;

        public PremiumBodyAtlasPlugin(StandaloneController standaloneController)
        {
            this.licenseManager = standaloneController.LicenseManager;
        }

        public void Dispose()
        {
            cloneWindowDialog.Dispose();
            stateList.Dispose();
            windowLayout.Dispose();
            notesDialog.Dispose();
            savePatientDialog.Dispose();
            openPatientDialog.Dispose();
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Medical.Resources.PremiumImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            standaloneController.DocumentController.addDocumentHandler(new PatientDocumentHandler(standaloneController));

            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;
            standaloneController.PatientDataController.PatientDataChanged += new Action<PatientDataFile>(PatientDataController_PatientDataChanged);

            standaloneController.AnatomyController.ShowPremiumAnatomy = true;

            //Dialogs
            notesDialog = new NotesDialog(standaloneController.MedicalStateController);
            guiManager.addManagedDialog(notesDialog);

            stateList = new StateListDialog(standaloneController.MedicalStateController);
            guiManager.addManagedDialog(stateList);

            savePatientDialog = new SavePatientDialog(guiManager);
            savePatientDialog.SaveFile += new EventHandler(savePatientDialog_SaveFile);

            openPatientDialog = new OpenPatientDialog(guiManager);
            openPatientDialog.OpenFile += new EventHandler(openPatientDialog_OpenFile);

            cloneWindowDialog = new CloneWindowDialog();

            //Tasks
            windowLayout = new ChangeWindowLayoutTask(standaloneController.SceneViewController);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new ShowPopupTask(openPatientDialog, "Medical.OpenPatient", "Open", "PremiumFeatures/Open", TaskMenuCategories.Patient, 1));

            CallbackTask saveTaskItem = new CallbackTask("Medical.SavePatient", "Save", "CommonToolstrip/Save", TaskMenuCategories.Patient, 2, false);
            saveTaskItem.OnClicked += new CallbackTask.ClickedCallback(saveTaskItem_OnClicked);
            taskController.addTask(saveTaskItem);

            CallbackTask saveAsTaskItem = new CallbackTask("Medical.SavePatientAs", "Save As", "CommonToolstrip/SaveAs", TaskMenuCategories.Patient, 3, false);
            saveAsTaskItem.OnClicked += new CallbackTask.ClickedCallback(saveAsTaskItem_OnClicked);
            taskController.addTask(saveAsTaskItem);

            PinableMDIDialogOpenTask statesTask = new PinableMDIDialogOpenTask(stateList, "Medical.StateList", "States", "PremiumFeatures/StatesIcon", TaskMenuCategories.Patient);
            taskController.addTask(statesTask);

            PinableMDIDialogOpenTask notesTask = new PinableMDIDialogOpenTask(notesDialog, "Medical.Notes", "Notes", "PremiumFeatures/NotesIcon", TaskMenuCategories.Patient);
            taskController.addTask(notesTask);

            taskController.addTask(new ChangeBackgroundColorTask(standaloneController.SceneViewController));
            if (PlatformConfig.AllowCloneWindows)
            {
                standaloneController.TaskController.addTask(new CloneWindowTask(standaloneController, cloneWindowDialog));
            }
            taskController.addTask(windowLayout);
        }

        public void sceneLoaded(SimScene scene)
        {
            windowLayout.sceneLoaded(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public long PluginId
        {
            get
            {
                return 1;
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
                return "PremiumFeatures/BrandingImage";
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

        public bool AllowUninstall
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<long> DependencyPluginIds
        {
            get
            {
                return IEnumerableUtil<long>.EmptyIterator;
            }
        }

        public void open()
        {
            openPatientDialog.show(0, 0);
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
