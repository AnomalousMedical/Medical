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

namespace Medical.GUI
{
    public partial class BasicForm : MedicalForm
    {
        private BasicController controller;
        private OpenPatientDialog openPatient = new OpenPatientDialog();
        private SavePatientDialog savePatient = new SavePatientDialog();

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
