using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class BoneManipulatorController
    {
        static NaturalSort<String> sorter = new NaturalSort<string>();
        private static SortedList<String, BoneManipulator> boneManipulators = new SortedList<string, BoneManipulator>(sorter);

        public static void addBoneManipulator(String name, BoneManipulator boneManipulator)
        {
            boneManipulators.Add(name, boneManipulator);
        }

        public static void removeBoneManipulator(String name)
        {
            boneManipulators.Remove(name);
        }

        public static IEnumerable<BoneManipulator> getManipulators()
        {
            return boneManipulators.Values;
        }
    }
}
