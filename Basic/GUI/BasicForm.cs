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
    public partial class BasicForm : MedicalForm
    {
        private BasicController controller;

        public BasicForm()
        {
            InitializeComponent();
            this.initialize(dockPanel, toolStripContainer);
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
            fileTracker.openFile(this);
            if (fileTracker.lastDialogAccepted())
            {
                controller.open(fileTracker.getCurrentFile());
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
