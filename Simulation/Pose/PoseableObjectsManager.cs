using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public static class PoseableObjectsManager
    {
        private static List<PoseableIdentifier> poseables = new List<PoseableIdentifier>();

        public static void add(PoseableIdentifier poseable)
        {
            poseables.Add(poseable);
        }

        public static void remove(PoseableIdentifier poseable)
        {
            poseables.Remove(poseable);
        }

        public static SortedPoseableRaycastResults findAnatomy(Ray3 ray)
        {
            float distance = 0.0f;
            SortedPoseableRaycastResults results = new SortedPoseableRaycastResults();
            foreach (PoseableIdentifier anatomy in poseables)
            {
                if (anatomy.checkCollision(ray, ref distance))
                {
                    results.add(new PoseableRaycastResult(anatomy, distance));
                }
            }
            results.sort();
            return results;
        }
    }
}
