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
        private ImageBox installIcon;
        private EditBox installName;
        private ServerDownloadInfo currentInfo = null;
        private ProgressBar downloadProgressBar;
        private TextBox downloadStatusText;

        public DownloadingPanel(Widget widget)
        {
            this.widget = widget;

            Button cancelButton = (Button)widget.findWidget("CancelButton");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            installIcon = (ImageBox)widget.findWidget("DownloadingIcon");
            installName = (EditBox)widget.findWidget("DownloadingName");
            downloadProgressBar = (ProgressBar)widget.findWidget("DownloadProgressBar");
            downloadStatusText = (TextBox)widget.findWidget("DownloadingStatusText");
        }

        public void setDownloadInfo(ServerDownloadInfo info)
        {
            if (currentInfo != null)
            {
                unsubscribeFromDownload(currentInfo);
            }
            installIcon.setItemResource(info.ImageKey);
            installName.Caption = info.Name;
            installName.TextCursor = 0;
            Download download = info.Download;
            if (download != null)
            {
                setDownloadProgress(download, info.StatusString);
            }
            else
            {
                downloadProgressBar.Position = 0;
                downloadStatusText.Caption = "Downloading";
            }
            currentInfo = info;
            subscribeToDownload(currentInfo);
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

        private void setDownloadProgress(Download download, String status)
        {
            downloadStatusText.Caption = status;
            downloadProgressBar.Position = (uint)((float)download.TotalRead / download.TotalSize * 100.0f);
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (CancelDownload != null)
            {
                CancelDownload.Invoke(this, e);
            }
        }

        void downloadInfo_UpdateStatus(ServerDownloadInfo downloadInfo, string status)
        {
            Download download = downloadInfo.Download;
            setDownloadProgress(download, status);
        }

        void downloadInfo_DownloadSuccessful(ServerDownloadInfo downloadInfo)
        {
            unsubscribeFromDownload(downloadInfo);
        }

        void downloadInfo_DownloadFailed(ServerDownloadInfo downloadInfo)
        {
            unsubscribeFromDownload(downloadInfo);
        }

        void downloadInfo_DownloadCanceled(ServerDownloadInfo downloadInfo)
        {
            unsubscribeFromDownload(downloadInfo);
        }

        private void subscribeToDownload(ServerDownloadInfo downloadInfo)
        {
            downloadInfo.DownloadCanceled += downloadInfo_DownloadCanceled;
            downloadInfo.DownloadFailed += downloadInfo_DownloadFailed;
            downloadInfo.DownloadSuccessful += downloadInfo_DownloadSuccessful;
            downloadInfo.UpdateStatus += downloadInfo_UpdateStatus;
        }

        private void unsubscribeFromDownload(ServerDownloadInfo downloadInfo)
        {
            downloadInfo.DownloadCanceled -= downloadInfo_DownloadCanceled;
            downloadInfo.DownloadFailed -= downloadInfo_DownloadFailed;
            downloadInfo.DownloadSuccessful -= downloadInfo_DownloadSuccessful;
            downloadInfo.UpdateStatus -= downloadInfo_UpdateStatus;
        }
    }
}
