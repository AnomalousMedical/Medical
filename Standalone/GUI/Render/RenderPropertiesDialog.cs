using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using System.Drawing;

namespace Medical.GUI
{
    public class RenderPropertiesDialog : Dialog
    {
        private ComboBox aaCombo;
        private NumericEdit width;
        private NumericEdit height;

        private ResolutionMenu resolutionMenu;
        private SceneViewController sceneViewController;
        private ImageRenderer imageRenderer;

        public RenderPropertiesDialog(SceneViewController sceneViewController, ImageRenderer imageRenderer)
            :base("Medical.GUI.Render.RenderPropertiesDialog.layout")
        {
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;

            aaCombo = window.findWidget("RenderingTab/AACombo") as ComboBox;
            aaCombo.SelectedIndex = aaCombo.ItemCount - 1;

            width = new NumericEdit(window.findWidget("RenderingTab/WidthEdit") as Edit);
            height = new NumericEdit(window.findWidget("RenderingTab/HeightEdit") as Edit);

            Button renderButton = window.findWidget("RenderingTab/Render") as Button;
            renderButton.MouseButtonClick += new MyGUIEvent(renderButton_MouseButtonClick);

            Button sizeButton = window.findWidget("RenderingTab/SizeButton") as Button;
            sizeButton.MouseButtonClick += new MyGUIEvent(sizeButton_MouseButtonClick);

            //ResolutionMenu
            resolutionMenu = new ResolutionMenu();
            resolutionMenu.ResolutionChanged += new EventHandler(resolutionMenu_ResolutionChanged);
        }

        public void render()
        {
            SceneViewWindow drawingWindow = sceneViewController.ActiveWindow;
            if (drawingWindow != null)
            {
                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = Width;
                imageProperties.Height = Height;
                imageProperties.UseWindowBackgroundColor = true;
                imageProperties.AntiAliasingMode = AAValue;
                Bitmap bitmap = imageRenderer.renderImage(imageProperties);
                if (bitmap != null)
                {
                    ImageWindow window = new ImageWindow(MainWindow.Instance, sceneViewController.ActiveWindow.Name, bitmap);
                }
            }
        }

        void renderButton_MouseButtonClick(Widget source, EventArgs e)
        {
            render();
        }

        void sizeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            resolutionMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        void resolutionMenu_ResolutionChanged(object sender, EventArgs e)
        {
            width.Edit.Enabled = height.Edit.Enabled = resolutionMenu.IsCustom;
            Width = resolutionMenu.ImageWidth;
            Height = resolutionMenu.ImageHeight;
        }

        public int Width
        {
            get
            {
                return width.IntValue;
            }
            set
            {
                width.IntValue = value;
            }
        }

        public int Height
        {
            get
            {
                return height.IntValue;
            }
            set
            {
                height.IntValue = value;
            }
        }

        public int AAValue
        {
            get
            {
                return (int)Math.Pow(2, aaCombo.SelectedIndex);
            }
        }
    }
}
