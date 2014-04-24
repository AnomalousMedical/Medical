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
        private SceneViewController sceneViewController;
        private bool render = true;
        private ButtonGrid buttonGrid;
        private List<ImageInfo> activeImages = new List<ImageInfo>();
        private ScrollView scrollView;

        private EditBox secondsToSleepEdit;
        private EditBox numToUpdateEdit;

        private int numImagesToUpdate = 3;
        private double secondsToSleep = 1;

        class ImageInfo
        {
            public String TextureName { get; set; }

            public TextureSceneView SceneView { get; set; }

            public ButtonGridItem LayoutContainer { get; set; }
        }

        public TestTextureSceneView(SceneViewController sceneViewController)
            : base("UnitTestPlugin.GUI.TestTextureSceneView.TestTextureSceneView.layout")
        {
            this.sceneViewController = sceneViewController;
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
                RenderManager.Instance.destroyTexture(info.TextureName);
                buttonGrid.removeItem(info.LayoutContainer);
                sceneViewController.destroyWindow(info.SceneView);
            }
            buttonGrid.SuppressLayout = false;
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
            String textureName = "TestRTT_" + count++;
            int width = 100;
            int height = 100;

            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            TextureSceneView sceneView = sceneViewController.createTextureSceneView(textureName, activeWindow.Translation, activeWindow.LookAt, width * 2, height * 2);

            LayerState layers = new LayerState("");
            layers.captureState();
            String activeTransaparencyState = TransparencyController.ActiveTransparencyState;
            TransparencyController.ActiveTransparencyState = sceneView.CurrentTransparencyState;
            layers.instantlyApply();
            TransparencyController.ActiveTransparencyState = activeTransaparencyState;

            sceneView.AlwaysRender = false;
            sceneView.RenderOneFrame = true;

            ButtonGridItem item = buttonGrid.addItem("Main", textureName, textureName);
            item.ImageBox.setImageTexture(textureName);
            item.ImageBox.setImageCoord(new IntCoord(0, 0, width * 2, height * 2));

            activeImages.Add(new ImageInfo()
            {
                LayoutContainer = item,
                SceneView = sceneView,
                TextureName = textureName
            });

            buttonGrid.resizeAndLayout(window.ClientWidget.Width);
            determineVisibleItems();//slow will go over all elements
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
            Logging.Log.Debug("Canvas Position Changed {0}", viewArea);
            foreach (var item in buttonGrid.Items)
            {
                Logging.Log.Debug("{0} {1} {2}", item.Caption, item.Coord, viewArea.overlaps(item.Coord) ? "onscreen" : "offscreen");
            }
        }
    }
}
