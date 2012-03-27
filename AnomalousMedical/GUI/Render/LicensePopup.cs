using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class LicensePopup : AbstractFullscreenGUIPopup
    {
        private EditBox licenseTextArea;

        public LicensePopup(GUIManager guiManager)
            :base("Medical.GUI.Render.LicensePopup.layout", guiManager)
        {
            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            licenseTextArea = (EditBox)widget.findWidget("LicenseTextArea");
        }

        public String LicenseText
        {
            get
            {
                return licenseTextArea.Caption;
            }
            set
            {
                licenseTextArea.Caption = value;
            }
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }
    }
}
