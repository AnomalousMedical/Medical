using Engine;
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
        private TextureSceneView sceneView;
        private ImageBox imageBox;
        String textureName = "TestSceneRTT";
        private bool render = true;

        public TestTextureSceneView(SceneViewController sceneViewController)
            : base("UnitTestPlugin.GUI.TestTextureSceneView.TestTextureSceneView.layout")
        {
            this.sceneViewController = sceneViewController;
            this.imageBox = (ImageBox)window.findWidget("ImageBox");

            int width = imageBox.Width;
            int height = imageBox.Height;

            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            sceneView = sceneViewController.createTextureSceneView(textureName, activeWindow.Translation, activeWindow.LookAt, width, height);

            imageBox.setImageTexture(textureName);
            imageBox.setImageCoord(new IntCoord(0, 0, width, height));

            sceneView.AlwaysRender = false;
            sceneView.RenderOneFrame = true;

            Coroutine.Start(renderForce());
        }

        public override void Dispose()
        {
            render = false;
            RenderManager.Instance.destroyTexture(textureName);
            sceneViewController.destroyWindow(sceneView);
            base.Dispose();
        }

        private IEnumerator<YieldAction> renderForce()
        {
            while(render)
            {
                sceneView.RenderOneFrame = true;
                yield return Coroutine.Wait(1);
            }
        }
    }
}
