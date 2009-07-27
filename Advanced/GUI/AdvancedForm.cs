using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Controller;

namespace Medical.GUI
{
    public partial class AdvancedForm : MedicalForm
    {
        private AdvancedController controller;
        private FileTracker patientFileTracker = new FileTracker("*.pat|*.pat");

        public AdvancedForm()
        {
            InitializeComponent();
            this.initialize(dockPanel, toolStripContainer, Text);
        }

        public void initialize(AdvancedController controller)
        {
            this.controller = controller;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            controller.stop();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.createMedicalState("Test");
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.newScene();
            clearWindowTitle();
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

        private void fourWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.setFourWindowLayout();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            patientFileTracker.saveFile(this);
            if (patientFileTracker.lastDialogAccepted())
            {
                controller.saveMedicalState(patientFileTracker.getCurrentFile());
                updateWindowTitle(patientFileTracker.getCurrentFile());
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            patientFileTracker.saveFileAs(this);
            if (patientFileTracker.lastDialogAccepted())
            {
                controller.saveMedicalState(patientFileTracker.getCurrentFile());
                updateWindowTitle(patientFileTracker.getCurrentFile());
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            patientFileTracker.openFile(this);
            if (patientFileTracker.lastDialogAccepted())
            {
                controller.openStates(patientFileTracker.getCurrentFile());
                updateWindowTitle(patientFileTracker.getCurrentFile());
            }
            //fileTracker.openFile(this);
            //if (fileTracker.lastDialogAccepted())
            //{
            //    controller.open(fileTracker.getCurrentFile());
            //}
        }
    }
}
