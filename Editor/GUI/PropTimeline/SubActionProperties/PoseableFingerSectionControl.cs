﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PoseableFingerSectionControl
    {
        private PoseableFingerSectionAnimator fingerSection;

        private NumericEdit yawEdit;
        private NumericEdit pitchEdit;

        public PoseableFingerSectionControl(Edit pitchEdit)
            :this(null, pitchEdit)
        {

        }

        public PoseableFingerSectionControl(Edit yawEdit, Edit pitchEdit)
        {
            if (yawEdit != null)
            {
                this.yawEdit = new NumericEdit(yawEdit);
                this.yawEdit.ValueChanged += new MyGUIEvent(yawEdit_ValueChanged);
                this.yawEdit.MinValue = -360;
                this.yawEdit.MaxValue = 360;
                this.yawEdit.Increment = 1;
            }

            this.pitchEdit = new NumericEdit(pitchEdit);
            this.pitchEdit.ValueChanged += new MyGUIEvent(pitchEdit_ValueChanged);
            this.pitchEdit.MinValue = -360;
            this.pitchEdit.MaxValue = 360;
            this.pitchEdit.Increment = 1;
        }

        public PoseableFingerSectionAnimator FingerSection
        {
            get
            {
                return fingerSection;
            }
            set
            {
                this.fingerSection = value;
                if (fingerSection != null)
                {
                    if (yawEdit != null)
                    {
                        yawEdit.FloatValue = fingerSection.Yaw;
                    }
                    pitchEdit.FloatValue = fingerSection.Pitch;
                }
            }
        }

        void pitchEdit_ValueChanged(Widget source, EventArgs e)
        {
            if (fingerSection != null)
            {
                fingerSection.Pitch = pitchEdit.FloatValue;
            }
        }

        void yawEdit_ValueChanged(Widget source, EventArgs e)
        {
            if (fingerSection != null)
            {
                fingerSection.Yaw = yawEdit.FloatValue;
            }
        }
    }
}
