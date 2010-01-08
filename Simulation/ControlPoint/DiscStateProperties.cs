using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;

namespace Medical
{
    public class DiscStateProperties : Saveable
    {
        private Vector3 discOffset;
        private Vector3 rdaOffset;
        private float popLocation;
        private Vector3 horizontalOffset;
        private String discName;
        private bool locked;
        private bool displaceLateralPole;

        public DiscStateProperties(Disc disc)
        {
            discOffset = disc.DiscOffset;
            rdaOffset = disc.RDAOffset;
            popLocation = disc.PopLocation;
            discName = disc.Owner.Name;
            horizontalOffset = disc.HorizontalOffset;
            locked = disc.Locked;
            displaceLateralPole = disc.DisplaceLateralPole;
        }

        public DiscStateProperties(String discName)
        {
            this.discName = discName;
        }

        public void blend(DiscStateProperties target, float percent)
        {
            Disc disc = DiscController.getDisc(discName);
            disc.DiscOffset = this.discOffset.lerp(ref target.discOffset, ref percent);
            disc.RDAOffset = this.rdaOffset.lerp(ref target.rdaOffset, ref percent);
            disc.HorizontalOffset = this.horizontalOffset.lerp(ref target.horizontalOffset, ref percent);
            float delta = target.popLocation - this.popLocation;
            disc.PopLocation = this.popLocation + delta * percent;
            if (percent < 0.05f)
            {
                disc.DisplaceLateralPole = displaceLateralPole;
            }
            else
            {
                disc.DisplaceLateralPole = target.displaceLateralPole;
            }
            if (percent < 1.0f)
            {
                disc.Locked = locked;
            }
            else
            {
                disc.Locked = target.locked;
            }
        }

        public Vector3 DiscOffset
        {
            get
            {
                return discOffset;
            }
            set
            {
                discOffset = value;
            }
        }

        public Vector3 RDAOffset
        {
            get
            {
                return rdaOffset;
            }
            set
            {
                rdaOffset = value;
            }
        }

        public float PopLocation
        {
            get
            {
                return popLocation;
            }
            set
            {
                popLocation = value;
            }
        }

        public Vector3 HorizontalOffset
        {
            get
            {
                return horizontalOffset;
            }
            set
            {
                horizontalOffset = value;
            }
        }

        public String DiscName
        {
            get
            {
                return discName;
            }
            set
            {
                discName = value;
            }
        }

        public bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
            }
        }

        public bool DisplaceLateralPole
        {
            get
            {
                return displaceLateralPole;
            }
            set
            {
                displaceLateralPole = value;
            }
        }

        #region Saveable Members

        private const string DISC_OFFSET = "DiscOffset";
        private const string RDA_OFFSET = "RDAOffset";
        private const string POP_LOCATION = "PopLocation";
        private const string DISC_NAME = "DiscName";
        private const string HORIZONTAL_OFFSET = "HorizontalOffset";
        private const string LOCKED = "Locked";
        private const string DISPLACE_LATERAL_POLE = "DisplaceLateralPole";

        protected DiscStateProperties(LoadInfo info)
        {
            discOffset = info.GetVector3(DISC_OFFSET);
            rdaOffset = info.GetVector3(RDA_OFFSET);
            popLocation = info.GetFloat(POP_LOCATION);
            discName = info.GetString(DISC_NAME);
            horizontalOffset = info.GetVector3(HORIZONTAL_OFFSET);
            //Check for version up with locked
            if (info.hasValue(LOCKED))
            {
                locked = info.GetBoolean(LOCKED);
            }
            //Check for version with displace lateral pole
            if(info.hasValue(DISPLACE_LATERAL_POLE))
            {
                displaceLateralPole = info.GetBoolean(DISPLACE_LATERAL_POLE);
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(DISC_OFFSET, discOffset);
            info.AddValue(RDA_OFFSET, rdaOffset);
            info.AddValue(POP_LOCATION, popLocation);
            info.AddValue(DISC_NAME, discName);
            info.AddValue(HORIZONTAL_OFFSET, horizontalOffset);
            info.AddValue(LOCKED, locked);
            info.AddValue(DISPLACE_LATERAL_POLE, displaceLateralPole);
        }

        #endregion
    }
}
