using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class manages the differences between the version 1 and version 2 scenes. It should be
    /// considered temporary and can be removed once we have converted completely to version 2 scenes.
    /// </summary>
    public static class SimulationVersionManager
    {
        public const int OriginalVersion = 1;
        public const int IkCatmullRomSplineRigidTeethVersion = 2;

        private static int loadedVersion = 0;

        public static event Action OnVersionChanged;

        /// <summary>
        /// Use this for scene changes that only need to look at version stuff.
        /// </summary>
        public static event Action<SimulationScene> OnSceneChanged;

        internal static void sceneChanged(SimulationScene scene)
        {
            if(loadedVersion != scene.Version)
            {
                loadedVersion = scene.Version;
                if(OnVersionChanged != null)
                {
                    OnVersionChanged.Invoke();
                }
            }
            if(OnSceneChanged != null)
            {
                OnSceneChanged.Invoke(scene);
            }
        }

        public static int LoadedVersion
        {
            get
            {
                return loadedVersion;
            }
        }
    }
}
