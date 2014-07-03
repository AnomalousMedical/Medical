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
        private static Dictionary<String, FKRoot> fkChainRoots = new Dictionary<string, FKRoot>();

        internal static void add(PoseableIdentifier poseable)
        {
            poseables.Add(poseable);
        }

        internal static void remove(PoseableIdentifier poseable)
        {
            poseables.Remove(poseable);
        }

        internal static void addFkChainRoot(FKRoot root)
        {
            fkChainRoots.Add(root.RootName, root);
        }

        internal static void removeFkChainRoot(FKRoot root)
        {
            fkChainRoots.Remove(root.RootName);
        }

        /// <summary>
        /// Try to get an FKRoot specified by name. Will return true and fill out root if 
        /// the root is found. Will return false if the root is not found.
        /// </summary>
        /// <param name="name">The name of the root to find.</param>
        /// <param name="root">The variable to put the value of root into.</param>
        /// <returns>True if the root is found, false if it is not.</returns>
        public static bool tryGetFkChainRoot(String name, out FKRoot root)
        {
            return fkChainRoots.TryGetValue(name, out root);
        }

        /// <summary>
        /// Find the poseables that collide with ray.
        /// </summary>
        /// <param name="ray">The ray to search with.</param>
        /// <returns>A list of poseables that collide with ray sorted by their distance along ray from nearest to farthest.</returns>
        public static SortedPoseableRaycastResults findPoseable(Ray3 ray)
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
