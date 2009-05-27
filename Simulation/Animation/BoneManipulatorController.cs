using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class BoneManipulatorController
    {
        private static Dictionary<String, BoneManipulator> boneManipulators = new Dictionary<string, BoneManipulator>();

        public static void addBoneManipulator(String name, BoneManipulator boneManipulator)
        {
            boneManipulators.Add(name, boneManipulator);
        }

        public static void removeBoneManipulator(String name)
        {
            boneManipulators.Remove(name);
        }

        public static Dictionary<String, BoneManipulator> getManipulators()
        {
            return boneManipulators;
        }
    }
}
