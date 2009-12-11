using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

namespace Medical
{
    public class BoneManipulatorController
    {
        private static Dictionary<String, AnimationManipulator> boneManipulators = new Dictionary<String, AnimationManipulator>();

        public static void addBoneManipulator(AnimationManipulator boneManipulator)
        {
            String name = boneManipulator.UIName;
            if (name != null && !boneManipulators.ContainsKey(name))
            {
                boneManipulators.Add(name, boneManipulator);
            }
            else
            {
                Log.Default.sendMessage("Added duplicate BoneManipulator named {0}. Duplicate ignored.", LogLevel.Warning, "Medical", name);
            }
        }

        public static void removeBoneManipulator(AnimationManipulator boneManipulator)
        {
            String name = boneManipulator.UIName;
            if (name != null && boneManipulators.ContainsKey(name))
            {
                if (boneManipulators[name] == boneManipulator)
                {
                    boneManipulators.Remove(name);
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
            boneManipulators.TryGetValue(name, out ret);
            return ret;
        }

        public static BoneManipulatorState createBoneManipulatorState()
        {
            BoneManipulatorState state = new BoneManipulatorState();
            foreach (BoneManipulator manipulator in boneManipulators.Values)
            {
                state.addPosition(manipulator.createStateEntry());
            }
            return state;
        }
    }
}
