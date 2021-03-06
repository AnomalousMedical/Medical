﻿using Anomalous.GuiFramework.Cameras;
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
        private ButtonGrid buttonGrid;
        private ScrollView scrollView;

        private LiveThumbnailController liveThumbnailController;

        public ButtonGridLiveThumbnailController(String baseName, IntSize2 thumbSize, SceneViewController sceneViewController, ButtonGrid buttonGrid, ScrollView scrollView)
            :this(new LiveThumbnailController(baseName, thumbSize, sceneViewController), buttonGrid, scrollView)
        {
            
        }

        public ButtonGridLiveThumbnailController(LiveThumbnailController liveThumbnailController, ButtonGrid buttonGrid, ScrollView scrollView)
        {
            this.liveThumbnailController = liveThumbnailController;
            liveThumbnailController.ThumbnailDestroyed += liveThumbnailController_ThumbnailDestroyed;
            this.buttonGrid = buttonGrid;
            this.scrollView = scrollView;
            scrollView.CanvasPositionChanged += scrollView_CanvasPositionChanged;
        }

        public void Dispose()
        {
            liveThumbnailController.Dispose();
        }

        public void itemAdded(ButtonGridItem item, LayerState layers, Vector3 translation, Vector3 lookAt, UserObjectType userObject = default(UserObjectType))
        {
            ButtonGridItemLiveThumbnailHost host = new ButtonGridItemLiveThumbnailHost(item)
            {
                Layers = layers,
                Translation = translation,
                LookAt = lookAt,
                UserObject = userObject
            };
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

        public String getTextureName(ButtonGridItem item)
        {
            if (item != null)
            {
                return ((ButtonGridItemLiveThumbnailHost)item.UserObject).TextureName;
            }
            return null;
        }

        public IntCoord getTextureCoord(ButtonGridItem item)
        {
            if (item != null)
            {
                return ((ButtonGridItemLiveThumbnailHost)item.UserObject).TextureCoord;
            }
            return default(IntCoord);
        }

        public ButtonGridItem findItemByUserObject(UserObjectType userObject)
        {
            if (userObject != null)
            {
                foreach (ButtonGridItemLiveThumbnailHost host in liveThumbnailController.Hosts)
                {
                    if (userObject.Equals(host.UserObject))
                    {
                        return host.Item;
                    }
                }
            }
            return null;
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
            IntCoord viewArea = VisibleArea;
            foreach (ButtonGridItemLiveThumbnailHost info in liveThumbnailController.Hosts)
            {
                liveThumbnailController.setVisibility(info, viewArea.overlaps(info.Coord));
            }
        }
        
        /// <summary>
        /// Force all thumbs to update for one frame.
        /// </summary>
        public void updateAllThumbs()
        {
            liveThumbnailController.updateAllThumbs();
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

            public IntCoord Coord
            {
                get
                {
                    return item.Coord;
                }
            }

            public UserObjectType UserObject { get; set; }

            public ButtonGridItem Item
            {
                get
                {
                    return item;
                }
            }

            public String TextureName { get; private set; }

            public IntCoord TextureCoord { get; private set; }

            public override void setTextureInfo(string name, IntCoord coord)
            {
                item.ImageBox.setImageTexture(name);
                item.ImageBox.setImageCoord(coord);

                this.TextureName = name;
                this.TextureCoord = coord;
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
