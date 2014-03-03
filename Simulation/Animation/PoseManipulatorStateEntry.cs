using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class PoseManipulatorStateEntry : AnimationManipulatorStateEntry
    {
        private String name;
        private float position;

        public PoseManipulatorStateEntry(String name, float position)
        {
            this.name = name;
            this.position = position;
        }

        public void blend(AnimationManipulatorStateEntry target, float percent)
        {
            PoseManipulatorStateEntry poseTarget = target as PoseManipulatorStateEntry;
            float start = position;
            float end = poseTarget.position;
            float delta = end - start;
            PoseManipulator bone = AnimationManipulatorController.getManipulator(name) as PoseManipulator;
            if (bone != null)
            {
                bone.Position = start + delta * percent;
            }
        }

        public AnimationManipulatorStateEntry clone()
        {
            return new PoseManipulatorStateEntry(name, position);
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public void changeSide(string oldName, string newName)
        {
            name.Replace(oldName, newName);
        }

        #region Saveable Members

        private const String NAME = "Name";
        private const String POSITION = "Position";

        protected PoseManipulatorStateEntry(LoadInfo info)
        {
            name = info.GetString(NAME);
            position = info.GetFloat(POSITION);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(POSITION, position);
        }

        #endregion
    }
}
