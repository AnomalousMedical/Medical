using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    abstract class ServerDownloadInfo : DownloadListener
    {
        private const float BYTES_TO_MEGABYTES = 9.53674316e-7f;

        private DownloadUIDisplay uiDisplay;

        public void startDownload(DownloadController downloadController, DownloadUIDisplay uiDisplay)
        {
            this.uiDisplay = uiDisplay;
            doStartDownload(downloadController);
        }

        protected abstract void doStartDownload(DownloadController downloadController);

        public String Name { get; set; }

        public String ImageKey { get; set; }

        public Download Download { get; protected set; }

        public Object UserObject { get; set; }

        #region DownloadListener Members

        public void updateStatus(Download download)
        {
            if (uiDisplay != null)
            {
                String statusString = String.Format("{0} - {1}%\n{2} of {3} (MB)", Name, (int)((float)download.TotalRead / download.TotalSize * 100.0f), (download.TotalRead * BYTES_TO_MEGABYTES).ToString("N2"), (download.TotalSize * BYTES_TO_MEGABYTES).ToString("N2"));
                uiDisplay.updateStatus(this, statusString);
            }
        }

        public void downloadCompleted(Download download)
        {
            if (download.Successful)
            {
                if (uiDisplay != null)
                {
                    uiDisplay.downloadSuccessful(this);
                }
            }
            else
            {
                if (download.Cancel)
                {
                    if (uiDisplay != null)
                    {
                        uiDisplay.downloadCanceled(this);
                    }
                }
                else
                {
                    if (uiDisplay != null)
                    {
                        uiDisplay.downloadFailed(this);
                    }
                }
            }
        }

        #endregion
    }
}
