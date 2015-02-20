﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Medical.Controller;

namespace Medical.GUI
{
    public enum ServerDownloadStatus
    {
        NotInstalled,
        Update,
        Downloading,
        PendingUninstall,
        PendingInstall,
        Installed,
        Unlicensed,
        Removed, //Means that the plugin has been removed and should be removed from the list of plugins
    }

    abstract class ServerDownloadInfo : DownloadGUIInfo, DownloadListener
    {
        private const float BYTES_TO_MEGABYTES = 9.53674316e-7f;
        private const double KB_IN_MB = 1024.0;

        public delegate void UpdateStatusDelegate(ServerDownloadInfo downloadInfo, String status);
        public delegate void ServerDownloadInfoDelegate(ServerDownloadInfo downloadInfo);
        public delegate void RequestRestartDelegate(ServerDownloadInfo downloadInfo, String restartMessage, bool startPlatformUpdate);

        public event UpdateStatusDelegate UpdateStatus;
        public event ServerDownloadInfoDelegate DownloadSuccessful;
        public event ServerDownloadInfoDelegate DownloadFailed;
        public event ServerDownloadInfoDelegate DownloadCanceled;
        public event RequestRestartDelegate RequestRestart;

        private ServerDownloadStatus constructedStatus;

        public ServerDownloadInfo(ServerDownloadStatus status)
            :base(status)
        {
            constructedStatus = status;
        }

        public void startDownload(DownloadController downloadController)
        {
            Status = ServerDownloadStatus.Downloading;
            StatusString = String.Format("{0}\n{1}", Name, "Starting Download");
            doStartDownload(downloadController);
        }

        protected abstract void doStartDownload(DownloadController downloadController);

        /// <summary>
        /// Override this to create uninstall info for this download. Can be null.
        /// </summary>
        /// <returns>UninstallInfo for this download or null if there is none.</returns>
        public virtual DownloadGUIInfo createClientDownloadInfo()
        {
            return null;
        }

        /// <summary>
        /// Tests if this download depends on testAsDependency. If true downloading this object's file
        /// requires a download of testAsDependency.
        /// </summary>
        /// <param name="testAsDependency">The download to test as a dependency.</param>
        /// <returns>True if this download also needs testAsDependency downloaded.</returns>
        public virtual bool dependsOn(ServerDownloadInfo testAsDependency)
        {
            return false;
        }

        /// <summary>
        /// Determine if this download is for a given dependency plugin id.
        /// </summary>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        public virtual bool shouldAutoDownlaod(IEnumerable<long> autoDownloadIds)
        {
            return false;
        }

        public Download Download { get; protected set; }

        public String StatusString { get; set; }

        #region DownloadListener Members

        public void updateStatus(Download download)
        {
            if (UpdateStatus != null)
            {
                if (download.TotalSize > 0)
                {
                    double downloadSpeed = download.DownloadSpeed;
                    String downloadSpeedPretty;
                    if (downloadSpeed > KB_IN_MB)
                    {
                        downloadSpeedPretty = String.Format("{0} mb", (downloadSpeed / KB_IN_MB).ToString("N2"));
                    }
                    else
                    {
                        downloadSpeedPretty = String.Format("{0} kb", downloadSpeed.ToString("N2"));
                    }
                    StatusString = String.Format("{0}\n{4} - {1}%\n{2} of {3} (MB) at {5} per second", Name, (int)((float)download.TotalRead / download.TotalSize * 100.0f), (download.TotalRead * BYTES_TO_MEGABYTES).ToString("N2"), (download.TotalSize * BYTES_TO_MEGABYTES).ToString("N2"), download.StatusString, downloadSpeedPretty);
                }
                else
                {
                    StatusString = String.Format("{0}\n{1}", Name, download.StatusString);
                }
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
                Status = constructedStatus;
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
