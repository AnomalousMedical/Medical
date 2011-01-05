using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using System.Drawing;
using Engine;

namespace Medical.GUI
{
    public class RenderTaskbarItem : TaskbarItem
    {
        private SceneViewController sceneViewController;
        private ImageRenderer imageRenderer;
        private RenderPropertiesPopup properties;

        public RenderTaskbarItem(SceneViewController sceneViewController, ImageRenderer imageRenderer, DialogManager dialogManager)
            :base("Render", "RenderIconLarge")
        {
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;
            properties = new RenderPropertiesPopup();
            properties.Render += new EventHandler(properties_Render);
            dialogManager.addManagedDialog(properties);
        }

        public override void Dispose()
        {
            properties.Dispose();
            base.Dispose();
        }

        public override void clicked(Widget source, EventArgs e)
        {
            properties.open(false);
        }

        public override void rightClicked(Widget source, EventArgs e)
        {
            render();
        }

        public void render()
        {
            SceneViewWindow drawingWindow = sceneViewController.ActiveWindow;
            if (drawingWindow != null)
            {
                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = properties.Width;
                imageProperties.Height = properties.Height;
                imageProperties.UseWindowBackgroundColor = true;
                imageProperties.AntiAliasingMode = properties.AAValue;
                Bitmap bitmap = imageRenderer.renderImage(imageProperties);
                if (bitmap != null)
                {
                    ImageWindow window = new ImageWindow(MainWindow.Instance, sceneViewController.ActiveWindow.Name, bitmap);
                }
            }
        }

        void properties_Render(object sender, EventArgs e)
        {
            render();
        }
    }
}
