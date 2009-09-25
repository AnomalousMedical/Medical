using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public interface BoneManipulatorStateEntry : Saveable
    {
        void blend(BoneManipulatorStateEntry target, float percent);

        BoneManipulatorStateEntry clone();

        String Name { get; }

        void changeSide(String oldName, String newName);
    }
}
