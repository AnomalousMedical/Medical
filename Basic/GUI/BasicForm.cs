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

namespace Medical.GUI
{
    public partial class BasicForm : MedicalForm
    {
        private BasicController controller;
        private OpenPatientDialog openPatient = new OpenPatientDialog();

        public BasicForm()
        {
            InitializeComponent();
            this.initialize(dockPanel, toolStripContainer);
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
            base.Dispose(disposing);
        }

        public void initialize(BasicController controller)
        {
            this.controller = controller;
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
            //fileTracker.saveFile(this);
            //if (fileTracker.lastDialogAccepted())
            //{
            //    controller.saveMedicalState(fileTracker.getCurrentFile());
            //}
            InputResult result = InputBox.GetInput("Save", "Enter a file name", this);
            bool saveFile = true;
            if (result.ok)
            {
                while (File.Exists(MedicalConfig.SaveDirectory + "/" + result.text + ".sim.xml") && saveFile)
                {
                    DialogResult msgRes = MessageBox.Show(this, "A file named " + result.text + " already exists. Would you like to overwrite it?", "Overwright?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (msgRes == DialogResult.No)
                    {
                        result = InputBox.GetInput("Save", "Enter a different file name", this, result.text);
                        if (!result.ok)
                        {
                            saveFile = false;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                saveFile = false;
            }
            if (saveFile)
            {
                controller.saveMedicalState(MedicalConfig.SaveDirectory + "/" + result.text + ".sim.xml");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addDistortionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.showStateWizard();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.openDefaultScene();
        }

        private void oneWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.setOneWindowLayout();
        }

        private void twoWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.setTwoWindowLayout();
        }

        private void threeWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.setThreeWindowLayout();
        }

        private void fourWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.setFourWindowLayout();
        }
    }
}
