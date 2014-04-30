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
        private List<PooledSceneView> activeImages = new List<PooledSceneView>();
        private TextureSceneViewPool texturePool;
        private SceneViewController sceneViewController;
        private List<LiveThumbnailHostInfo> thumbnailHosts = new List<LiveThumbnailHostInfo>();
        private bool allowThumbUpdate = true;

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
        }

        public void Dispose()
        {
            foreach (var info in activeImages)
            {
                LiveThumbnailUpdater.removeSceneView(info.SceneView);
                info.finished();
            }
            texturePool.Dispose();
        }

        /// <summary>
        /// Add a thumbnail host to be tracked by this controller. Note that you will have to call determineVisibleHosts to get
        /// it to actually have a thumbnail.
        /// </summary>
        /// <param name="host"></param>
        public void addThumbnailHost(LiveThumbnailHost host)
        {
            host._HostInfo = new LiveThumbnailHostInfo()
            {
                Host = host,
                Visible = false,
            };

            thumbnailHosts.Add(host._HostInfo);
        }

        /// <summary>
        /// Remove a thumbnail host from tracking by this controller.
        /// </summary>
        /// <param name="host"></param>
        public void removeThumbnailHost(LiveThumbnailHost host)
        {
            var hostInfo = host._HostInfo;
            if (hostInfo.Visible)
            {
                returnThumbToPool(hostInfo);
            }
            thumbnailHosts.Remove(hostInfo);
            host._HostInfo = null;
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
            var sceneView = texturePool.getSceneView(info.Host.Translation, info.Host.LookAt, info.Host.Layers);
            sceneView.SceneView.AlwaysRender = false;
            sceneView.SceneView.RenderOneFrame = true;

            info.Host.setTextureInfo(sceneView.SceneView.TextureName, new IntCoord(0, 0, (int)sceneView.SceneView.Width, (int)sceneView.SceneView.Height));

            activeImages.Add(sceneView);
            if (allowThumbUpdate)
            {
                LiveThumbnailUpdater.addSceneView(sceneView.SceneView);
            }
            info.CurrentSceneView = sceneView;

            info.WindowCreatedCallback = (window) =>
            {
                setupWindowLayers(window, info.Host.Layers);
            };

            sceneView.SceneView.CameraCreated += info.WindowCreatedCallback;
        }

        private void returnThumbToPool(LiveThumbnailHostInfo info)
        {
            info.CurrentSceneView.SceneView.CameraCreated -= info.WindowCreatedCallback;
            info.WindowCreatedCallback = null;
            activeImages.Remove(info.CurrentSceneView);
            LiveThumbnailUpdater.removeSceneView(info.CurrentSceneView.SceneView);
            info.CurrentSceneView.finished();
            info.CurrentSceneView = null;
        }

        public IEnumerable<LiveThumbnailHost> Hosts
        {
            get
            {
                foreach(var hostInfo in thumbnailHosts)
                {
                    yield return hostInfo.Host;
                }
            }
        }

        public bool AllowThumbUpdate
        {
            get
            {
                return allowThumbUpdate;
            }
            set
            {
                if(allowThumbUpdate != value)
                {
                    allowThumbUpdate = value;
                    if(allowThumbUpdate)
                    {
                        foreach (var info in activeImages)
                        {
                            LiveThumbnailUpdater.addSceneView(info.SceneView);
                        }
                    }
                    else
                    {
                        foreach (var info in activeImages)
                        {
                            LiveThumbnailUpdater.removeSceneView(info.SceneView);
                        }
                    }
                }
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
