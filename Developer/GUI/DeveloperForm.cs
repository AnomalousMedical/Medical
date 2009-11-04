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
    public partial class DeveloperForm : MedicalForm
    {
        private DeveloperController controller;
        private FileTracker patientFileTracker = new FileTracker("*.pat|*.pat");
        private FileTracker sequenceFileTracker = new FileTracker("*.seq|*.seq");
        private DockArea dockArea;

        public DeveloperForm()
        {
            InitializeComponent();
            this.initialize(Text);
        }

        public void initialize(DeveloperController controller)
        {
            this.controller = controller;
        }

        public ToolStripContainer ToolStrip
        {
            get
            {
                return toolStripContainer;
            }
        }

        public DockArea DockPanel
        {
            get
            {
                return dockArea;
            }
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

        private void addStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.createMedicalState("Test");
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.newScene();
            clearWindowTitle();
            patientFileTracker.clearCurrentFile();
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

        private void newDistortion_Click(object sender, EventArgs e)
        {
            controller.createNewMedicalStates();
            clearWindowTitle();
            patientFileTracker.clearCurrentFile();
        }

        private void openDistortion_Click(object sender, EventArgs e)
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

        private void saveDistortion_Click(object sender, EventArgs e)
        {
            patientFileTracker.saveFile(this);
            if (patientFileTracker.lastDialogAccepted())
            {
                controller.saveMedicalState(patientFileTracker.getCurrentFile());
                updateWindowTitle(patientFileTracker.getCurrentFile());
            }
        }

        private void saveDistortionAs_Click(object sender, EventArgs e)
        {
            patientFileTracker.saveFileAs(this);
            if (patientFileTracker.lastDialogAccepted())
            {
                controller.saveMedicalState(patientFileTracker.getCurrentFile());
                updateWindowTitle(patientFileTracker.getCurrentFile());
            }
        }

        private void newSequence_Click(object sender, EventArgs e)
        {
            controller.createNewSequence();
        }

        private void openSequence_Click(object sender, EventArgs e)
        {
            sequenceFileTracker.openFile(this);
            if (sequenceFileTracker.lastDialogAccepted())
            {
                controller.loadSequence(sequenceFileTracker.getCurrentFile());
            }
        }

        private void saveSequence_Click(object sender, EventArgs e)
        {
            sequenceFileTracker.saveFile(this);
            if (sequenceFileTracker.lastDialogAccepted())
            {
                controller.saveSequence(sequenceFileTracker.getCurrentFile());
            }
        }

        private void saveSequenceAs_Click(object sender, EventArgs e)
        {
            sequenceFileTracker.saveFileAs(this);
            if (sequenceFileTracker.lastDialogAccepted())
            {
                controller.saveSequence(sequenceFileTracker.getCurrentFile());
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.showOptions();
        }
    }
}
