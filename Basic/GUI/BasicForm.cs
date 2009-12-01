using System;
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
        private AboutBox aboutBox = new AboutBox(Resources.articulometricsclinic);
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

        public BasicForm(ShortcutController shortcuts)
        {
            InitializeComponent();
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
            exitMenuItem.Click += new EventHandler(exitMenuItem_Click);

            showNavigationQATButton.Click += new EventHandler(showNavigationQATButton_Click);
            showTeethCollisionQATButton.Click += new EventHandler(showTeethCollisionQATButton_Click);

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

            //temporary
            tempStateButton.Click += new EventHandler(tempStateButton_Click);

            aboutCommand.Execute += new EventHandler(aboutCommand_Execute);
        }

        void aboutCommand_Execute(object sender, EventArgs e)
        {
            aboutBox.ShowDialog(this);
        }

        void tempStateButton_Click(object sender, EventArgs e)
        {
            controller.showStatePicker(tempStateButton.TextLine1);
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
            distortionTab.Visible = enabled;
            simulationTab.Visible = enabled;
            sequencesTab.Visible = enabled;
            renderingTab.Visible = enabled;
            windowTab.Visible = enabled;
            leftNavigator.Visible = enabled && leftNavigator.Pages.Count > 0;
        }

        public void createDistortionMenu(IEnumerable<DistortionWizard> wizards)
        {
            foreach (DistortionWizard wizard in wizards)
            {
                //ToolStripMenuItem item = new ToolStripMenuItem(wizard.Name);
                //item.Click += distortionWizardItem_Click;
                //distortionToolStripMenuItem.DropDownItems.Add(item);
            }
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
            //InputResult result = InputBox.GetInput("Save", "Enter a file name", this, validateFileName);
            //if (result.ok)
            //{
            //    controller.saveMedicalState(MedicalConfig.SaveDirectory + "/" + result.text + ".sim.xml");
            //}
            savePatient.ShowDialog(this);
            if (savePatient.SaveFile)
            {
                controller.saveMedicalState(savePatient.Filename);
                StatusController.SetStatus(String.Format("File saved to {0}", savePatient.Filename));
            }
        }

        void openMenuItem_Click(object sender, EventArgs e)
        {
            //fileTracker.openFile(this);
            //if (fileTracker.lastDialogAccepted())
            //{
            //    controller.openStates(fileTracker.getCurrentFile());
            //}
            openPatient.listFiles(MedicalConfig.SaveDirectory);
            openPatient.ShowDialog(this);
            if (openPatient.FileChosen)
            {
                controller.openStates(openPatient.CurrentFile);
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
