using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

namespace Medical
{
    public class AnimationManipulatorController
    {
        private static Dictionary<String, AnimationManipulator> manipulators = new Dictionary<String, AnimationManipulator>();

        public static void addAnimationManipulator(AnimationManipulator boneManipulator)
        {
            String name = boneManipulator.UIName;
            if (name != null && !manipulators.ContainsKey(name))
            {
                manipulators.Add(name, boneManipulator);
            }
            else
            {
                Log.Default.sendMessage("Added duplicate BoneManipulator named {0}. Duplicate ignored.", LogLevel.Warning, "Medical", name);
            }
        }

        public static void removeAnimationManipulator(AnimationManipulator boneManipulator)
        {
            String name = boneManipulator.UIName;
            if (name != null && manipulators.ContainsKey(name))
            {
                if (manipulators[name] == boneManipulator)
                {
                    manipulators.Remove(name);
                }
                else
                {
                    Log.Default.sendMessage("Attempted to remove BoneManipulator named {0} that does not match the manipulator in the controller. No changes made.", LogLevel.Warning, "Medical", name);
                }
            }
            else
            {
                Log.Default.sendMessage("Attempted to remove BoneManipulator named {0}. No changes made.", LogLevel.Warning, "Medical", name);
            }
        }

        public static AnimationManipulator getManipulator(String name)
        {
            AnimationManipulator ret;
            manipulators.TryGetValue(name, out ret);
            return ret;
        }

        public static AnimationManipulatorState createAnimationManipulatorState()
        {
            AnimationManipulatorState state = new AnimationManipulatorState();
            foreach (AnimationManipulator manipulator in manipulators.Values)
            {
                state.addPosition(manipulator.createStateEntry());
            }
            return state;
        }
    }
}
