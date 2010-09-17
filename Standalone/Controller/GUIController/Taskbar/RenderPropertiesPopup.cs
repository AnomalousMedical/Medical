using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class RenderPropertiesPopup : IDisposable
    {
        private Layout layout;
        private Widget mainWidget;
        private PopupContainer popupContainer;

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

        public RenderPropertiesPopup()
        {
            layout = LayoutManager.Instance.loadLayout("Medical.Controller.GUIController.Taskbar.RenderPropertiesPopup.layout");
            mainWidget = layout.getWidget(0);
            mainWidget.Visible = false;
            popupContainer = new PopupContainer(mainWidget);

            aaCombo = mainWidget.findWidget("RenderingTab/AACombo") as ComboBox;
            aaCombo.SelectedIndex = aaCombo.getItemCount() - 1;

            width = new NumericEdit(mainWidget.findWidget("RenderingTab/WidthEdit") as Edit);
            height = new NumericEdit(mainWidget.findWidget("RenderingTab/HeightEdit") as Edit);

            Button sizeButton = mainWidget.findWidget("RenderingTab/SizeButton") as Button;
            sizeButton.MouseButtonClick += new MyGUIEvent(sizeButton_MouseButtonClick);

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
            popupContainer.addChildPopup(resolutionMenuWidget);

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
            LayoutManager.Instance.unloadLayout(layout);
        }

        public void show(int left, int top)
        {
            popupContainer.show(left, top);
        }

        void sizeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            resolutionMenuPopup.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        void resolutionMenuGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            Button selectedButton = resolutionMenuGroup.SelectedButton;
            width.Edit.Enabled = height.Edit.Enabled = selectedButton == custom;
            if (selectedButton == onePointThreeMegapixel)
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
