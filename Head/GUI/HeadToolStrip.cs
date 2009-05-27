using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Controller;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.GUI
{
    class HeadToolStrip : ToolStrip
    {
        private MuscleControl muscleControl = new MuscleControl();
        private TeethControl teethControl = new TeethControl();
        private MandibleOffsetControl mandibleOffsetControl = new MandibleOffsetControl();
        private MandibleSizeControl mandibleSizeControl = new MandibleSizeControl();
        private HeadController headController;
        private ToolStripButton teethButton;
        private ToolStripButton musclesButton;
        private ToolStripButton mandibleOffsetButton;
        private ToolStripButton mandibleSizeButton;

        public HeadToolStrip(HeadController headController)
        {
            this.headController = headController;

            musclesButton = new ToolStripButton("Muscles");
            musclesButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            musclesButton.Click += new EventHandler(musclesButton_Click);
            this.Items.Add(musclesButton);
            muscleControl.VisibleChanged += new EventHandler(muscleControl_VisibleChanged);

            mandibleOffsetButton = new ToolStripButton("Mandible Offset");
            mandibleOffsetButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mandibleOffsetButton.Click += new EventHandler(mandibleButton_Click);
            this.Items.Add(mandibleOffsetButton);
            mandibleOffsetControl.VisibleChanged += new EventHandler(mandibleControl_VisibleChanged);

            mandibleSizeButton = new ToolStripButton("Mandible Size");
            mandibleSizeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mandibleSizeButton.Click += new EventHandler(mandibleSizeButton_Click);
            this.Items.Add(mandibleSizeButton);
            mandibleSizeControl.VisibleChanged += new EventHandler(mandibleSizeControl_VisibleChanged);

            //ToolStripButton diskButton = new ToolStripButton("Disk");
            //diskButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            //this.Items.Add(diskButton);

            teethButton = new ToolStripButton("Teeth");
            teethButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            teethButton.Click += new EventHandler(teethButton_Click);
            this.Items.Add(teethButton);
            teethControl.VisibleChanged += new EventHandler(teethControl_VisibleChanged);
        }

        protected override void Dispose(bool disposing)
        {
            teethControl.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Used when restoring window positions. Return the window matching the
        /// persistString or null if no match is found.
        /// </summary>
        /// <param name="persistString">A string describing the window.</param>
        /// <returns>The matching DockContent or null if none is found.</returns>
        public DockContent getDockContent(String persistString)
        {
            if (persistString == muscleControl.GetType().ToString())
            {
                return muscleControl;
            }
            if (persistString == mandibleOffsetControl.GetType().ToString())
            {
                return mandibleOffsetControl;
            }
            if (persistString == mandibleSizeControl.GetType().ToString())
            {
                return mandibleSizeControl;
            }
            if (persistString == teethControl.GetType().ToString())
            {
                return teethControl;
            }
            return null;
        }

        /// <summary>
        /// Alert the GUIS that the scene has changed.
        /// </summary>
        public void sceneChanged()
        {
            muscleControl.sceneChanged();
        }

        void teethButton_Click(object sender, EventArgs e)
        {
            if (teethControl.Visible)
            {
                headController.removeControl(teethControl);
            }
            else
            {
                headController.addControlToUI(teethControl); 
            }
        }

        void teethControl_VisibleChanged(object sender, EventArgs e)
        {
            teethButton.Checked = teethControl.Visible;
        }

        void musclesButton_Click(object sender, EventArgs e)
        {
            if (muscleControl.Visible)
            {
                headController.removeControl(muscleControl);
            }
            else
            {
                headController.addControlToUI(muscleControl);
            }
        }

        void muscleControl_VisibleChanged(object sender, EventArgs e)
        {
            musclesButton.Checked = muscleControl.Visible;
        }

        void mandibleButton_Click(object sender, EventArgs e)
        {
            if (mandibleOffsetControl.Visible)
            {
                headController.removeControl(mandibleOffsetControl);
            }
            else
            {
                headController.addControlToUI(mandibleOffsetControl);
            }
        }

        void mandibleControl_VisibleChanged(object sender, EventArgs e)
        {
            mandibleOffsetButton.Checked = mandibleOffsetControl.Visible;
        }

        void mandibleSizeButton_Click(object sender, EventArgs e)
        {
            if (mandibleSizeControl.Visible)
            {
                headController.removeControl(mandibleSizeControl);
            }
            else
            {
                headController.addControlToUI(mandibleSizeControl);
            }
        }

        void mandibleSizeControl_VisibleChanged(object sender, EventArgs e)
        {
            mandibleSizeButton.Checked = mandibleSizeControl.Visible;
        }
    }
}
