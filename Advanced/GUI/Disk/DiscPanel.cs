using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Medical.GUI
{
    public partial class DiscPanel : UserControl
    {
        private Disc disc;
        private bool allowSynchronization = true;

        public DiscPanel()
        {
            InitializeComponent();
            discPopSlider.ValueChanged += new EventHandler(discPopSlider_ValueChanged);
            discPopUpDown.ValueChanged += new EventHandler(discPopUpDown_ValueChanged);
            rdaOffsetSlider.ValueChanged += new EventHandler(rdaOffsetSlider_ValueChanged);
            rdaOffsetUpDown.ValueChanged += new EventHandler(rdaOffsetUpDown_ValueChanged);
            discOffsetSlider.ValueChanged += new EventHandler(discOffsetSlider_ValueChanged);
            discOffsetUpDown.ValueChanged += new EventHandler(discOffsetUpDown_ValueChanged);
            discLockedCheck.CheckedChanged += new EventHandler(discLockedCheck_CheckedChanged);
            //lateralPoleRotSlider.ValueChanged += new EventHandler(lateralPoleRotSlider_ValueChanged);
            //lateralPoleRotUpDown.ValueChanged += new EventHandler(lateralPoleRotUpDown_ValueChanged);
            horizontalClockFaceSlider.ValueChanged += new EventHandler(clockFaceSlider_ValueChanged);
            horizontalClockFaceUpDown.ValueChanged += new EventHandler(clockFaceUpDown_ValueChanged);
            verticalClockFaceSlider.ValueChanged += new EventHandler(clockFaceSlider_ValueChanged);
            verticalClockFaceUpDown.ValueChanged += new EventHandler(clockFaceUpDown_ValueChanged);
        }

        public void sceneLoaded()
        {
            if (DiscName != null)
            {
                disc = DiscController.getDisc(DiscName);
            }
            this.Enabled = disc != null;
            if (Enabled)
            {
                synchronizePop(disc, disc.PopLocation);
                synchronizeDiscOffset(disc, disc.DiscOffset.y);
                synchronizeRDAOffset(disc, disc.RDAOffset.y);
                synchronizeLocked(disc, disc.Locked);
                synchronizeLateralPoleDisplacement(disc, disc.DisplaceLateralPole);
            }
        }

        public void sceneUnloaded()
        {
            disc = null;
        }

        public void reset()
        {
            synchronizePop(null, disc.NormalPopLocation);
            synchronizeDiscOffset(null, disc.NormalDiscOffset.y);
            synchronizeRDAOffset(null, disc.NormalRDAOffset.y);
            synchronizeLocked(null, false);
            synchronizeLateralPoleDisplacement(null, false);
            synchronizeClockFaceOffset(null, disc.NormalClockFaceOffset);
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
                    discPopUpDown.Value = (decimal)popLocation;
                }
                allowSynchronization = true;
            }
        }

        void discPopUpDown_ValueChanged(object sender, EventArgs e)
        {
            synchronizePop(discPopUpDown, (float)discPopUpDown.Value);
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
                    rdaOffsetUpDown.Value = (decimal)rdaOffset;
                }
                allowSynchronization = true;
            }
        }

        void rdaOffsetUpDown_ValueChanged(object sender, EventArgs e)
        {
            synchronizeRDAOffset(rdaOffsetUpDown, (float)rdaOffsetUpDown.Value);
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
                    discOffsetUpDown.Value = (decimal)discOffset;
                }
                allowSynchronization = true;
            }
        }

        void discOffsetUpDown_ValueChanged(object sender, EventArgs e)
        {
            synchronizeDiscOffset(discOffsetUpDown, (float)discOffsetUpDown.Value);
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
                    horizontalClockFaceUpDown.Value = (decimal)offset.z;
                    verticalClockFaceUpDown.Value = (decimal)offset.y;
                }
                allowSynchronization = true;
            }
        }

        void clockFaceUpDown_ValueChanged(object sender, EventArgs e)
        {
            Vector3 offset = new Vector3(0.0f, (float)verticalClockFaceUpDown.Value, (float)horizontalClockFaceUpDown.Value);
            synchronizeClockFaceOffset(sender, offset);
        }

        void clockFaceSlider_ValueChanged(object sender, EventArgs e)
        {
            Vector3 offset = new Vector3(0.0f, 
                (float)verticalClockFaceSlider.Value / -(float)verticalClockFaceSlider.Maximum, 
                (float)horizontalClockFaceSlider.Value / (float)horizontalClockFaceSlider.Maximum);
            synchronizeClockFaceOffset(sender, offset);
        }

        //Synchronize Lateral Pole Rotation
        //void synchronizeLateralPoleRotation(object sender, float position)
        //{
        //    if (allowSynchronization)
        //    {
        //        allowSynchronization = false;
        //        if (sender != disc)
        //        {
        //            disc.LateralPoleRotation = position;
        //        }
        //        if (sender != lateralPoleRotSlider)
        //        {
        //            lateralPoleRotSlider.Value = (int)(position * lateralPoleRotSlider.Maximum);
        //        }
        //        if (sender != lateralPoleRotUpDown)
        //        {
        //            lateralPoleRotUpDown.Value = (decimal)position;
        //        }
        //        allowSynchronization = true;
        //    }
        //}

        //void lateralPoleRotUpDown_ValueChanged(object sender, EventArgs e)
        //{
        //    synchronizeLateralPoleRotation(lateralPoleRotUpDown, (float)lateralPoleRotUpDown.Value);
        //}

        //void lateralPoleRotSlider_ValueChanged(object sender, EventArgs e)
        //{
        //    synchronizeLateralPoleRotation(lateralPoleRotSlider, lateralPoleRotSlider.Value / (float)lateralPoleRotSlider.Maximum);
        //}
    }
}
