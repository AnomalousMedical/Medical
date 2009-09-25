using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;

namespace Medical
{
    public class BoneRotatorStateEntry : BoneManipulatorStateEntry, Saveable
    {
        private String name;
        private Quaternion rotation;

        public BoneRotatorStateEntry(String name, Quaternion rotation)
        {
            this.name = name;
            this.rotation = rotation;
        }

        public void blend(BoneManipulatorStateEntry target, float percent)
        {
            BoneRotatorStateEntry rotateTarget = target as BoneRotatorStateEntry;
            ((BoneRotator)BoneManipulatorController.getManipulator(name)).Rotation = rotation.slerp(ref rotateTarget.rotation, percent);
        }

        public BoneManipulatorStateEntry clone()
        {
            return new BoneRotatorStateEntry(name, rotation);
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public void changeSide(String oldName, String newName)
        {
            name = name.Replace(oldName, newName);
            Vector3 euler = rotation.getEuler();
            rotation.setEuler(euler.x, -euler.y, -euler.z);
        }

        #region Saveable Members

        private const String NAME = "Name";
        private const String ROTATION = "Rotation";

        protected BoneRotatorStateEntry(LoadInfo info)
        {
            name = info.GetString(NAME);
            rotation = info.GetQuaternion(ROTATION);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(ROTATION, rotation);
        }

        #endregion
    }
}
