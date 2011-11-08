using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class DownloadingPanel
    {
        private Widget widget;
        public event EventHandler CancelDownload;
        private StaticImage installIcon;
        private Edit installName;

        public DownloadingPanel(Widget widget)
        {
            this.widget = widget;

            Button cancelButton = (Button)widget.findWidget("CancelButton");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            installIcon = (StaticImage)widget.findWidget("DownloadingIcon");
            installName = (Edit)widget.findWidget("DownloadingName");
        }

        public void setDownloadInfo(ServerDownloadInfo info)
        {
            installIcon.setItemResource(info.ImageKey);
            installName.Caption = info.Name;
            installName.TextCursor = 0;
        }

        public bool Visible
        {
            get
            {
                return widget.Visible;
            }
            set
            {
                widget.Visible = value;
            }
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (CancelDownload != null)
            {
                CancelDownload.Invoke(this, e);
            }
        }
    }
}
