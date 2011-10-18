using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class PlatformUpdateDownloadInfo : ServerDownloadInfo
    {
        public PlatformUpdateDownloadInfo(Version version)
            : base(ServerDownloadStatus.Update)
        {
            Name = "Anomalous Platform version " + version;
            this.Version = version;
            this.ImageKey = "AnomalousMedicalCore/UpdateImage";
        }

        protected override void doStartDownload(DownloadController downloadController)
        {
            this.Download = downloadController.downloadPlatformUpdate(Version, this);
        }

        public Version Version { get; private set; }
    }
}
