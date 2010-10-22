using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Logging;

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
        private Quaternion rotation;

        public ToothState(String name, bool extracted, Vector3 offset, Quaternion rotation)
        {
            this.name = name;
            this.extracted = extracted;
            this.offset = offset;
            if (!offset.isNumber())
            {
                Log.Warning("Got a non numerical offset for tooth {0}", Name);
            }
            this.rotation = rotation;
            if (!rotation.isNumber())
            {
                Log.Warning("Got a non numerical rotation for tooth {0}", Name);
            }
        }

        internal ToothState(ToothState source)
        {
            this.name = source.name;
            this.extracted = source.extracted;
            this.offset = source.offset;
            this.rotation = source.rotation;
        }

        public void blend(ToothState end, float percent)
        {
            Tooth tooth = TeethController.getTooth(name);
            tooth.Extracted = this.Extracted;
            Vector3 offset = this.offset.lerp(ref end.offset, ref percent);
            if (offset.isNumber())
            {
                tooth.Offset = offset;
            }
            else
            {
                //Log.Debug("Got a non numerical offset for tooth {0} time {1}", Name, percent);
                tooth.Offset = end.offset;
            }
            Quaternion rotation = this.rotation.slerp(ref end.rotation, percent);
            if(rotation.isNumber())
            {
                tooth.Rotation = rotation;
            }
            else
            {
                //Log.Debug("Got a non numerical rotation for tooth {0} time {1}", Name, percent);
                tooth.Rotation = end.rotation;
            }
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

        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
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
        private const string ROTATION = "Rotation";

        protected ToothState(LoadInfo info)
        {
            name = info.GetString(NAME);
            extracted = info.GetBoolean(EXTRACTED);
            offset = info.GetVector3(OFFSET);
            rotation = info.GetQuaternion(ROTATION);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(EXTRACTED, extracted);
            info.AddValue(OFFSET, offset);
            info.AddValue(ROTATION, rotation);
        }

        #endregion
    }
}
