using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.ObjectManagement;
using Engine;

namespace Medical.GUI
{
    class DiscControl : FixedSizeMDIDialog
    {
        private bool allowSynchronization = true;

        MinMaxScroll horizontalOffsetTrackBar;
        NumericEdit horizontalOffsetUpDown;

        DiscPanel leftDiscPanel;
        DiscPanel rightDiscPanel;
        Button restoreButton;

        private float storedHorizontalOffset;

        public DiscControl()
            : base("Developer.GUI.DiscEditor.DiscControl.layout")
        {
            horizontalOffsetTrackBar = new MinMaxScroll((ScrollBar)window.findWidget("horizontalOffsetTrackBar"));
            horizontalOffsetTrackBar.Maximum = 10000;
            horizontalOffsetTrackBar.Minimum = -10000;
            horizontalOffsetTrackBar.ScrollChangePosition += new MyGUIEvent(horizontalOffsetTrackBar_ValueChanged);

            horizontalOffsetUpDown = new NumericEdit((EditBox)window.findWidget("horizontalOffsetUpDown"));
            horizontalOffsetUpDown.ValueChanged += new MyGUIEvent(horizontalOffsetUpDown_ValueChanged);
            horizontalOffsetUpDown.MaxValue = 1.0f;
            horizontalOffsetUpDown.MinValue = -1.0f;
            horizontalOffsetUpDown.Increment = 0.1f;

            Button resetButton = (Button)window.findWidget("ResetButton");
            resetButton.MouseButtonClick += resetButton_MouseButtonClick;

            restoreButton = (Button)window.findWidget("RestoreButton");
            restoreButton.MouseButtonClick += restoreButton_MouseButtonClick;

            this.Visible = true;
            leftDiscPanel = new DiscPanel(window, 6, 295, "Left Disc");
            leftDiscPanel.DiscName = "LeftTMJDisc";
            rightDiscPanel = new DiscPanel(window, 6, 35, "Right Disc");
            rightDiscPanel.DiscName = "RightTMJDisc";
            this.Visible = false;
        }

        void restoreButton_MouseButtonClick(Widget source, EventArgs e)
        {
            synchronizeHorizontalOffset(null, storedHorizontalOffset);
            leftDiscPanel.restore();
            rightDiscPanel.restore();
            restoreButton.Enabled = false;
        }

        void resetButton_MouseButtonClick(Widget source, EventArgs e)
        {
            storedHorizontalOffset = rightDiscPanel.Disc.HorizontalOffset.x;

            synchronizeHorizontalOffset(null, 0.0f);
            leftDiscPanel.reset();
            rightDiscPanel.reset();

            restoreButton.Enabled = true;
        }

        public void sceneLoaded(SimScene scene)
        {
            leftDiscPanel.sceneLoaded();
            rightDiscPanel.sceneLoaded();
            if (rightDiscPanel.Disc != null)
            {
                synchronizeHorizontalOffset(rightDiscPanel.Disc, rightDiscPanel.Disc.HorizontalOffset.x);
            }
        }

        public void sceneUnloading()
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
                    horizontalOffsetUpDown.FloatValue = offset;
                }
                allowSynchronization = true;
            }
        }

        void horizontalOffsetUpDown_ValueChanged(object sender, EventArgs e)
        {
            synchronizeHorizontalOffset(horizontalOffsetUpDown, horizontalOffsetUpDown.FloatValue);
        }

        void horizontalOffsetTrackBar_ValueChanged(object sender, EventArgs e)
        {
            synchronizeHorizontalOffset(horizontalOffsetTrackBar, horizontalOffsetTrackBar.Value / (float)horizontalOffsetTrackBar.Maximum);
        }
    }
}
