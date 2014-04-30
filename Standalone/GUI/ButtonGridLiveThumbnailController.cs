using Engine;
using Medical.Controller;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class ButtonGridLiveThumbnailController<UserObjectType> : IDisposable
    {
        private SingleSelectButtonGrid buttonGrid;
        private ScrollView scrollView;

        private LiveThumbnailController liveThumbnailController;

        public ButtonGridLiveThumbnailController(String baseName, IntSize2 thumbSize, SceneViewController sceneViewController, SingleSelectButtonGrid buttonGrid, ScrollView scrollView)
        {
            this.buttonGrid = buttonGrid;
            this.scrollView = scrollView;
            scrollView.CanvasPositionChanged += scrollView_CanvasPositionChanged;
            liveThumbnailController = new LiveThumbnailController("TestRTT_", new IntSize2(200, 200), sceneViewController);
            liveThumbnailController.ThumbnailDestroyed += liveThumbnailController_ThumbnailDestroyed;
        }

        public void Dispose()
        {
            liveThumbnailController.Dispose();
        }

        public void itemAdded(ButtonGridItem item, UserObjectType userObject = default(UserObjectType))
        {
            ButtonGridItemLiveThumbnailHost host = new ButtonGridItemLiveThumbnailHost(item);
            host.UserObject = userObject;
            item.UserObject = host;
            liveThumbnailController.addThumbnailHost(host);
            determineVisibleHosts();
        }

        public void itemRemoved(ButtonGridItem item)
        {
            liveThumbnailController.removeThumbnailHost((LiveThumbnailHost)item.UserObject);
            determineVisibleHosts();
        }

        public UserObjectType getUserObject(ButtonGridItem item)
        {
            if (item != null)
            {
                return ((ButtonGridItemLiveThumbnailHost)item.UserObject).UserObject;
            }
            return default(UserObjectType);
        }

        /// <summary>
        /// Resize the button grid, will also trigger a determineVisibleHosts.
        /// </summary>
        /// <param name="newWidth">The new width of the button grid.</param>
        public void resizeAndLayout()
        {
            buttonGrid.resizeAndLayout(scrollView.ViewCoord.width);
            determineVisibleHosts();
        }

        /// <summary>
        /// Manually determine visible hosts, call this if the button grid layout changes automatically.
        /// </summary>
        public void determineVisibleHosts()
        {
            liveThumbnailController.determineVisibleHosts(VisibleArea);
        }

        public double SecondsToSleep
        {
            get
            {
                return liveThumbnailController.SecondsToSleep;
            }
            set
            {
                liveThumbnailController.SecondsToSleep = value;
            }
        }

        public int NumImagesToUpdate
        {
            get
            {
                return liveThumbnailController.NumImagesToUpdate;
            }
            set
            {
                liveThumbnailController.NumImagesToUpdate = value;
            }
        }

        private IntCoord VisibleArea
        {
            get
            {
                IntCoord viewArea = scrollView.ViewCoord;
                var canvasPos = scrollView.CanvasPosition;
                viewArea.left = (int)canvasPos.x;
                viewArea.top = (int)canvasPos.y;

                return viewArea;
            }
        }

        void liveThumbnailController_ThumbnailDestroyed(LiveThumbnailController thumbController, PooledSceneView sceneView)
        {
            RenderManager.Instance.destroyTexture(sceneView.SceneView.Name);
        }

        void scrollView_CanvasPositionChanged(Widget source, EventArgs e)
        {
            determineVisibleHosts();
        }

        class ButtonGridItemLiveThumbnailHost : LiveThumbnailHost
        {
            private ButtonGridItem item;

            public ButtonGridItemLiveThumbnailHost(ButtonGridItem item)
            {
                this.item = item;
            }

            public override IntCoord Coord
            {
                get
                {
                    return item.Coord;
                }
            }

            public UserObjectType UserObject { get; set; }

            public override void setTextureInfo(string name, IntCoord coord)
            {
                item.ImageBox.setImageTexture(name);
                item.ImageBox.setImageCoord(coord);
            }
        }
    }

    public class ButtonGridLiveThumbnailController : ButtonGridLiveThumbnailController<Object>
    {
        public ButtonGridLiveThumbnailController(String baseName, IntSize2 thumbSize, SceneViewController sceneViewController, SingleSelectButtonGrid buttonGrid, ScrollView scrollView)
            :base(baseName, thumbSize, sceneViewController, buttonGrid, scrollView)
        {
        }
    }
}
