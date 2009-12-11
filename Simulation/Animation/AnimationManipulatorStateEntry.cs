using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public interface AnimationManipulatorStateEntry : Saveable
    {
        void blend(AnimationManipulatorStateEntry target, float percent);

        AnimationManipulatorStateEntry clone();

        String Name { get; }

        void changeSide(String oldName, String newName);
    }
}
