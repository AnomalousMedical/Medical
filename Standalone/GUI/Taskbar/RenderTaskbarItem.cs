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
        private RenderPropertiesPopup properties;

        public RenderTaskbarItem(SceneViewController sceneViewController, ImageRenderer imageRenderer)
            :base("Render", "RenderIconLarge")
        {
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;
            properties = new RenderPropertiesPopup();
        }

        public override void Dispose()
        {
            properties.Dispose();
            base.Dispose();
        }

        public override void clicked(Widget source, EventArgs e)
        {
            render();
        }

        public override void rightClicked(Widget source, EventArgs e)
        {
            properties.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        private void render()
        {
            //StatusController.SetStatus("Rendering image...");
            SceneViewWindow drawingWindow = sceneViewController.ActiveWindow;
            if (drawingWindow != null)
            {
                int width = properties.Width;
                int height = properties.Height;
                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = width;
                imageProperties.Height = height;
                imageProperties.UseWindowBackgroundColor = true;
                imageProperties.AntiAliasingMode = properties.AAValue;
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
