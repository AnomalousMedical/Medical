﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Controller;
using System.IO;
using Medical.Properties;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;
using Engine.Platform;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Docking;

namespace Medical.GUI
{    
    public partial class BasicForm : MedicalForm
    {
        private BasicController controller;
        private OpenPatientDialog openPatient = new OpenPatientDialog();
        private SavePatientDialog savePatient = new SavePatientDialog();
        private ClinicalAbout aboutBox;
        private ShortcutController shortcutController;
        private LayerGUIController layerGUIController;
        private WindowGUIController windowGUIController;
        private NavigationGUIController navigationGUIController;
        private RenderGUIController renderGUIController;
        private MandibleGUIController mandibleGUIController;
        private SequencesGUIController sequencesGUIController;
        private StateGUIController stateGUIController;
        private Size leftNavigatorSize;
        private BasicStateWizardHost stateWizardHost;

        private Dictionary<String, KryptonRibbonRecentDoc> recentDocsMap = new Dictionary<string, KryptonRibbonRecentDoc>();
        private RecentDocuments recentDocuments = MedicalConfig.RecentDocuments;

        private Dictionary<String, KryptonRibbonGroup> wizardGroups = new Dictionary<string, KryptonRibbonGroup>();

        private bool allowSimulationTab = true;

        public BasicForm(ShortcutController shortcuts, String featureLevelString)
        {
            InitializeComponent();
            aboutBox = new ClinicalAbout(featureLevelString);
            stateWizardHost = new BasicStateWizardHost(this);
            this.shortcutController = shortcuts;

            leftNavigatorSize = leftNavigator.Size;

            ShortcutGroup shortcutGroup = shortcuts.createOrRetrieveGroup("MainUI");
            
            ShortcutEventCommand newShortcut = new ShortcutEventCommand("New", Keys.N, true);
            newShortcut.Execute += newShortcut_Execute;
            shortcutGroup.addShortcut(newShortcut);

            ShortcutEventCommand openShortcut = new ShortcutEventCommand("Open", Keys.O, true);
            openShortcut.Execute += openShortcut_Execute;
            shortcutGroup.addShortcut(openShortcut);

            ShortcutEventCommand saveShortcut = new ShortcutEventCommand("Save", Keys.S, true);
            saveShortcut.Execute += saveShortcut_Execute;
            shortcutGroup.addShortcut(saveShortcut);

            StatusController.StatusChanged += new StatusUpdater(StatusController_StatusChanged);

            //App menu
            changeSceneMenuItem.Click += new EventHandler(changeSceneMenuItem_Click);
            openMenuItem.Click += new EventHandler(openMenuItem_Click);
            saveMenuItem.Click += new EventHandler(saveMenuItem_Click);
            saveAsMenuItem.Click += new EventHandler(saveAsMenuItem_Click);
            exitMenuItem.Click += new EventHandler(exitMenuItem_Click);

            showNavigationQATButton.Click += new EventHandler(showNavigationQATButton_Click);
            showTeethCollisionQATButton.Click += new EventHandler(showTeethCollisionQATButton_Click);

            recentDocuments.DocumentAdded += new RecentDocumentEvent(recentDocuments_DocumentAdded);
            recentDocuments.DocumentReaccessed += new RecentDocumentEvent(recentDocuments_DocumentReaccessed);
            recentDocuments.DocumentRemoved += new RecentDocumentEvent(recentDocuments_DocumentRemoved);
            foreach (String document in recentDocuments)
            {
                KryptonRibbonRecentDoc doc = new KryptonRibbonRecentDoc();
                doc.Text = Path.GetFileNameWithoutExtension(document);
                doc.ExtraText = Path.GetDirectoryName(document);
                doc.Click += recentDocument_Click;
                doc.Tag = document;
                clinicalRibbon.RibbonAppButton.AppButtonRecentDocs.Add(doc);
                recentDocsMap.Add(document, doc);
            }

            //Hide navigators
            leftNavigator.Visible = false; 
            leftNavigator.Pages.Removed += new TypedHandler<KryptonPage>(Pages_Removed);
            leftNavigator.Pages.Inserted += new TypedHandler<KryptonPage>(Pages_Inserted);
            leftNavigator.Pages.Cleared += new EventHandler(Pages_Cleared);

            //Hide info panels
            topInformationPanel.Visible = false;
            leftInformationPanel.Visible = false;

            //Docking
            KryptonDockingWorkspace w = kryptonDockingManager.ManageWorkspace("Workspace", kryptonDockableWorkspace);
            kryptonDockingManager.ManageFloating("Floating", this);

            aboutCommand.Execute += new EventHandler(aboutCommand_Execute);

            //Determine if simulation tab is enabled
            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_FEATURE_SIMULATION))
            {
                allowSimulationTab = true;
            }
            else
            {
                allowSimulationTab = false;
                simulationTab.Visible = false;
            }
        }

        void aboutCommand_Execute(object sender, EventArgs e)
        {
            aboutBox.ShowDialog(this);
        }

        void showTeethCollisionQATButton_Click(object sender, EventArgs e)
        {
            showTeethCollisionCommand.Checked = !showTeethCollisionCommand.Checked;
        }

        void showNavigationQATButton_Click(object sender, EventArgs e)
        {
            showNavigationCommand.Checked = !showNavigationCommand.Checked;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (layerGUIController != null)
            {
                layerGUIController.Dispose();
                openPatient.Dispose();
                savePatient.Dispose();
                stateGUIController.Dispose();
                windowGUIController.Dispose();
            }
            base.Dispose(disposing);
        }

        public void initialize(BasicController controller)
        {
            this.controller = controller;
            windowGUIController = new WindowGUIController(this, controller, shortcutController);
            layerGUIController = new LayerGUIController(this, controller, shortcutController);
            navigationGUIController = new NavigationGUIController(this, controller, shortcutController);
            renderGUIController = new RenderGUIController(this, controller, shortcutController);
            mandibleGUIController = new MandibleGUIController(this, controller);
            sequencesGUIController = new SequencesGUIController(this, controller);
            stateGUIController = new StateGUIController(this, controller);
        }

        public void addPrimaryNavigatorPage(KryptonPage page)
        {
            leftNavigator.Pages.Add(page);
        }

        public void removePrimaryNavigatorPage(KryptonPage page)
        {
            leftNavigator.Pages.Remove(page);
        }

        public void enableViewMode(bool enabled)
        {
            navigationTab.Visible = enabled;
            displayTab.Visible = enabled;
            distortionTab.Visible = enabled;
            simulationTab.Visible = enabled && allowSimulationTab;
            sequencesTab.Visible = enabled;
            renderingTab.Visible = enabled;
            windowTab.Visible = enabled;
            leftNavigator.Visible = enabled && leftNavigator.Pages.Count > 0;
            if (enabled)
            {
                clinicalRibbon.SelectedTab = distortionTab;
            }
            saveAsMenuItem.Enabled = enabled;
            saveMenuItem.Enabled = enabled;
            openMenuItem.Enabled = enabled;
            changeSceneMenuItem.Enabled = enabled;
            clinicalRibbon.RibbonAppButton.AppButtonShowRecentDocs = enabled;
            //mandibleGUIController.AllowSceneManipulation = enabled;
            this.Focus();
        }

        public void createDistortionMenu(IEnumerable<DistortionWizard> wizards)
        {
            KryptonRibbonGroup wizardRibbonGroup;
            KryptonRibbonGroupTriple currentTriple = null;
            foreach (DistortionWizard wizard in wizards)
            {
                wizardGroups.TryGetValue(wizard.Group, out wizardRibbonGroup);
                //If the group does not exist create it.
                if (wizardRibbonGroup == null)
                {
                    wizardRibbonGroup = new KryptonRibbonGroup();
                    wizardRibbonGroup.TextLine1 = wizard.Group;
                    wizardGroups.Add(wizard.Group, wizardRibbonGroup);
                    currentTriple = null;
                    distortionTab.Groups.Add(wizardRibbonGroup);
                }
                //Get the current triple out of the group if it already exists.
                else
                {
                    currentTriple = wizardRibbonGroup.Items[wizardRibbonGroup.Items.Count - 1] as KryptonRibbonGroupTriple;
                }
                //Add the item to the triple.
                if (currentTriple == null || currentTriple.Items.Count > 2)
                {
                    currentTriple = new KryptonRibbonGroupTriple();
                    wizardRibbonGroup.Items.Add(currentTriple);
                    currentTriple.MinimumSize = GroupItemSize.Large;
                }
                KryptonRibbonGroupButton wizardButton = new KryptonRibbonGroupButton();
                wizardButton.Click += new EventHandler(wizardButton_Click);
                wizardButton.TextLine1 = wizard.TextLine1;
                wizardButton.TextLine2 = wizard.TextLine2;
                wizardButton.ImageLarge = wizard.ImageLarge;
                wizardButton.Tag = wizard.Name;
                currentTriple.Items.Add(wizardButton);
            }
        }

        void wizardButton_Click(object sender, EventArgs e)
        {
            controller.showStatePicker(((KryptonRibbonGroupButton)sender).Tag.ToString());
        }

        void distortionWizardItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                controller.showStatePicker(item.Text);
            }
        }

        public KryptonDockingManager DockingManager
        {
            get
            {
                return kryptonDockingManager;
            }
        }

        public KryptonDockableWorkspace DockableWorkspace
        {
            get
            {
                return kryptonDockableWorkspace;
            }
        }

        public BasicStateWizardHost StateWizardHost
        {
            get
            {
                return stateWizardHost;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            controller.stop();
        }

        #region App Menu

        void saveMenuItem_Click(object sender, EventArgs e)
        {
            if (savePatient.save(this))
            {
                PatientDataFile patientData = savePatient.PatientData;
                controller.saveMedicalState(patientData);
                changeActiveFile(patientData);
                StatusController.SetStatus(String.Format("File saved to {0}", patientData.BackingFile));
            }
        }

        void saveAsMenuItem_Click(object sender, EventArgs e)
        {
            if (savePatient.saveAs(this))
            {
                PatientDataFile patientData = savePatient.PatientData;
                controller.saveMedicalState(patientData);
                changeActiveFile(patientData);
                StatusController.SetStatus(String.Format("File saved to {0}", patientData.BackingFile));
            }
        }

        void openMenuItem_Click(object sender, EventArgs e)
        {
            openPatient.ShowDialog(this);
            if (openPatient.FileChosen)
            {
                PatientDataFile patientData = openPatient.CurrentFile;
                changeActiveFile(patientData);
                controller.openStates(patientData);
            }
        }

        private bool validateFileName(String input, out String errorPrompt)
        {
            if (input == null || input == "")
            {
                errorPrompt = "Please enter a non empty name.";
                return false;
            }
            if (File.Exists(MedicalConfig.SaveDirectory + "/" + input + ".sim.xml"))
            {
                DialogResult msgRes = MessageBox.Show(this, "A file named " + input + " already exists. Would you like to overwrite it?", "Overwright?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (msgRes == DialogResult.No)
                {
                    errorPrompt = "Enter a different file name.";
                    return false;
                }
            }
            errorPrompt = "";
            return true;
        }

        void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void changeSceneMenuItem_Click(object sender, EventArgs e)
        {
            controller.newScene();
            changeActiveFile(null);
        }

        void saveShortcut_Execute(ShortcutEventCommand shortcut)
        {
            saveMenuItem_Click(null, null);
        }

        void openShortcut_Execute(ShortcutEventCommand shortcut)
        {
            openMenuItem_Click(null, null);
        }

        void newShortcut_Execute(ShortcutEventCommand shortcut)
        {
            changeSceneMenuItem_Click(null, null);
        }

        void changeActiveFile(PatientDataFile patientData)
        {
            if (patientData != null)
            {
                updateWindowTitle(String.Format("{0} {1} - {2}", patientData.FirstName, patientData.LastName, patientData.BackingFile));
                recentDocuments.addDocument(patientData.BackingFile);
            }
            else
            {
                clearWindowTitle();
            }
            savePatient.PatientData = patientData;
        }

        void recentDocuments_DocumentRemoved(RecentDocuments source, string document)
        {
            KryptonRibbonRecentDoc doc;
            recentDocsMap.TryGetValue(document, out doc);
            if (doc != null)
            {
                clinicalRibbon.RibbonAppButton.AppButtonRecentDocs.Remove(doc);
                recentDocsMap.Remove(document);
            }
        }

        void recentDocuments_DocumentReaccessed(RecentDocuments source, string document)
        {
            KryptonRibbonRecentDoc doc;
            recentDocsMap.TryGetValue(document, out doc);
            if (doc != null)
            {
                KryptonRibbonRecentDoc firstDoc = clinicalRibbon.RibbonAppButton.AppButtonRecentDocs[0];
                if (doc != firstDoc)
                {
                    clinicalRibbon.RibbonAppButton.AppButtonRecentDocs.MoveBefore(doc, firstDoc);
                }
            }
        }

        void recentDocuments_DocumentAdded(RecentDocuments source, string document)
        {
            KryptonRibbonRecentDoc doc = new KryptonRibbonRecentDoc();
            doc.Text = Path.GetFileNameWithoutExtension(document);
            doc.ExtraText = Path.GetDirectoryName(document);
            doc.Click += recentDocument_Click;
            doc.Tag = document;
            clinicalRibbon.RibbonAppButton.AppButtonRecentDocs.Insert(0, doc);
            recentDocsMap.Add(document, doc);
        }

        void recentDocument_Click(object sender, EventArgs e)
        {
            KryptonRibbonRecentDoc doc = sender as KryptonRibbonRecentDoc;
            if (doc != null)
            {
                PatientDataFile patientData = new PatientDataFile(doc.Tag.ToString());
                if (patientData.loadHeader())
                {
                    changeActiveFile(patientData);
                    controller.openStates(patientData);
                }
                else
                {
                    MessageBox.Show(this, String.Format("Error loading file {0}.", patientData.BackingFile), "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion App Menu

        #region Window Management

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.showOptions();
        }

        #endregion Window Management

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutBox.ShowDialog(this);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            shortcutController.resetButtons();
        }

        void StatusController_StatusChanged(string status)
        {
            mainStatusLabel.Text = status;
            Application.DoEvents();
        }

        private void buttonSpecExpandCollapse_Click(object sender, EventArgs e)
        {
            // Are we currently showing fully expanded?
            if (leftNavigator.NavigatorMode == NavigatorMode.HeaderBarCheckButtonGroup)
            {
                // Switch to mini mode and reverse direction of arrow
                leftNavigator.NavigatorMode = NavigatorMode.HeaderBarCheckButtonOnly;
                buttonSpecExpandCollapse.TypeRestricted = PaletteNavButtonSpecStyle.ArrowRight;
                leftNavigator.MinimumSize = new Size(0, 0);
            }
            else
            {
                // Switch to full mode and reverse direction of arrow
                leftNavigator.NavigatorMode = NavigatorMode.HeaderBarCheckButtonGroup;
                buttonSpecExpandCollapse.TypeRestricted = PaletteNavButtonSpecStyle.ArrowLeft;
                leftNavigator.MinimumSize = leftNavigatorSize;
            }
        }

        void Pages_Cleared(object sender, EventArgs e)
        {
            leftNavigator.Visible = false;
        }

        void Pages_Inserted(object sender, TypedCollectionEventArgs<KryptonPage> e)
        {
            leftNavigator.Visible = leftNavigator.Pages.Count > 0;
        }

        void Pages_Removed(object sender, TypedCollectionEventArgs<KryptonPage> e)
        {
            leftNavigator.Visible = leftNavigator.Pages.Count > 0;
        }
    }
}
