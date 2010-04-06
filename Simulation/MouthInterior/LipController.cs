using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class LipController
    {
        static bool collisionEnabled = true;

        static bool topLipsRigid = true;
        static List<LipSection> topLipCollisionSections = new List<LipSection>();

        static bool bottomLipsRigid = true;
        static List<LipSection> bottomLipCollisionSections = new List<LipSection>();

        internal static void addTopCollisionSection(LipSection section)
        {
            if (topLipCollisionSections.Count == 0)
            {
                collisionEnabled = section.CollisionEnabled;
                topLipsRigid = section.LipsRigid;
            }
            topLipCollisionSections.Add(section);
        }

        internal static void removeTopCollisionSection(LipSection section)
        {
            topLipCollisionSections.Remove(section);
        }

        internal static void addBottomCollisionSection(LipSection section)
        {
            if (bottomLipCollisionSections.Count == 0)
            {
                collisionEnabled = section.CollisionEnabled;
                bottomLipsRigid = section.LipsRigid;
            }
            bottomLipCollisionSections.Add(section);
        }

        internal static void removeBottomCollisionSection(LipSection section)
        {
            bottomLipCollisionSections.Remove(section);
        }

        public static void setOriginalPosition()
        {
            foreach (LipSection section in topLipCollisionSections)
            {
                section.setOriginalPosition();
            }
            foreach (LipSection section in bottomLipCollisionSections)
            {
                section.setOriginalPosition();
            }
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
                foreach (LipSection section in topLipCollisionSections)
                {
                    section.CollisionEnabled = collisionEnabled;
                }
                foreach (LipSection section in bottomLipCollisionSections)
                {
                    section.CollisionEnabled = collisionEnabled;
                }
            }
        }

        public static bool TopLipsRigid
        {
            get
            {
                return topLipsRigid;
            }
            set
            {
                topLipsRigid = value;
                foreach (LipSection section in topLipCollisionSections)
                {
                    section.LipsRigid = topLipsRigid;
                }
            }
        }

        public static bool BottomLipsRigid
        {
            get
            {
                return bottomLipsRigid;
            }
            set
            {
                bottomLipsRigid = value;
                foreach (LipSection section in bottomLipCollisionSections)
                {
                    section.LipsRigid = bottomLipsRigid;
                }
            }
        }
    }
}
