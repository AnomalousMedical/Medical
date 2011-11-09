using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class UninstallPanel
    {
        private Widget widget;
        public event EventHandler UninstallItem;
        private StaticImage uninstallIcon;
        private Edit uninstallName;
        private Edit uninstallDescription;

        public UninstallPanel(Widget widget)
        {
            this.widget = widget;

            Button uninstallButton = (Button)widget.findWidget("UninstallButton");
            uninstallButton.MouseButtonClick += new MyGUIEvent(uninstallButton_MouseButtonClick);

            uninstallIcon = (StaticImage)widget.findWidget("UninstallIcon");
            uninstallName = (Edit)widget.findWidget("UninstallName");
            uninstallDescription = (Edit)widget.findWidget("UninstallDescription");
        }

        public void setInfo(UninstallInfo info)
        {
            uninstallIcon.setItemResource(info.ImageKey);
            uninstallName.Caption = info.Name;
            uninstallName.TextCursor = 0;

            info.getDescription(delegate(String caption, DownloadGUIInfo downloadGUIInfo)
            {
                if (downloadGUIInfo == info)
                {
                    uninstallDescription.Caption = caption;
                    uninstallDescription.TextCursor = 0;
                }
            });
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

        void uninstallButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (UninstallItem != null)
            {
                UninstallItem.Invoke(this, e);
            }
        }
    }
}
