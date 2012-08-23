using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    class PlatformUpdateDownload : Download
    {
        public PlatformUpdateDownload(Version version, DownloadController downloadController, DownloadListener downloadListener)
            :base(downloadController, downloadListener)
        {
            this.DestinationFolder = MedicalConfig.SafeDownloadFolder;
            this.Version = version;
        }

        protected override void onCompleted(bool success)
        {
            if (success)
            {
                UpdateController.writeUpdateIndex(Path.Combine(this.DestinationFolder, this.FileName), Version);
            }
        }

        public Version Version { get; private set; }

        public override DownloadType Type
        {
            get
            {
                return DownloadType.PlatformUpdate;
            }
        }

        public override string IdName
        {
            get
            {
                return "OsId";
            }
        }

        public override string Id
        {
            get
            {
                return ((int)PlatformConfig.OsId).ToString();
            }
        }
    }
}
