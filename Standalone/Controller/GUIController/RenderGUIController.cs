using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class RenderGUIController
    {
        private SceneViewController sceneViewController;
        private ImageRenderer imageRenderer;

        public RenderGUIController(Widget ribbonGui, SceneViewController sceneViewController, ImageRenderer imageRenderer)
        {
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;

            Button renderButton = ribbonGui.findWidget("RenderingTab/RenderButton") as Button;
            renderButton.MouseButtonClick +=new MyGUIEvent(renderButton_MouseButtonClick);
        }

        void  renderButton_MouseButtonClick(Widget source, EventArgs e)
        {
 	        render();
        }

        private void render()
        {
            //StatusController.SetStatus("Rendering image...");
            SceneViewWindow drawingWindow = sceneViewController.ActiveWindow;
            if (drawingWindow != null)
            {
                int width = 1024;// (int)this.width.Value;
                int height = 768;// (int)this.height.Value;
                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = width;
                imageProperties.Height = height;
                imageProperties.UseWindowBackgroundColor = false;
                imageProperties.CustomBackgroundColor = Engine.Color.Black;// Engine.Color.FromARGB(backgroundColorButton.SelectedColor.ToArgb());
                imageProperties.AntiAliasingMode = 1;// (int)Math.Pow(2, aaCombo.SelectedIndex);
                Bitmap bitmap = imageRenderer.renderImage(imageProperties);
                if (bitmap != null)
                {
                    PictureForm picture = new PictureForm();
                    picture.initialize(bitmap);
                    picture.Text = String.Format("{0} - {1}x{2}", drawingWindow.Name, width, height);
                    picture.Show();
                }
            }
            //StatusController.TaskCompleted();
        }
    }
}
