using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class RenderGUIController : IDisposable
    {
        private SceneViewController sceneViewController;
        private ImageRenderer imageRenderer;

        private ComboBox aaCombo;
        private NumericEdit width;
        private NumericEdit height;

        private Layout resolutionMenu;
        private PopupContainer resolutionMenuPopup;
        private ButtonGroup resolutionMenuGroup;
        private Button onePointThreeMegapixel;
        private Button fourMegapixel;
        private Button sixMegapixel;
        private Button eightMegapixel;
        private Button tenMegapixel;
        private Button twelveMegapixel;
        private Button custom;

        private ColorMenu colorMenu;

        public RenderGUIController(Widget ribbonGui, SceneViewController sceneViewController, ImageRenderer imageRenderer)
        {
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;

            Button renderButton = ribbonGui.findWidget("RenderingTab/RenderButton") as Button;
            renderButton.MouseButtonClick +=new MyGUIEvent(renderButton_MouseButtonClick);

            aaCombo = ribbonGui.findWidget("RenderingTab/AACombo") as ComboBox;
            aaCombo.SelectedIndex = aaCombo.getItemCount() - 1;

            width = new NumericEdit(ribbonGui.findWidget("RenderingTab/WidthEdit") as Edit);
            height = new NumericEdit(ribbonGui.findWidget("RenderingTab/HeightEdit") as Edit);

            Button sizeButton = ribbonGui.findWidget("RenderingTab/SizeButton") as Button;
            sizeButton.MouseButtonClick += new MyGUIEvent(sizeButton_MouseButtonClick);

            Button colorButton = ribbonGui.findWidget("RenderingTab/ColorButton") as Button;
            colorButton.MouseButtonClick += new MyGUIEvent(colorButton_MouseButtonClick);

            colorMenu = new ColorMenu();

            //ResolutionMenu
            resolutionMenu = LayoutManager.Instance.loadLayout("Medical.Controller.GUIController.ResolutionMenu.layout");
            Widget resolutionMenuWidget = resolutionMenu.getWidget(0);
            resolutionMenuWidget.Visible = false;
            resolutionMenuPopup = new PopupContainer(resolutionMenuWidget);
            onePointThreeMegapixel = resolutionMenuWidget.findWidget("1Point3Megapixel") as Button;
            fourMegapixel = resolutionMenuWidget.findWidget("4Megapixel") as Button;
            sixMegapixel = resolutionMenuWidget.findWidget("6Megapixel") as Button;
            eightMegapixel = resolutionMenuWidget.findWidget("8Megapixel") as Button;
            tenMegapixel = resolutionMenuWidget.findWidget("10Megapixel") as Button;
            twelveMegapixel = resolutionMenuWidget.findWidget("12Megapixel") as Button;
            custom = resolutionMenuWidget.findWidget("Custom") as Button;

            resolutionMenuGroup = new ButtonGroup();
            resolutionMenuGroup.SelectedButtonChanged += new EventHandler(resolutionMenuGroup_SelectedButtonChanged);
            resolutionMenuGroup.addButton(onePointThreeMegapixel);
            resolutionMenuGroup.addButton(fourMegapixel);
            resolutionMenuGroup.addButton(sixMegapixel);
            resolutionMenuGroup.addButton(eightMegapixel);
            resolutionMenuGroup.addButton(tenMegapixel);
            resolutionMenuGroup.addButton(twelveMegapixel);
            resolutionMenuGroup.addButton(custom);
            resolutionMenuGroup.SelectedButton = custom;
        }

        public void Dispose()
        {
            colorMenu.Dispose();
            LayoutManager.Instance.unloadLayout(resolutionMenu);
        }

        void resolutionMenuGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            Button selectedButton = resolutionMenuGroup.SelectedButton;
            width.Edit.Enabled = height.Edit.Enabled = selectedButton == custom;
            if(selectedButton == onePointThreeMegapixel)
            {
                width.IntValue = 1280;
                height.IntValue = 1024;
            }
            else if (selectedButton == fourMegapixel)
            {
                width.IntValue = 2448;
                height.IntValue = 1632;
            }
            else if (selectedButton == sixMegapixel)
            {
                width.IntValue = 3000;
                height.IntValue = 2000;
            }
            else if (selectedButton == eightMegapixel)
            {
                width.IntValue = 3456;
                height.IntValue = 2304;
            }
            else if (selectedButton == tenMegapixel)
            {
                width.IntValue = 3648;
                height.IntValue = 2736;
            }
            else if (selectedButton == twelveMegapixel)
            {
                width.IntValue = 4000;
                height.IntValue = 3000;
            }
        }

        void renderButton_MouseButtonClick(Widget source, EventArgs e)
        {
 	        render();
        }

        void sizeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            resolutionMenuPopup.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        void colorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            colorMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        private void render()
        {
            //StatusController.SetStatus("Rendering image...");
            SceneViewWindow drawingWindow = sceneViewController.ActiveWindow;
            if (drawingWindow != null)
            {
                int width = this.width.IntValue;
                int height = this.height.IntValue;
                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = width;
                imageProperties.Height = height;
                imageProperties.UseWindowBackgroundColor = false;
                imageProperties.CustomBackgroundColor = colorMenu.SelectedColor;
                imageProperties.AntiAliasingMode = (int)Math.Pow(2, aaCombo.SelectedIndex);
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
