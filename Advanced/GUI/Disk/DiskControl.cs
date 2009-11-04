using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public partial class DiskControl : GUIElement
    {
        private bool allowSynchronization = true;

        public DiskControl()
        {
            InitializeComponent();
            horizontalOffsetTrackBar.ValueChanged += new EventHandler(horizontalOffsetTrackBar_ValueChanged);
            horizontalOffsetUpDown.ValueChanged += new EventHandler(horizontalOffsetUpDown_ValueChanged);
        }

        protected override void sceneLoaded(SimScene scene)
        {
            leftDiscPanel.sceneLoaded();
            rightDiscPanel.sceneLoaded();
        }

        protected override void sceneUnloading()
        {
            leftDiscPanel.sceneUnloaded();
            rightDiscPanel.sceneUnloaded();
        }

        private void distortionButton_Click(object sender, EventArgs e)
        {
            leftDiscPanel.reset();
            rightDiscPanel.reset();
            synchronizeHorizontalOffset(null, 0.0f);
        }

        //Synchronize Horizontal Offset
        private void synchronizeHorizontalOffset(object sender, float offset)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (sender != rightDiscPanel.Disc)
                {
                    Vector3 vec3Offset = new Vector3(offset, 0.0f, 0.0f);
                    rightDiscPanel.Disc.HorizontalOffset = vec3Offset;
                    leftDiscPanel.Disc.HorizontalOffset = vec3Offset;
                }
                if (sender != horizontalOffsetTrackBar)
                {
                    horizontalOffsetTrackBar.Value = (int)(offset * horizontalOffsetTrackBar.Maximum);
                }
                if (sender != horizontalOffsetUpDown)
                {
                    horizontalOffsetUpDown.Value = (decimal)offset;
                }
                allowSynchronization = true;
            }
        }

        void horizontalOffsetUpDown_ValueChanged(object sender, EventArgs e)
        {
            synchronizeHorizontalOffset(horizontalOffsetUpDown, (float)horizontalOffsetUpDown.Value);
        }

        void horizontalOffsetTrackBar_ValueChanged(object sender, EventArgs e)
        {
            synchronizeHorizontalOffset(horizontalOffsetTrackBar, horizontalOffsetTrackBar.Value / (float)horizontalOffsetTrackBar.Maximum);
        }
    }
}
