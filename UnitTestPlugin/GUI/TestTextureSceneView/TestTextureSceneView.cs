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

            window.WindowChangedCoord += window_WindowChangedCoord;

            Coroutine.Start(renderForce());
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

        private IEnumerator<YieldAction> renderForce()
        {
            int count = 0;
            while(render)
            {
                if (count < activeImages.Count)
                {
                    activeImages[count++].SceneView.RenderOneFrame = true;
                }
                else
                {
                    count = 0;
                }
                yield return Coroutine.Wait(1);
            }
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            String textureName = "TestRTT_" + Guid.NewGuid().ToString();
            int width = 100;
            int height = 100;

            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            TextureSceneView sceneView = sceneViewController.createTextureSceneView(textureName, activeWindow.Translation, activeWindow.LookAt, width, height);

            sceneView.AlwaysRender = false;
            sceneView.RenderOneFrame = true;

            ButtonGridItem item = buttonGrid.addItem("Main", textureName, textureName);
            item.ImageBox.setImageTexture(textureName);
            item.ImageBox.setImageCoord(new IntCoord(0, 0, width, height));

            activeImages.Add(new ImageInfo()
            {
                LayoutContainer = item,
                SceneView = sceneView,
                TextureName = textureName
            });

            buttonGrid.resizeAndLayout(window.ClientWidget.Width);
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            buttonGrid.resizeAndLayout(window.ClientWidget.Width);
        }
    }
}
