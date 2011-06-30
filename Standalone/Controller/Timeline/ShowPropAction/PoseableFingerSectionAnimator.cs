using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Platform;

namespace Medical
{
    public class PoseableFingerSectionAnimator : Saveable
    {
        private Radian yaw;
        private Radian pitch;

        private Radian startYaw;
        private Radian startPitch;

        public PoseableFingerSectionAnimator()
        {
            
        }

        public void apply()
        {
            FingerSection.setOrientation(yaw, pitch);
        }

        public void getStartingValues()
        {
            startYaw = FingerSection.Yaw;
            startPitch = FingerSection.Pitch;
        }

        public void blend(float percent)
        {
            FingerSection.setOrientation(NumberFunctions.lerp(startYaw, yaw, percent), NumberFunctions.lerp(startPitch, pitch, percent));
        }

        public PoseableFingerSection FingerSection { get; set; }

        public Degree Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                yaw = value;
                if (FingerSection != null)
                {
                    FingerSection.Yaw = value;
                }
            }
        }

        public Degree Pitch
        {
            get
            {
                return pitch;
            }
            set
            {
                pitch = value;
                if (FingerSection != null)
                {
                    FingerSection.Pitch = value;
                }
            }
        }

        #region Saveable Members

        protected PoseableFingerSectionAnimator(LoadInfo info)
        {
            yaw = info.GetFloat("Yaw");
            pitch = info.GetFloat("Pitch");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Yaw", (float)yaw);
            info.AddValue("Pitch", (float)pitch);
        }

        #endregion
    }
}
