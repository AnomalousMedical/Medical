using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class InstallPanel
    {
        private Widget installPanelWidget;
        public event EventHandler InstallItem;
        private StaticImage installIcon;
        private Edit installName;
        private Edit installDescription;

        public InstallPanel(Widget widget)
        {
            this.installPanelWidget = widget;

            Button installButton = (Button)widget.findWidget("InstallButton");
            installButton.MouseButtonClick += new MyGUIEvent(installButton_MouseButtonClick);

            installIcon = (StaticImage)widget.findWidget("InstallIcon");
            installName = (Edit)widget.findWidget("InstallName");
            installDescription = (Edit)widget.findWidget("InstallDescription");
        }

        public void setDownloadInfo(ServerDownloadInfo info)
        {
            installIcon.setItemResource(info.ImageKey);
            installName.Caption = info.Name;
            installName.TextCursor = 0;
            installDescription.Caption = "Loading information from server...";
            info.getDescription(delegate(String caption, DownloadGUIInfo downloadGUIInfo)
            {
                if (info == downloadGUIInfo)
                {
                    installDescription.Caption = caption;
                    installDescription.TextCursor = 0;
                }
            });
        }

        public bool Visible
        {
            get
            {
                return installPanelWidget.Visible;
            }
            set
            {
                installPanelWidget.Visible = value;
            }
        }

        void installButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (InstallItem != null)
            {
                InstallItem.Invoke(this, e);
            }
        }
    }
}
