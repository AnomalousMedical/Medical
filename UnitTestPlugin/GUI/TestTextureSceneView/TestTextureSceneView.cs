using Engine;
using Medical;
using Medical.Controller;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestPlugin.GUI
{
    class TestTextureSceneView : MDIDialog
    {
        private bool render = true;
        private ButtonGrid buttonGrid;
        private List<PooledSceneView> activeImages = new List<PooledSceneView>();
        private TextureSceneViewPool texturePool;
        private SceneViewController sceneViewController;
        private ScrollView scrollView;

        private EditBox secondsToSleepEdit;
        private EditBox numToUpdateEdit;

        private int numImagesToUpdate = 3;
        private double secondsToSleep = 1;

        private class ButtonGridButtonInfo
        {
            public ButtonGridButtonInfo()
            {
                Visible = false;
                FirstTextureAssignment = true;
            }

            public LayerState Layers { get; set; }

            public Vector3 Translation { get; set; }

            public Vector3 LookAt { get; set; }

            public bool Visible { get; set; }

            public PooledSceneView CurrentSceneView { get; set; }

            public bool FirstTextureAssignment { get; set; }
        }

        public TestTextureSceneView(SceneViewController sceneViewController)
            : base("UnitTestPlugin.GUI.TestTextureSceneView.TestTextureSceneView.layout")
        {
            this.sceneViewController = sceneViewController;
            this.texturePool = new TextureSceneViewPool(sceneViewController, "TestRTT_", new IntSize2(200, 200));
            texturePool.SceneViewDestroyed += texturePool_SceneViewDestroyed;

            Button addButton = (Button)window.findWidget("AddButton");
            addButton.MouseButtonClick += addButton_MouseButtonClick;

            scrollView = (ScrollView)window.findWidget("ScrollView");
            buttonGrid = new SingleSelectButtonGrid(scrollView);
            scrollView.CanvasPositionChanged += scrollView_CanvasPositionChanged;

            window.WindowChangedCoord += window_WindowChangedCoord;

            numToUpdateEdit = (EditBox)window.findWidget("NumToUpdate");
            numToUpdateEdit.Caption = numImagesToUpdate.ToString();
            secondsToSleepEdit = (EditBox)window.findWidget("SecondsToSleep");
            secondsToSleepEdit.Caption = secondsToSleep.ToString();
            Button applyButton = (Button)window.findWidget("ApplyButton");
            applyButton.MouseButtonClick += applyButton_MouseButtonClick;

            Coroutine.Start(renderUpdates());
        }

        void scrollView_CanvasPositionChanged(Widget source, EventArgs e)
        {
            determineVisibleItems();
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            int.TryParse(numToUpdateEdit.Caption, out numImagesToUpdate);
            double.TryParse(secondsToSleepEdit.Caption, out secondsToSleep);
        }

        public override void Dispose()
        {
            render = false;
            buttonGrid.SuppressLayout = true;
            foreach (var info in activeImages)
            {
                info.finished();
            }
            buttonGrid.SuppressLayout = false;
            texturePool.Dispose();
            base.Dispose();
        }

        private IEnumerator<YieldAction> renderUpdates()
        {
            int count = 0;
            while(render)
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

        static int count = 0;

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            LayerState layers = new LayerState("");
            layers.captureState();

            ButtonGridItem item = buttonGrid.addItem("Main", count++.ToString());
            item.UserObject = new ButtonGridButtonInfo()
            {
                Layers = layers,
                Translation = activeWindow.Translation,
                LookAt = activeWindow.LookAt,
                Visible = false
            };

            buttonGrid.resizeAndLayout(window.ClientWidget.Width);
            determineVisibleItems();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            buttonGrid.resizeAndLayout(scrollView.ViewCoord.width);
            determineVisibleItems();
        }

        private void determineVisibleItems()
        {
            IntCoord viewArea = scrollView.ViewCoord;
            var canvasPos = scrollView.CanvasPosition;
            viewArea.left = (int)canvasPos.x;
            viewArea.top = (int)canvasPos.y;
            foreach (var item in buttonGrid.Items)
            {
                ButtonGridButtonInfo info = (ButtonGridButtonInfo)item.UserObject;
                bool overlaps = viewArea.overlaps(item.Coord);
                if(overlaps != info.Visible)
                {
                    info.Visible = overlaps;
                    if (info.Visible)
                    {
                        Logging.Log.Debug("Creating texture for {0}", item.Caption);
                        var sceneView = texturePool.getSceneView(info.Translation, info.LookAt, info.Layers);
                        sceneView.SceneView.AlwaysRender = false;
                        sceneView.SceneView.RenderOneFrame = true;

                        if (info.FirstTextureAssignment)
                        {
                            item.ImageBox.setImageTexture(sceneView.SceneView.TextureName);
                            item.ImageBox.setImageCoord(new IntCoord(0, 0, (int)sceneView.SceneView.Width, (int)sceneView.SceneView.Height));
                            info.FirstTextureAssignment = false;
                        }
                        else
                        {
                            item.ImageBox.setImageInfo(sceneView.SceneView.TextureName, new IntCoord(0, 0, (int)sceneView.SceneView.Width, (int)sceneView.SceneView.Height), new IntSize2((int)sceneView.SceneView.Width, (int)sceneView.SceneView.Height));
                        }

                        activeImages.Add(sceneView);
                        info.CurrentSceneView = sceneView;
                    }
                    else
                    {
                        Logging.Log.Debug("Destroying texture for {0}", item.Caption);
                        info.CurrentSceneView.finished();
                        activeImages.Remove(info.CurrentSceneView);
                        info.CurrentSceneView = null;
                    }
                }
            }
        }

        void texturePool_SceneViewDestroyed(PooledSceneView sceneView)
        {
            RenderManager.Instance.destroyTexture(sceneView.SceneView.Name);
        }
    }
}
