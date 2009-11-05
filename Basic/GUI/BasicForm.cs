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
using WeifenLuo.WinFormsUI.Docking;
using Medical.Properties;
using ComponentFactory.Krypton.Ribbon;

namespace Medical.GUI
{    
    public partial class BasicForm : MedicalForm
    {
        private BasicController controller;
        private OpenPatientDialog openPatient = new OpenPatientDialog();
        private SavePatientDialog savePatient = new SavePatientDialog();
        private AboutBox aboutBox = new AboutBox(Resources.articulometricsclinic);
        private ShortcutController shortcutController;
        private ToolStrip toolStrip1 = new ToolStrip();
        private ToolStripContainer toolStripContainer = new ToolStripContainer();
        private LayerGUIController layerGUIController;
        private WindowGUIController windowGUIController;

        public BasicForm(ShortcutController shortcuts)
        {
            InitializeComponent();
            this.initialize(Text);
            this.shortcutController = shortcuts;

            //navigationButton.ImageIndex = 5;

            ShortcutGroup shortcutGroup = shortcuts.createOrRetrieveGroup("MainUI");
            ShortcutEventCommand navigationShortcut = new ShortcutEventCommand("Navigation", Keys.Space, false);
            navigationShortcut.Execute += navigationShortcut_Execute;
            shortcutGroup.addShortcut(navigationShortcut);

            ShortcutEventCommand newShortcut = new ShortcutEventCommand("New", Keys.N, true);
            newShortcut.Execute += newShortcut_Execute;
            shortcutGroup.addShortcut(newShortcut);

            ShortcutEventCommand openShortcut = new ShortcutEventCommand("Open", Keys.O, true);
            openShortcut.Execute += openShortcut_Execute;
            shortcutGroup.addShortcut(openShortcut);

            ShortcutEventCommand saveShortcut = new ShortcutEventCommand("Save", Keys.S, true);
            saveShortcut.Execute += saveShortcut_Execute;
            shortcutGroup.addShortcut(saveShortcut);

            ShortcutEventCommand oneWindowShortcut = new ShortcutEventCommand("oneWindowShortcut", Keys.D1, true);
            oneWindowShortcut.Execute += new ShortcutEventCommand.ExecuteEvent(oneWindowShortcut_Execute);
            shortcutGroup.addShortcut(oneWindowShortcut);

            ShortcutEventCommand twoWindowShortcut = new ShortcutEventCommand("twoWindowShortcut", Keys.D2, true);
            twoWindowShortcut.Execute += new ShortcutEventCommand.ExecuteEvent(twoWindowShortcut_Execute);
            shortcutGroup.addShortcut(twoWindowShortcut);

            ShortcutEventCommand threeWindowShortcut = new ShortcutEventCommand("threeWindowShortcut", Keys.D3, true);
            threeWindowShortcut.Execute += new ShortcutEventCommand.ExecuteEvent(threeWindowShortcut_Execute);
            shortcutGroup.addShortcut(threeWindowShortcut);

            ShortcutEventCommand fourWindowShortcut = new ShortcutEventCommand("fourWindowShortcut", Keys.D4, true);
            fourWindowShortcut.Execute += new ShortcutEventCommand.ExecuteEvent(fourWindowShortcut_Execute);
            shortcutGroup.addShortcut(fourWindowShortcut);

            StatusController.StatusChanged += new StatusUpdater(StatusController_StatusChanged);

            //App menu
            changeSceneMenuItem.Click += new EventHandler(changeSceneMenuItem_Click);
            openMenuItem.Click += new EventHandler(openMenuItem_Click);
            saveMenuItem.Click += new EventHandler(saveMenuItem_Click);
            exitMenuItem.Click += new EventHandler(exitMenuItem_Click);

            //Navigation
            showNavigationButton.Click += new EventHandler(showNavigationButton_Click);
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
            layerGUIController.Dispose();
            openPatient.Dispose();
            savePatient.Dispose();
            base.Dispose(disposing);
        }

        public void initialize(BasicController controller, ImageList imageList)
        {
            this.controller = controller;
            toolStrip1.ImageList = imageList;
            windowGUIController = new WindowGUIController(this, controller);
            layerGUIController = new LayerGUIController(this, controller);
        }

        public void setViewMode()
        {
            //windowToolStripMenuItem.Visible = true;
            //toolsToolStripMenuItem.Visible = true;
            //distortionToolStripMenuItem.Visible = true;
            //newToolStripMenuItem.Visible = true;
            //openToolStripMenuItem.Visible = true;
            //saveToolStripMenuItem.Visible = true;
        }

        public void setDistortionMode()
        {
            //windowToolStripMenuItem.Visible = false;
            //toolsToolStripMenuItem.Visible = false;
            //distortionToolStripMenuItem.Visible = false;
            //newToolStripMenuItem.Visible = false;
            //openToolStripMenuItem.Visible = false;
            //saveToolStripMenuItem.Visible = false;
        }

        public void createDistortionMenu(IEnumerable<DistortionWizard> wizards)
        {
            foreach (DistortionWizard wizard in wizards)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(wizard.Name);
                item.Click += distortionWizardItem_Click;
//                distortionToolStripMenuItem.DropDownItems.Add(item);
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

        public ToolStripContainer ToolStrip
        {
            get
            {
                return toolStripContainer;
            }
        }

        public DockPanel DockPanel
        {
            get
            {
                return dockPanel;
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

        private void oneWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.PresetWindows.setPresetSet("One Window");
        }

        private void twoWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.PresetWindows.setPresetSet("Two Windows");
        }

        private void threeWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.PresetWindows.setPresetSet("Three Windows");
        }

        private void fourWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.PresetWindows.setPresetSet("Four Windows");
        }

        void fourWindowShortcut_Execute(ShortcutEventCommand shortcut)
        {
            fourWindowsToolStripMenuItem_Click(null, null);
        }

        void threeWindowShortcut_Execute(ShortcutEventCommand shortcut)
        {
            threeWindowsToolStripMenuItem_Click(null, null);
        }

        void twoWindowShortcut_Execute(ShortcutEventCommand shortcut)
        {
            twoWindowsToolStripMenuItem_Click(null, null);
        }

        void oneWindowShortcut_Execute(ShortcutEventCommand shortcut)
        {
            oneWindowToolStripMenuItem_Click(null, null);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.showOptions();
        }

        #endregion Window Management

        #region Navigation

        void showNavigationButton_Click(object sender, EventArgs e)
        {
            controller.ShowNavigation = showNavigationButton.Checked;
        }

        void navigationShortcut_Execute(ShortcutEventCommand shortcut)
        {
            showNavigationButton.Checked = !showNavigationButton.Checked;
            showNavigationButton_Click(null, null);
        }

        #endregion Navigation

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
    }
}
