using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public enum ServerDownloadStatus
    {
        NotInstalled,
        Update
    }

    abstract class ServerDownloadInfo : DownloadListener
    {
        public delegate void UpdateStatusDelegate(ServerDownloadInfo downloadInfo, String status);
        public delegate void ServerDownloadInfoDelegate(ServerDownloadInfo downloadInfo);
        public delegate void RequestRestartDelegate(ServerDownloadInfo downloadInfo, String restartMessage, bool startPlatformUpdate);

        public event UpdateStatusDelegate UpdateStatus;
        public event ServerDownloadInfoDelegate DownloadSuccessful;
        public event ServerDownloadInfoDelegate DownloadFailed;
        public event ServerDownloadInfoDelegate DownloadCanceled;
        public event RequestRestartDelegate RequestRestart;

        private const float BYTES_TO_MEGABYTES = 9.53674316e-7f;

        public ServerDownloadInfo(ServerDownloadStatus status)
        {
            Status = status;
            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In ut urna ac orci posuere gravida. Sed vel purus magna, sed iaculis nisl. Donec a rhoncus nulla. Fusce consequat faucibus nulla quis dictum. Duis rutrum eros sit amet leo suscipit rhoncus lobortis arcu egestas. Quisque varius volutpat dui eu dictum. Morbi fringilla dolor ut tortor mattis tempor viverra nunc ullamcorper. Donec ac tempor tortor. Maecenas a egestas quam. Maecenas pharetra consectetur accumsan. Aliquam erat volutpat. Mauris quis nibh mi, id faucibus eros. Nullam porta urna a magna lacinia tristique. Nullam malesuada, nisi quis scelerisque scelerisque, risus nisi egestas tellus, sed porttitor lacus eros vel nisi. Duis augue magna, egestas ut suscipit ac, egestas in est.\n\nSed eget turpis velit. Vestibulum sit amet ultricies erat. Fusce eu auctor diam. Duis nec augue eu nulla consectetur dapibus. Nunc placerat vulputate vulputate. Nullam sagittis, enim et tempor euismod, est justo vehicula tortor, eu rutrum sapien risus ut ligula. Curabitur et turpis purus. Integer ornare ullamcorper erat, quis mollis ligula eleifend sit amet. Nulla vulputate sodales ornare. Aenean non metus lacinia ligula cursus ullamcorper sit amet nec metus.\n\nDuis viverra neque eu arcu adipiscing vestibulum. Cras consequat dignissim tellus, in molestie dui euismod eu. Pellentesque purus elit, ultricies dignissim ultrices a, commodo sit amet tortor. Phasellus gravida lorem nec augue tempor viverra. Sed imperdiet mattis lectus non tempor. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nullam neque nisl, blandit quis luctus in, cursus et lacus. Praesent dignissim neque quis metus cursus cursus. Etiam tincidunt condimentum dui at ornare.\n\nIn ac est nibh, tempor imperdiet diam. Sed dapibus suscipit dolor, eu tincidunt eros consectetur ac. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Quisque nunc tortor, dapibus at congue a, interdum non arcu. In fermentum posuere risus ac iaculis. Sed id lorem lorem. Donec arcu dolor, eleifend vitae pretium ac, tempus ac eros. Pellentesque tempus porttitor metus ut venenatis. Sed interdum lectus eget arcu lacinia sollicitudin. Suspendisse potenti. In et metus ligula, vel pharetra odio. Nunc enim lectus, sollicitudin eget imperdiet quis, egestas vel arcu. Maecenas ultrices mauris nec purus sodales fermentum. Sed id ipsum a elit cursus luctus. Ut non blandit erat. Ut vel velit arcu, id tincidunt sem.\n\nAliquam lacinia, mauris non lobortis dignissim, nibh lectus tempor tellus, sit amet laoreet massa diam sed arcu. Mauris rutrum consequat ultrices. Quisque commodo lacinia felis, at dictum arcu convallis non. Sed eget lacus risus, vitae suscipit nisl. Mauris ac metus dui. Aliquam nisl dui, adipiscing eu mollis vel, eleifend nec ligula. Duis ultrices, ipsum vel condimentum ultricies, mauris enim rutrum lacus, at convallis arcu mi in nulla. Integer interdum egestas nunc et pretium. Sed nec turpis neque. Phasellus adipiscing molestie felis eget laoreet.";
        }

        public void startDownload(DownloadController downloadController)
        {
            StatusString = String.Format("{0} - {1}", Name, "Starting Download");
            doStartDownload(downloadController);
        }

        protected abstract void doStartDownload(DownloadController downloadController);

        /// <summary>
        /// Override this to create uninstall info for this download. Can be null.
        /// </summary>
        /// <returns>UninstallInfo for this download or null if there is none.</returns>
        public virtual UninstallInfo createUninstallInfo()
        {
            return null;
        }

        public String Name { get; set; }

        public String ImageKey { get; set; }

        public Download Download { get; protected set; }

        public Object UserObject { get; set; }

        public ServerDownloadStatus Status { get; set; }

        public String StatusString { get; set; }

        public String Description { get; set; }

        #region DownloadListener Members

        public void updateStatus(Download download)
        {
            if (UpdateStatus != null)
            {
                StatusString = String.Format("{0} - {1}%\n{2} of {3} (MB)", Name, (int)((float)download.TotalRead / download.TotalSize * 100.0f), (download.TotalRead * BYTES_TO_MEGABYTES).ToString("N2"), (download.TotalSize * BYTES_TO_MEGABYTES).ToString("N2"));
                UpdateStatus.Invoke(this, StatusString);
            }
        }

        public virtual void downloadCompleted(Download download)
        {
            if (download.Successful)
            {
                if (DownloadSuccessful != null)
                {
                    DownloadSuccessful.Invoke(this);
                }
            }
            else
            {
                if (download.Cancel)
                {
                    if (DownloadCanceled != null)
                    {
                        DownloadCanceled.Invoke(this);
                    }
                }
                else
                {
                    if (DownloadFailed != null)
                    {
                        DownloadFailed.Invoke(this);
                    }
                }
            }
        }

        protected void requestRestart(String restartMessage, bool startPlatformUpdate)
        {
            if (RequestRestart != null)
            {
                RequestRestart.Invoke(this, restartMessage, startPlatformUpdate);
            }
        }

        #endregion
    }
}
