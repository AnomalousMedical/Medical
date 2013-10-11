using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class DiscPanel : Component
    {
        private Disc disc;
        private bool allowSynchronization = true;

        MinMaxScroll discPopSlider;
        NumericEdit discPopUpDown;
        
        MinMaxScroll rdaOffsetSlider;
        NumericEdit rdaOffsetUpDown;
        
        MinMaxScroll discOffsetSlider;
        NumericEdit discOffsetUpDown;
        
        MinMaxScroll horizontalClockFaceSlider;
        NumericEdit horizontalClockFaceUpDown;

        MinMaxScroll verticalClockFaceSlider;
        NumericEdit verticalClockFaceUpDown;

        CheckButton discLockedCheck;
        CheckButton lateralPoleDisplacementCheck;

        float storedPop;
        float storedDiscOffset;
        float storedRDAOffset;
        bool storedLocked;
        bool storedLateralPoleDisplaced;
        Vector3 storedClockFaceOffset;

        public DiscPanel(Widget parent, int x, int y, String name)
            : base("Developer.GUI.DiscEditor.DiscPanel.layout")
        {
            widget.attachToWidget(parent);
            widget.setPosition(x, y);

            TextBox sideLabel = (TextBox)widget.findWidget("SideLabel");
            sideLabel.Caption = name;

            discPopSlider = new MinMaxScroll((ScrollBar)widget.findWidget("discPopSlider"));
            discPopSlider.Minimum = 0;
            discPopSlider.Maximum = 10000;
            discPopUpDown = new NumericEdit((EditBox)widget.findWidget("discPopUpDown"));
            discPopUpDown.MinValue = 0.0f;
            discPopUpDown.MaxValue = 1.0f;
            discPopUpDown.Increment = 0.01f;

            rdaOffsetSlider = new MinMaxScroll((ScrollBar)widget.findWidget("rdaOffsetSlider"));
            rdaOffsetSlider.Minimum = 0;
            rdaOffsetSlider.Maximum = 10000;
            rdaOffsetUpDown = new NumericEdit((EditBox)widget.findWidget("rdaOffsetUpDown"));
            rdaOffsetUpDown.MinValue = -1.0f;
            rdaOffsetUpDown.MaxValue = 0.0f;
            rdaOffsetUpDown.Increment = 0.1f;

            discOffsetSlider = new MinMaxScroll((ScrollBar)widget.findWidget("discOffsetSlider"));
            discOffsetSlider.Minimum = 0;
            discOffsetSlider.Maximum = 10000;
            discOffsetUpDown = new NumericEdit((EditBox)widget.findWidget("discOffsetUpDown"));
            discOffsetUpDown.MinValue = -1.0f;
            discOffsetUpDown.MaxValue = 0.0f;
            discOffsetUpDown.Increment = 0.1f;

            horizontalClockFaceSlider = new MinMaxScroll((ScrollBar)widget.findWidget("horizontalClockFaceSlider"));
            horizontalClockFaceSlider.Minimum = -10000;
            horizontalClockFaceSlider.Maximum = 10000;
            horizontalClockFaceUpDown = new NumericEdit((EditBox)widget.findWidget("horizontalClockFaceUpDown"));
            horizontalClockFaceUpDown.MinValue = -1.0f;
            horizontalClockFaceUpDown.MaxValue = 1.0f;
            horizontalClockFaceUpDown.Increment = 0.1f;

            verticalClockFaceSlider = new MinMaxScroll((ScrollBar)widget.findWidget("verticalClockFaceSlider"));
            verticalClockFaceSlider.Minimum = 0;
            verticalClockFaceSlider.Maximum = 10000;
            verticalClockFaceUpDown = new NumericEdit((EditBox)widget.findWidget("verticalClockFaceUpDown"));
            verticalClockFaceUpDown.MinValue = -1.0f;
            verticalClockFaceUpDown.MaxValue = 0.0f;
            verticalClockFaceUpDown.Increment = 0.1f;

            discLockedCheck = new CheckButton((Button)widget.findWidget("discLockedCheck"));
            lateralPoleDisplacementCheck = new CheckButton((Button)widget.findWidget("lateralPoleDisplacementCheck"));
            lateralPoleDisplacementCheck.CheckedChanged += lateralPoleDisplacementCheck_CheckedChanged;

            discPopSlider.ScrollChangePosition += new MyGUIEvent(discPopSlider_ValueChanged);
            discPopUpDown.ValueChanged += new MyGUIEvent(discPopUpDown_ValueChanged);
            rdaOffsetSlider.ScrollChangePosition += new MyGUIEvent(rdaOffsetSlider_ValueChanged);
            rdaOffsetUpDown.ValueChanged += new MyGUIEvent(rdaOffsetUpDown_ValueChanged);
            discOffsetSlider.ScrollChangePosition += new MyGUIEvent(discOffsetSlider_ValueChanged);
            discOffsetUpDown.ValueChanged += new MyGUIEvent(discOffsetUpDown_ValueChanged);
            discLockedCheck.CheckedChanged += new MyGUIEvent(discLockedCheck_CheckedChanged);
            horizontalClockFaceSlider.ScrollChangePosition += new MyGUIEvent(clockFaceSlider_ValueChanged);
            horizontalClockFaceUpDown.ValueChanged += new MyGUIEvent(clockFaceUpDown_ValueChanged);
            verticalClockFaceSlider.ScrollChangePosition += new MyGUIEvent(clockFaceSlider_ValueChanged);
            verticalClockFaceUpDown.ValueChanged += new MyGUIEvent(clockFaceUpDown_ValueChanged);
        }

        public void sceneLoaded()
        {
            if (DiscName != null)
            {
                disc = DiscController.getDisc(DiscName);
            }
            widget.Enabled = disc != null;
            if (widget.Enabled)
            {
                synchronizePop(disc, disc.PopLocation);
                synchronizeDiscOffset(disc, disc.DiscOffset.y);
                synchronizeRDAOffset(disc, disc.RDAOffset.y);
                synchronizeLocked(disc, disc.Locked);
                synchronizeLateralPoleDisplacement(disc, disc.DisplaceLateralPole);
                synchronizeClockFaceOffset(disc, disc.ClockFaceOffset);
            }
        }

        public void sceneUnloaded()
        {
            disc = null;
        }

        public void reset()
        {
            storedPop = disc.PopLocation;
            storedDiscOffset = disc.DiscOffset.y;
            storedRDAOffset = disc.RDAOffset.y;
            storedLocked = disc.Locked;
            storedLateralPoleDisplaced = disc.DisplaceLateralPole;
            storedClockFaceOffset = disc.ClockFaceOffset;

            synchronizePop(null, disc.NormalPopLocation);
            synchronizeDiscOffset(null, disc.NormalDiscOffset.y);
            synchronizeRDAOffset(null, disc.NormalRDAOffset.y);
            synchronizeLocked(null, false);
            synchronizeLateralPoleDisplacement(null, false);
            synchronizeClockFaceOffset(null, disc.NormalClockFaceOffset);
        }

        public void restore()
        {
            synchronizePop(null, storedPop);
            synchronizeDiscOffset(null, storedDiscOffset);
            synchronizeRDAOffset(null, storedRDAOffset);
            synchronizeLocked(null, storedLocked);
            synchronizeLateralPoleDisplacement(null, storedLateralPoleDisplaced);
            synchronizeClockFaceOffset(null, storedClockFaceOffset);
        }

        public String DiscName { get; set; }

        public Disc Disc
        {
            get
            {
                return disc;
            }
        }

        //Synchronize Pop Location
        void synchronizePop(object sender, float popLocation)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (sender != disc)
                {
                    disc.PopLocation = popLocation;
                }
                if (sender != discPopSlider)
                {
                    discPopSlider.Value = (int)(popLocation * discPopSlider.Maximum);
                }
                if (sender != discPopUpDown)
                {
                    discPopUpDown.FloatValue = popLocation;
                }
                allowSynchronization = true;
            }
        }

        void discPopUpDown_ValueChanged(object sender, EventArgs e)
        {
            synchronizePop(discPopUpDown, discPopUpDown.FloatValue);
        }

        void discPopSlider_ValueChanged(object sender, EventArgs e)
        {
            synchronizePop(discPopSlider, discPopSlider.Value / (float)discPopSlider.Maximum);
        }

        //Synchronize RDA Offset Location
        void synchronizeRDAOffset(object sender, float rdaOffset)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (sender != disc)
                {
                    disc.RDAOffset = new Vector3(0.0f, rdaOffset, 0.0f);
                }
                if (sender != rdaOffsetSlider)
                {
                    rdaOffsetSlider.Value = (int)(rdaOffset * -rdaOffsetSlider.Maximum);
                }
                if (sender != rdaOffsetUpDown)
                {
                    rdaOffsetUpDown.FloatValue = rdaOffset;
                }
                allowSynchronization = true;
            }
        }

        void rdaOffsetUpDown_ValueChanged(object sender, EventArgs e)
        {
            synchronizeRDAOffset(rdaOffsetUpDown, rdaOffsetUpDown.FloatValue);
        }

        void rdaOffsetSlider_ValueChanged(object sender, EventArgs e)
        {
            synchronizeRDAOffset(rdaOffsetSlider, rdaOffsetSlider.Value / -(float)rdaOffsetSlider.Maximum);
        }

        //Synchronize RDA Offset Location
        void synchronizeDiscOffset(object sender, float discOffset)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (sender != disc)
                {
                    disc.DiscOffset = new Vector3(0.0f, discOffset, 0.0f);
                }
                if (sender != discOffsetSlider)
                {
                    discOffsetSlider.Value = (int)(discOffset * -discOffsetSlider.Maximum);
                }
                if (sender != discOffsetUpDown)
                {
                    discOffsetUpDown.FloatValue = discOffset;
                }
                allowSynchronization = true;
            }
        }

        void discOffsetUpDown_ValueChanged(object sender, EventArgs e)
        {
            synchronizeDiscOffset(discOffsetUpDown, discOffsetUpDown.FloatValue);
        }

        void discOffsetSlider_ValueChanged(object sender, EventArgs e)
        {
            synchronizeDiscOffset(discOffsetSlider, discOffsetSlider.Value / -(float)discOffsetSlider.Maximum);
        }

        //Synchronize locked
        void synchronizeLocked(object sender, bool locked)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (sender != disc)
                {
                    disc.Locked = locked;
                }
                if (sender != discLockedCheck)
                {
                    discLockedCheck.Checked = locked;
                }
                allowSynchronization = true;
            }
        }

        void discLockedCheck_CheckedChanged(object sender, EventArgs e)
        {
            synchronizeLocked(discLockedCheck, discLockedCheck.Checked);
        }

        //Synchronize lateral pole displacement
        void synchronizeLateralPoleDisplacement(object sender, bool displace)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (sender != disc)
                {
                    disc.DisplaceLateralPole = displace;
                }
                if (sender != lateralPoleDisplacementCheck)
                {
                    lateralPoleDisplacementCheck.Checked = displace;
                }
                allowSynchronization = true;
            }
        }

        private void lateralPoleDisplacementCheck_CheckedChanged(object sender, EventArgs e)
        {
            synchronizeLateralPoleDisplacement(lateralPoleDisplacementCheck, lateralPoleDisplacementCheck.Checked);
        }

        //Synchronize clock face position
        void synchronizeClockFaceOffset(object sender, Vector3 offset)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (sender != disc)
                {
                    disc.ClockFaceOffset = offset;
                }
                if (sender != horizontalClockFaceSlider && sender != verticalClockFaceSlider)
                {
                    horizontalClockFaceSlider.Value = (int)(offset.z * horizontalClockFaceSlider.Maximum);
                    verticalClockFaceSlider.Value = (int)(offset.y * -verticalClockFaceSlider.Maximum);
                }
                if (sender != horizontalClockFaceUpDown && sender != verticalClockFaceUpDown)
                {
                    horizontalClockFaceUpDown.FloatValue = offset.z;
                    verticalClockFaceUpDown.FloatValue = offset.y;
                }
                allowSynchronization = true;
            }
        }

        void clockFaceUpDown_ValueChanged(object sender, EventArgs e)
        {
            Vector3 offset = new Vector3(0.0f, (float)verticalClockFaceUpDown.FloatValue, (float)horizontalClockFaceUpDown.FloatValue);
            synchronizeClockFaceOffset(sender, offset);
        }

        void clockFaceSlider_ValueChanged(object sender, EventArgs e)
        {
            Vector3 offset = new Vector3(0.0f,
                (float)verticalClockFaceSlider.Value / -(float)verticalClockFaceSlider.Maximum,
                (float)horizontalClockFaceSlider.Value / (float)horizontalClockFaceSlider.Maximum);
            synchronizeClockFaceOffset(sender, offset);
        }
    }
}
