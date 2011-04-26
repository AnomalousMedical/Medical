using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class AnatomyManager
    {
        private AnatomyManager() { }

        private static List<AnatomyIdentifier> anatomyList = new List<AnatomyIdentifier>();

        public static void addAnatomy(AnatomyIdentifier anatomy)
        {
            anatomyList.Add(anatomy);
        }

        public static void removeAnatomy(AnatomyIdentifier anatomy)
        {
            anatomyList.Remove(anatomy);
        }
    }
}
