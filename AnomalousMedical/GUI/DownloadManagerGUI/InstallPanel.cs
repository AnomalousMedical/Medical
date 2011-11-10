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
        private Button moreInfoButton;

        private DownloadGUIInfo currentInfo;

        public InstallPanel(Widget widget)
        {
            this.widget = widget;

            actionButton = (Button)widget.findWidget("InstallButton");
            actionButton.MouseButtonClick += new MyGUIEvent(actionButton_MouseButtonClick);

            moreInfoButton = (Button)widget.findWidget("MoreInfoButton");
            moreInfoButton.MouseButtonClick += new MyGUIEvent(moreInfoButton_MouseButtonClick);

            icon = (StaticImage)widget.findWidget("InstallIcon");
            nameText = (Edit)widget.findWidget("InstallName");
            descriptionText = (Edit)widget.findWidget("InstallDescription");
        }

        public void setDownloadInfo(DownloadGUIInfo info)
        {
            currentInfo = info;
            icon.setItemResource(info.ImageKey);
            nameText.OnlyText = info.Name;
            nameText.TextCursor = 0;
            descriptionText.OnlyText = "Loading information from server...";
            moreInfoButton.Visible = info.MoreInfoURL != null;
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

                case ServerDownloadStatus.Unlicensed:
                    actionButton.Caption = "Uninstall";
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
                    descriptionText.OnlyText = caption;
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

                case ServerDownloadStatus.Unlicensed:
                    if (UninstallItem != null)
                    {
                        UninstallItem.Invoke(currentInfo);
                    }
                    break;
            }
        }

        void moreInfoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser(currentInfo.MoreInfoURL);
        }
    }
}
