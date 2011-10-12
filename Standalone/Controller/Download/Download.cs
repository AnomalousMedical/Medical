using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;

namespace Medical
{
    public enum DownloadPostAction
    {
        DeleteFile
    }

    public abstract class Download
    {
        private DownloadListener listener;
        protected DownloadController controller;

        public Download(DownloadController controller, DownloadListener listener)
        {
            this.controller = controller;
            this.listener = listener;
            this.DownloadedToSafeLocation = false;
        }

        public void completed(bool success)
        {
            controller._alertDownloadCompleted(this);
            this.Successful = success;
            onCompleted(success);
            ThreadManager.invoke(new Action(delegate()
            {
                listener.downloadCompleted(this);
            }));
        }

        /// <summary>
        /// This method will be called by the download thread when the download
        /// is completed. It will be executed on the download background thread
        /// and should use ThreadManager.invoke to call back to the main UI.
        /// </summary>
        /// <param name="success"></param>
        protected abstract void onCompleted(bool success);

        /// <summary>
        /// This method will be called by the download thread when there is a
        /// status update. It will be executed on the download background thread
        /// and should use ThreadManager.invoke to call back to the main UI.
        /// </summary>
        public void updateStatus()
        {
            ThreadManager.invoke(new Action(delegate()
            {
                listener.updateStatus(this);
            }));
        }

        public void cancelDownload(DownloadPostAction postAction)
        {
            CancelPostAction = postAction;
            Cancel = true;
        }

        public abstract String Type { get; }

        public abstract String AdditionalArgs { get; }

        public String DestinationFolder { get; set; }

        public long TotalSize { get; set; }

        public long TotalRead { get; set; }

        public String FileName { get; set; }

        public Object UserObject { get; set; }

        public bool Successful { get; private set; }

        public bool Cancel { get; private set; }

        public bool DownloadedToSafeLocation { get; internal set; }

        public DownloadPostAction CancelPostAction { get; set; }
    }
}
