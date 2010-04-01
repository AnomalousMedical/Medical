using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class LipController
    {
        static bool collisionEnabled = true;
        static bool lipsRigid = true;

        static List<LipSection> lipCollisionSections = new List<LipSection>();

        internal static void addCollisionSection(LipSection section)
        {
            if (lipCollisionSections.Count == 0)
            {
                collisionEnabled = section.CollisionEnabled;
                lipsRigid = section.LipsRigid;
            }
            lipCollisionSections.Add(section);
        }

        internal static void removeTongueCollisionSection(LipSection section)
        {
            lipCollisionSections.Remove(section);
        }

        public static bool LipCollisionEnabled
        {
            get
            {
                return collisionEnabled;
            }
            set
            {
                collisionEnabled = value;
                foreach (LipSection section in lipCollisionSections)
                {
                    section.CollisionEnabled = collisionEnabled;
                }
            }
        }

        public static bool LipsRigid
        {
            get
            {
                return lipsRigid;
            }
            set
            {
                lipsRigid = value;
                foreach (LipSection section in lipCollisionSections)
                {
                    section.LipsRigid = lipsRigid;
                }
            }
        }
    }
}
