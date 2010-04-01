using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TongueController
    {
        static Tongue tongue;
        static bool collisionEnabled = true;

        static List<TongueCollisionSection> tongueCollisionSections = new List<TongueCollisionSection>();

        internal static void setTongue(Tongue tongue)
        {
            TongueController.tongue = tongue;
        }

        internal static void addCollisionSection(TongueCollisionSection section)
        {
            if (tongueCollisionSections.Count == 0)
            {
                collisionEnabled = section.CollisionEnabled;
            }
            tongueCollisionSections.Add(section);
        }

        internal static void removeTongueCollisionSection(TongueCollisionSection section)
        {
            tongueCollisionSections.Remove(section);
        }

        public static TongueMode TongueMode
        {
            get
            {
                return tongue.TongueMode;
            }
            set
            {
                tongue.TongueMode = value;
            }
        }

        public static float TonguePosition
        {
            get
            {
                return tongue.CurrentTonguePosition;
            }
            set
            {
                tongue.CurrentTonguePosition = value;
            }
        }

        public static bool TongueCollisionEnabled
        {
            get
            {
                return collisionEnabled;
            }
            set
            {
                collisionEnabled = value;
                foreach (TongueCollisionSection section in tongueCollisionSections)
                {
                    section.CollisionEnabled = collisionEnabled;
                }
            }
        }

        public static bool TongueDetected
        {
            get
            {
                return tongue != null;
            }
        }
    }
}
