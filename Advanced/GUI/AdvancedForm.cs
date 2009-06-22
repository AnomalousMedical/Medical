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

        public AdvancedForm()
        {
            InitializeComponent();
            this.initialize(dockPanel, toolStripContainer);
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileTracker.openFile(this);
            if (fileTracker.lastDialogAccepted())
            {
                controller.open(fileTracker.getCurrentFile());
            }
        }
    }
}
