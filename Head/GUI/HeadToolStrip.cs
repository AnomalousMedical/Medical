using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Controller;

namespace Medical.GUI
{
    class HeadToolStrip : ToolStrip
    {
        private TeethControl teethControl = new TeethControl();
        private HeadController headController;
        private ToolStripButton teethButton;

        public HeadToolStrip(HeadController headController)
        {
            this.headController = headController;

            ToolStripButton musclesButton = new ToolStripButton("Muscles");
            musclesButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.Items.Add(musclesButton);

            ToolStripButton mandibleButton = new ToolStripButton("Mandible");
            mandibleButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.Items.Add(mandibleButton);

            ToolStripButton diskButton = new ToolStripButton("Disk");
            diskButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.Items.Add(diskButton);

            teethButton = new ToolStripButton("Teeth");
            teethButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            teethButton.Click += new EventHandler(teethButton_Click);
            this.Items.Add(teethButton);
            teethControl.ParentChanged += new EventHandler(teethControl_ParentChanged);
        }

        protected override void Dispose(bool disposing)
        {
            teethControl.Dispose();
            base.Dispose(disposing);
        }

        void teethButton_Click(object sender, EventArgs e)
        {
            if (teethControl.Parent == null)
            {
                headController.addControlToUI(teethControl);
            }
            else
            {
                headController.removeControl(teethControl);
            }
        }

        void teethControl_ParentChanged(object sender, EventArgs e)
        {
            teethButton.Checked = teethControl.Parent != null;
        }
    }
}
