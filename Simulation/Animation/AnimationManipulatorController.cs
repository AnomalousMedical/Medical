using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

namespace Medical
{
    /// <summary>
    /// This class has been made internal as it is no longer the preferred
    /// mechanism for looking up animation manipultors. However, to retain
    /// compatablility with any created data files it must still exist because
    /// these files will store the animation manipulators they deal with as
    /// UIName instead of the element name. This should be fixed at some point
    /// and this class elimiated, but the distortion states will somehow have to
    /// be updated as well.
    /// </summary>
    internal class AnimationManipulatorController
    {
        private static Dictionary<String, AnimationManipulator> manipulators = new Dictionary<String, AnimationManipulator>();

        internal static void addAnimationManipulator(AnimationManipulator boneManipulator)
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

        internal static void removeAnimationManipulator(AnimationManipulator boneManipulator)
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

        internal static AnimationManipulator getManipulator(String name)
        {
            AnimationManipulator ret;
            manipulators.TryGetValue(name, out ret);
            return ret;
        }

        //public static AnimationManipulatorState createAnimationManipulatorState()
        //{
        //    AnimationManipulatorState state = new AnimationManipulatorState();
        //    foreach (AnimationManipulator manipulator in manipulators.Values)
        //    {
        //        state.addPosition(manipulator.createStateEntry());
        //    }
        //    return state;
        //}
    }
}
