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
        private static Dictionary<String, FKLink> fkChainRoots = new Dictionary<string, FKLink>();

        public static void add(PoseableIdentifier poseable)
        {
            poseables.Add(poseable);
        }

        public static void remove(PoseableIdentifier poseable)
        {
            poseables.Remove(poseable);
        }

        public static void addFkChainRoot(String name, FKLink root)
        {
            fkChainRoots.Add(name, root);
        }

        public static void removeFkChainRoot(String name)
        {
            fkChainRoots.Remove(name);
        }

        public static bool tryGetFkChainRoot(String name, out FKLink root)
        {
            return fkChainRoots.TryGetValue(name, out root);
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
