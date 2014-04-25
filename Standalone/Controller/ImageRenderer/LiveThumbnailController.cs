using Engine;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class simplifies getting render to texture scene views to use as thumbnails elsewhere in the system.
    /// It will help keep track of what is and is not visible and will maintain the pool of thumbnail images.
    /// </summary>
    public class LiveThumbnailController : IDisposable
    {
        private bool render = true;
        private List<PooledSceneView> activeImages = new List<PooledSceneView>();
        private TextureSceneViewPool texturePool;
        private SceneViewController sceneViewController;
        private List<LiveThumbnailHostInfo> thumbnailHosts = new List<LiveThumbnailHostInfo>();

        private int numImagesToUpdate = 3;
        private double secondsToSleep = 1;

        /// <summary>
        /// This event is fired when the actual thumbnail render to texture is destroyed. If you need to cleanup something
        /// on your end, do it here.
        /// </summary>
        public event Action<LiveThumbnailController, PooledSceneView> ThumbnailDestroyed;

        public LiveThumbnailController(String baseName, IntSize2 size, SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
            this.texturePool = new TextureSceneViewPool(sceneViewController, baseName, size);
            texturePool.SceneViewDestroyed += texturePool_SceneViewDestroyed;

            Coroutine.Start(renderUpdates());
        }

        public void Dispose()
        {
            render = false;
            foreach (var info in activeImages)
            {
                info.finished();
            }
            texturePool.Dispose();
        }

        /// <summary>
        /// Add a thumbnail host to be tracked by this controller.
        /// </summary>
        /// <param name="host"></param>
        public void addThumbnailHost(LiveThumbnailHost host)
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            LayerState layers = new LayerState("");
            layers.captureState();

            thumbnailHosts.Add(new LiveThumbnailHostInfo()
            {
                Host = host,
                Layers = layers,
                Translation = activeWindow.Translation,
                LookAt = activeWindow.LookAt,
                Visible = false,
            });
        }

        public void removeThumbnailHost(LiveThumbnailHost host)
        {
            returnThumbToPool(host._HostInfo);
        }

        /// <summary>
        /// Determine what is visible. Call this anytime your visible area displaying thumbnails changes.
        /// </summary>
        /// <param name="viewArea">The visible area to test against.</param>
        public void determineVisibleHosts(IntCoord viewArea)
        {
            foreach (var info in thumbnailHosts)
            {
                bool overlaps = viewArea.overlaps(info.Host.Coord);
                if (overlaps != info.Visible)
                {
                    info.Visible = overlaps;
                    if (info.Visible)
                    {
                        createLiveThumb(info);
                    }
                    else
                    {
                        returnThumbToPool(info);
                    }
                }
            }
        }

        private void createLiveThumb(LiveThumbnailHostInfo info)
        {
            var sceneView = texturePool.getSceneView(info.Translation, info.LookAt, info.Layers);
            sceneView.SceneView.AlwaysRender = false;
            sceneView.SceneView.RenderOneFrame = true;

            info.Host.setTextureInfo(sceneView.SceneView.TextureName, new IntCoord(0, 0, (int)sceneView.SceneView.Width, (int)sceneView.SceneView.Height));

            activeImages.Add(sceneView);
            info.CurrentSceneView = sceneView;

            info.WindowCreatedCallback = (window) =>
            {
                setupWindowLayers(window, info.Layers);
            };

            sceneView.SceneView.CameraCreated += info.WindowCreatedCallback;
        }

        private void returnThumbToPool(LiveThumbnailHostInfo info)
        {
            info.CurrentSceneView.SceneView.CameraCreated -= info.WindowCreatedCallback;
            info.WindowCreatedCallback = null;
            activeImages.Remove(info.CurrentSceneView);
            info.CurrentSceneView.finished();
            info.CurrentSceneView = null;
        }

        /// <summary>
        /// The number of images to update when a tick happens.
        /// </summary>
        public int NumImagesToUpdate
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
        public double SecondsToSleep
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

        private IEnumerator<YieldAction> renderUpdates()
        {
            int count = 0;
            while (render)
            {
                for (int i = 0; i < numImagesToUpdate; ++i)
                {
                    if (count < activeImages.Count)
                    {
                        activeImages[count++].SceneView.RenderOneFrame = true;
                    }
                    else
                    {
                        count = 0;
                        if (activeImages.Count > 0)
                        {
                            activeImages[count++].SceneView.RenderOneFrame = true;
                        }
                    }
                }
                yield return Coroutine.Wait(secondsToSleep);
            }
        }

        void setupWindowLayers(SceneViewWindow window, LayerState layers)
        {
            layers.instantlyApplyTo(window.CurrentTransparencyState);
        }

        void texturePool_SceneViewDestroyed(PooledSceneView sceneView)
        {
            if(ThumbnailDestroyed != null)
            {
                ThumbnailDestroyed.Invoke(this, sceneView);
            }
        }
    }
}
