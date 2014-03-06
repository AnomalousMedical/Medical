using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class LicenseDisplay : Component
    {
        private EventLayoutContainer layoutContainer = new EventLayoutContainer();

        private ImageBox logoImage;
        private TextBox licenseText;

        public LicenseDisplay()
            :base("Medical.GUI.LicenseDisplay.LicenseDisplay.layout")
        {
            logoImage = (ImageBox)widget.findWidget("LogoWatermark");
            logoImage.setItemResource("AnomalousMedical/CornerLogo");
            licenseText = (TextBox)widget.findWidget("LicenseText");

            layoutContainer.LayoutChanged += layoutContainer_LayoutChanged;
        }

        void layoutContainer_LayoutChanged(EventLayoutContainer obj)
        {
            if (widget.Height < layoutContainer.WorkingSize.Height)
            {
                widget.setCoord(layoutContainer.Location.x, layoutContainer.WorkingSize.Height - widget.Height + layoutContainer.Location.y, layoutContainer.WorkingSize.Width, widget.Height);
                widget.Visible = true;
            }
            else
            {
                widget.Visible = false;
            }
        }

        public SingleChildLayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
        }

        public string LicenseText
        {
            get
            {
                return licenseText.Caption;
            }
            set
            {
                licenseText.Caption = value;
            }
        }
    }
}
