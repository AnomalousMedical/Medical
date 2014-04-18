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

        public TestTextureSceneView(SceneViewController sceneViewController)
            : base("UnitTestPlugin.GUI.TestTextureSceneView.TestTextureSceneView.layout")
        {
            this.sceneViewController = sceneViewController;

            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            sceneView = sceneViewController.createTextureSceneView(textureName, activeWindow.Translation, activeWindow.LookAt, new Vector3(-15, -40, -15), new Vector3(15, 15, 15), 0, 1000);

            imageBox = (ImageBox)window.findWidget("ImageBox");

            imageBox.setImageTexture(textureName);
            imageBox.setImageCoord(new IntCoord(0, 0, 800, 600));

            //imageBox.setImageInfo("Test", new IntCoord(0, 0, 800, 600), new IntSize2(800, 600));
        }

        public override void Dispose()
        {
            RenderManager.Instance.destroyTexture(textureName);
            sceneViewController.destroyWindow(sceneView);
            base.Dispose();
        }
    }
}
