using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Developer.GUI
{
    class DeveloperResolutionMenu : PopupContainer
    {
        public event EventHandler ResolutionChanged;

        private ButtonGroup resolutionMenuGroup;
        private Button onePointThreeMegapixel;
        private Button fourMegapixel;
        private Button sixMegapixel;
        private Button eightMegapixel;
        private Button tenMegapixel;
        private Button twelveMegapixel;
        private Button custom;

        public DeveloperResolutionMenu()
            : base("Developer.GUI.DeveloperRenderer.DeveloperResolutionMenu.layout")
        {
            onePointThreeMegapixel = widget.findWidget("1Point3Megapixel") as Button;
            fourMegapixel = widget.findWidget("4Megapixel") as Button;
            sixMegapixel = widget.findWidget("6Megapixel") as Button;
            eightMegapixel = widget.findWidget("8Megapixel") as Button;
            tenMegapixel = widget.findWidget("10Megapixel") as Button;
            twelveMegapixel = widget.findWidget("12Megapixel") as Button;
            custom = widget.findWidget("Custom") as Button;

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

        public bool IsCustom
        {
            get
            {
                return resolutionMenuGroup.SelectedButton == custom;
            }
        }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        void resolutionMenuGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            Button selectedButton = resolutionMenuGroup.SelectedButton;
            if (selectedButton == onePointThreeMegapixel)
            {
                ImageWidth = 1280;
                ImageHeight = 1024;
            }
            else if (selectedButton == fourMegapixel)
            {
                ImageWidth = 2448;
                ImageHeight = 1632;
            }
            else if (selectedButton == sixMegapixel)
            {
                ImageWidth = 3000;
                ImageHeight = 2000;
            }
            else if (selectedButton == eightMegapixel)
            {
                ImageWidth = 3456;
                ImageHeight = 2304;
            }
            else if (selectedButton == tenMegapixel)
            {
                ImageWidth = 3648;
                ImageHeight = 2736;
            }
            else if (selectedButton == twelveMegapixel)
            {
                ImageWidth = 4000;
                ImageHeight = 3000;
            }
            if (ResolutionChanged != null)
            {
                ResolutionChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
