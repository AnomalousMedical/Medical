using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ResolutionMenu : PopupContainer
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

        public ResolutionMenu()
            : base("Medical.GUI.Render.ResolutionMenu.layout")
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

        public int Width { get; set; }

        public int Height { get; set; }

        void resolutionMenuGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            Button selectedButton = resolutionMenuGroup.SelectedButton;
            if (selectedButton == onePointThreeMegapixel)
            {
                Width = 1280;
                Height = 1024;
            }
            else if (selectedButton == fourMegapixel)
            {
                Width = 2448;
                Height = 1632;
            }
            else if (selectedButton == sixMegapixel)
            {
                Width = 3000;
                Height = 2000;
            }
            else if (selectedButton == eightMegapixel)
            {
                Width = 3456;
                Height = 2304;
            }
            else if (selectedButton == tenMegapixel)
            {
                Width = 3648;
                Height = 2736;
            }
            else if (selectedButton == twelveMegapixel)
            {
                Width = 4000;
                Height = 3000;
            }
            if (ResolutionChanged != null)
            {
                ResolutionChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
