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

namespace Medical.GUI
{    
    public partial class BasicForm : MedicalForm
    {
        private BasicController controller;
        private OpenPatientDialog openPatient = new OpenPatientDialog();
        private SavePatientDialog savePatient = new SavePatientDialog();
        private AboutBox aboutBox = new AboutBox(Resources.articulometricsclinic);
        private ShortcutController shortcutController;

        public BasicForm(ShortcutController shortcuts)
        {
            InitializeComponent();
            this.initialize(Text);
            this.shortcutController = shortcuts;

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

            ShortcutEventCommand distortionShortcut = new ShortcutEventCommand("AddDistortion", Keys.D, true);
            distortionShortcut.Execute += new ShortcutEventCommand.ExecuteEvent(distortionShortcut_Execute);
            shortcutGroup.addShortcut(distortionShortcut);

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
            openPatient.Dispose();
            savePatient.Dispose();
            base.Dispose(disposing);
        }

        public void initialize(BasicController controller)
        {
            this.controller = controller;
        }

        public void setViewMode()
        {
            windowToolStripMenuItem.Visible = true;
            toolsToolStripMenuItem.Visible = true;
            distortionToolStripMenuItem.Visible = true;
            newToolStripMenuItem.Visible = true;
            openToolStripMenuItem.Visible = true;
            saveToolStripMenuItem.Visible = true;
        }

        public void setDistortionMode()
        {
            windowToolStripMenuItem.Visible = false;
            toolsToolStripMenuItem.Visible = false;
            distortionToolStripMenuItem.Visible = false;
            newToolStripMenuItem.Visible = false;
            openToolStripMenuItem.Visible = false;
            saveToolStripMenuItem.Visible = false;
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addDistortionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.showStatePicker();
        }

        void distortionShortcut_Execute()
        {
            addDistortionToolStripMenuItem_Click(null, null);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.newScene();
        }

        void saveShortcut_Execute()
        {
            saveToolStripMenuItem_Click(null, null);
        }

        void openShortcut_Execute()
        {
            openToolStripMenuItem_Click(null, null);
        }

        void newShortcut_Execute()
        {
            newToolStripMenuItem_Click(null, null);
        }

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

        void fourWindowShortcut_Execute()
        {
            fourWindowsToolStripMenuItem_Click(null, null);
        }

        void threeWindowShortcut_Execute()
        {
            threeWindowsToolStripMenuItem_Click(null, null);
        }

        void twoWindowShortcut_Execute()
        {
            twoWindowsToolStripMenuItem_Click(null, null);
        }

        void oneWindowShortcut_Execute()
        {
            oneWindowToolStripMenuItem_Click(null, null);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.showOptions();
        }

        private void navigationButton_Click(object sender, EventArgs e)
        {
            controller.ShowNavigation = !navigationButton.Checked;
            navigationButton.Checked = controller.ShowNavigation;
        }

        void navigationShortcut_Execute()
        {
            navigationButton_Click(null, null);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutBox.ShowDialog(this);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            shortcutController.resetButtons();
        }
    }
}
