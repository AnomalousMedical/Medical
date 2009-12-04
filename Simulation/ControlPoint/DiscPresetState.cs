using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;

namespace Medical
{
    public class DiscPresetState : PresetState
    {
        private Vector3 discOffset;
        private Vector3 rdaOffset;
        private float popLocation;
        private Vector3 horizontalOffset;
        private String discName;
        private bool locked;

        public DiscPresetState(String discName, String name, String category, String imageName)
            :base(name, category, imageName)
        {
            this.discName = discName;
        }

        public void captureFromState(DiscStateProperties state)
        {
            this.DiscOffset = state.DiscOffset;
            this.HorizontalOffset = state.HorizontalOffset;
            this.Locked = state.Locked;
            this.PopLocation = state.PopLocation;
            this.RdaOffset = state.RDAOffset;
        }

        public override void applyToState(MedicalState state)
        {
            DiscStateProperties disc = new DiscStateProperties(discName);
            disc.DiscOffset = discOffset;
            disc.HorizontalOffset = horizontalOffset;
            disc.Locked = locked;
            disc.PopLocation = popLocation;
            disc.RDAOffset = rdaOffset;
            state.Disc.addPosition(disc);
        }

        public void changeSide(String oldName, String newName)
        {
            discName = discName.Replace(oldName, newName);
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

        public Vector3 RdaOffset
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

        #region Saveable Members

        private const String DISC_OFFSET = "discOffset";
        private const String RDA_OFFSET = "rdaOffset";
        private const String POP_LOCATION = "popLocation";
        private const String HORIZONTAL_OFFSET = "horizontalOffset";
        private const String DISC_NAME = "discName";
        private const String LOCKED = "locked";

        protected DiscPresetState(LoadInfo info)
            :base(info)
        {
            discOffset = info.GetVector3(DISC_OFFSET);
            rdaOffset = info.GetVector3(RDA_OFFSET);
            popLocation = info.GetFloat(POP_LOCATION);
            horizontalOffset = info.GetVector3(HORIZONTAL_OFFSET);
            discName = info.GetString(DISC_NAME);
            locked = info.GetBoolean(LOCKED);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(DISC_OFFSET, discOffset);
            info.AddValue(RDA_OFFSET, rdaOffset);
            info.AddValue(POP_LOCATION, popLocation);
            info.AddValue(HORIZONTAL_OFFSET, horizontalOffset);
            info.AddValue(DISC_NAME, discName);
            info.AddValue(LOCKED, locked);
        }

        #endregion Saveable Members
    }
}
