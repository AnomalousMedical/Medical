﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;

namespace Medical
{
    public class BoneScalarStateEntry : AnimationManipulatorStateEntry, Saveable
    {
        private String name;
        private Vector3 scale;

        public BoneScalarStateEntry(String name, Vector3 scale)
        {
            this.name = name;
            this.scale = scale;
        }

        public void blend(AnimationManipulatorStateEntry target, float percent)
        {
            BoneScalarStateEntry rotateTarget = target as BoneScalarStateEntry;
            BoneScalar bone = AnimationManipulatorController.getManipulator(name) as BoneScalar;
            if(bone != null)
            {
                bone.Scale = scale.lerp(ref rotateTarget.scale, ref percent);
            }
        }

        public AnimationManipulatorStateEntry clone()
        {
            return new BoneScalarStateEntry(name, scale);
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
        }

        #region Saveable Members

        private const String NAME = "Name";
        private const String SCALE = "Scale";

        protected BoneScalarStateEntry(LoadInfo info)
        {
            name = info.GetString(NAME);
            scale = info.GetVector3(SCALE);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(SCALE, scale);
        }

        #endregion
    }
}
