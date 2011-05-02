using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

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

        public static IEnumerable<AnatomyIdentifier> AnatomyList
        {
            get
            {
                return anatomyList;
            }
        }

        public static List<AnatomyIdentifier> findAnatomy(Ray3 ray)
        {
            float distance = 0.0f;
            List<AnatomyIdentifier> matches = new List<AnatomyIdentifier>();
            foreach (AnatomyIdentifier anatomy in anatomyList)
            {
                if (anatomy.checkCollision(ray, ref distance))
                {
                    matches.Add(anatomy);
                    Logging.Log.Debug("Match distance {0} {1}", anatomy.AnatomicalName, distance);
                }
            }
            return matches;
        }
    }
}
