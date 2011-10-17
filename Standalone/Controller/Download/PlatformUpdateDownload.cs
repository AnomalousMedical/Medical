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

        public override string Type
        {
            get
            {
                return "PlatformUpdate";
            }
        }

        public override string AdditionalArgs
        {
            get
            {
                return "osId=" + (int)PlatformConfig.OsId;
            }
        }
    }
}
