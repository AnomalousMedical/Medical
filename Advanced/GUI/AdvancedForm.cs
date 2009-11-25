using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Controller;
using WeifenLuo.WinFormsUI.Docking;
using ComponentFactory.Krypton.Ribbon;

namespace Medical.GUI
{
    public partial class AdvancedForm : MedicalForm
    {
        private AdvancedController controller;
        private FileTracker patientFileTracker = new FileTracker("*.pat|*.pat");
        private FileTracker sequenceFileTracker = new FileTracker("*.seq|*.seq");

        public AdvancedForm()
        {
            InitializeComponent();
            this.initialize(Text);
        }

        public void initialize(AdvancedController controller)
        {
            this.controller = controller;
        }

        public KryptonRibbonTab HomeTab
        {
            get
            {
                return homeTab;
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

        private void cloneSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.cloneActiveWindow();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.showOptions();
        }
    }
}
