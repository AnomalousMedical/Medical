using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class InstallPanel
    {
        public event Action<DownloadGUIInfo> InstallItem;
        public event Action<DownloadGUIInfo> UninstallItem;
        public event Action<DownloadGUIInfo> Restart;

        private Widget widget;
        private StaticImage icon;
        private Edit nameText;
        private Edit descriptionText;
        private Button actionButton;

        private DownloadGUIInfo currentInfo;

        public InstallPanel(Widget widget)
        {
            this.widget = widget;

            actionButton = (Button)widget.findWidget("InstallButton");
            actionButton.MouseButtonClick += new MyGUIEvent(actionButton_MouseButtonClick);

            icon = (StaticImage)widget.findWidget("InstallIcon");
            nameText = (Edit)widget.findWidget("InstallName");
            descriptionText = (Edit)widget.findWidget("InstallDescription");
        }

        public void setDownloadInfo(DownloadGUIInfo info)
        {
            currentInfo = info;
            icon.setItemResource(info.ImageKey);
            nameText.Caption = info.Name;
            nameText.TextCursor = 0;
            descriptionText.Caption = "Loading information from server...";
            switch (info.Status)
            {
                case ServerDownloadStatus.NotInstalled:
                    actionButton.Caption = "Install";
                    break;

                case ServerDownloadStatus.Installed:
                    actionButton.Caption = "Uninstall";
                    break;

                case ServerDownloadStatus.Update:
                    actionButton.Caption = "Update";
                    break;

                case ServerDownloadStatus.PendingUninstall:
                    actionButton.Caption = "Restart";
                    break;

                case ServerDownloadStatus.PendingInstall:
                    actionButton.Caption = "Restart";
                    break;
                    
            }
            info.getDescription(delegate(String caption, DownloadGUIInfo downloadGUIInfo)
            {
                if (currentInfo == downloadGUIInfo)
                {
                    if (caption.Length > descriptionText.MaxTextLength)
                    {
                        descriptionText.MaxTextLength = (uint)caption.Length;
                    }
                    descriptionText.Caption = caption;
                    descriptionText.TextCursor = 0;
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

        void actionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            switch (currentInfo.Status)
            {
                case ServerDownloadStatus.NotInstalled:
                    if (InstallItem != null)
                    {
                        InstallItem.Invoke(currentInfo);
                    }
                    break;

                case ServerDownloadStatus.Update:
                    if (InstallItem != null)
                    {
                        InstallItem.Invoke(currentInfo);
                    }
                    break;

                case ServerDownloadStatus.Installed:
                    if (UninstallItem != null)
                    {
                        UninstallItem.Invoke(currentInfo);
                    }
                    break;

                case ServerDownloadStatus.PendingUninstall:
                    if (Restart != null)
                    {
                        Restart.Invoke(currentInfo);
                    }
                    break;

                case ServerDownloadStatus.PendingInstall:
                    if (Restart != null)
                    {
                        Restart.Invoke(currentInfo);
                    }
                    break;
            }
        }
    }
}
