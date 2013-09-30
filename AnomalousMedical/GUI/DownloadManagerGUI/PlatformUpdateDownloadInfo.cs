using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Medical.Controller;

namespace Medical.GUI
{
    class PlatformUpdateDownloadInfo : ServerDownloadInfo
    {
        private DownloadManagerServer server;
        private String description = null;
        private bool readingDescriptionFromServer = false;

        public PlatformUpdateDownloadInfo(Version version, DownloadManagerServer server)
            : base(ServerDownloadStatus.Update)
        {
            Name = "Anomalous Medical version " + version;
            this.Version = version;
            this.ImageKey = "AnomalousMedicalCore/UpdateImage";
            this.server = server;
        }

        protected override void doStartDownload(DownloadController downloadController)
        {
            this.Download = downloadController.downloadPlatformUpdate(Version, this);
        }

        public override void downloadCompleted(Medical.Download download)
        {
            if (!download.Cancel)
            {
                requestRestart("Please restart to install the Anomalous Medical update.", true);
            }
            base.downloadCompleted(download);
        }

        public override DownloadGUIInfo createClientDownloadInfo()
        {
            return new UpdateInfo(ImageKey, Name, "Please restart to install the Anomalous Medical update.", ServerDownloadStatus.PendingInstall, true);
        }

        public Version Version { get; private set; }

        public override void getDescription(DescriptionFoundCallback descriptionFoundCallback)
        {
            if (description != null)
            {
                descriptionFoundCallback.Invoke(description, this);
            }
            else if (!readingDescriptionFromServer)
            {
                readingDescriptionFromServer = true;
                Thread descriptionReadThread = new Thread(delegate()
                {
                    description = server.readPlatformLicenseFromServer();
                    if (description != null)
                    {
                        ThreadManager.invoke(new Action(delegate()
                        {
                            descriptionFoundCallback.Invoke(description, this);
                        }));
                    }
                    else
                    {
                        readingDescriptionFromServer = false;
                        ThreadManager.invoke(new Action(delegate()
                        {
                            descriptionFoundCallback.Invoke("There was an error reading this license from the download server. You are still bound to the license. Please visit www.anomalousmedical.com for more information.\n\nYou can click this item again to attempt to read the license off the server again.", this);
                        }));
                    }
                });
                descriptionReadThread.Start();
            }
        }
    }
}
