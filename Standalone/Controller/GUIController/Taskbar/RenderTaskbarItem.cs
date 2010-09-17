using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using System.Drawing;

namespace Medical.GUI
{
    class RenderTaskbarItem : TaskbarItem
    {
        private SceneViewController sceneViewController;
        private ImageRenderer imageRenderer;

        public RenderTaskbarItem(SceneViewController sceneViewController, ImageRenderer imageRenderer)
            :base("Render", "RenderIconLarge")
        {
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;
        }

        public override void clicked(Widget source, EventArgs e)
        {
            render();
        }

        private void render()
        {
            //StatusController.SetStatus("Rendering image...");
            SceneViewWindow drawingWindow = sceneViewController.ActiveWindow;
            if (drawingWindow != null)
            {
                int width = 1280;//this.width.IntValue;
                int height = 1024;// this.height.IntValue;
                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = width;
                imageProperties.Height = height;
                imageProperties.UseWindowBackgroundColor = true;
                imageProperties.AntiAliasingMode = (int)Math.Pow(2, 4);//aaCombo.SelectedIndex);
                Bitmap bitmap = imageRenderer.renderImage(imageProperties);
                if (bitmap != null)
                {
                    ImageWindow window = new ImageWindow(MainWindow.Instance, sceneViewController.ActiveWindow.Name, bitmap);
                }
            }
            //StatusController.TaskCompleted();
        }
    }
}
