using Engine;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public static class LiveThumbnailUpdater
    {
        private static bool render = true;
        private static int numImagesToUpdate = 1;
        private static double secondsToSleep = 1;
        private static List<TextureSceneView> activeImages = new List<TextureSceneView>();

        static LiveThumbnailUpdater()
        {
            Coroutine.Start(renderUpdates());
        }

        public static void Shutdown()
        {
            render = false;
        }

        public static void addSceneView(TextureSceneView sceneView)
        {
            activeImages.Add(sceneView);
        }

        public static void removeSceneView(TextureSceneView sceneView)
        {
            activeImages.Remove(sceneView);
        }

        /// <summary>
        /// The number of images to update when a tick happens.
        /// </summary>
        public static int NumImagesToUpdate
        {
            get
            {
                return numImagesToUpdate;
            }
            set
            {
                numImagesToUpdate = value;
            }
        }

        /// <summary>
        /// How often between image updates.
        /// </summary>
        public static double SecondsToSleep
        {
            get
            {
                return secondsToSleep;
            }
            set
            {
                secondsToSleep = value;
            }
        }

        private static IEnumerator<YieldAction> renderUpdates()
        {
            int count = 0;
            while (render)
            {
                int iterations = numImagesToUpdate < activeImages.Count ? numImagesToUpdate : activeImages.Count;
                for (int i = 0; i < iterations; ++i)
                {
                    if (count < activeImages.Count)
                    {
                        activeImages[count++].RenderOneFrame = true;
                    }
                    else
                    {
                        count = 0;
                        if (activeImages.Count > 0)
                        {
                            activeImages[count++].RenderOneFrame = true;
                        }
                    }
                }
                yield return Coroutine.Wait(secondsToSleep);
            }
        }
    }
}
