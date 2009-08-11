using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;

namespace Medical
{
    /// <summary>
    /// The state for an individual tooth.
    /// </summary>
    public class ToothState : Saveable
    {
        private bool extracted;
        private Vector3 offset;
        private String name;

        public ToothState(String name, bool extracted, Vector3 offset)
        {
            this.name = name;
            this.extracted = extracted;
            this.offset = offset;
        }

        public void blend(ToothState end, float percent)
        {
            Tooth tooth = TeethController.getTooth(name);
            tooth.Extracted = this.Extracted;
            tooth.Offset = this.offset.lerp(ref end.offset, ref percent);
        }

        public bool Extracted
        {
            get
            {
                return extracted;
            }
            set
            {
                extracted = value;
            }
        }

        public Vector3 Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        #region Saveable Members

        private const string EXTRACTED = "Extracted";
        private const string OFFSET = "Offset";
        private const string NAME = "Name";

        protected ToothState(LoadInfo info)
        {
            name = info.GetString(NAME);
            extracted = info.GetBoolean(EXTRACTED);
            offset = info.GetVector3(OFFSET);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(EXTRACTED, extracted);
            info.AddValue(OFFSET, offset);
        }

        #endregion
    }
}
