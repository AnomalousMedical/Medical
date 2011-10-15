using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class PlatformUpdateDownload : Download
    {
        public PlatformUpdateDownload(DownloadController downloadController, DownloadListener downloadListener)
            :base(downloadController, downloadListener)
        {
            this.DestinationFolder = MedicalConfig.SafeDownloadFolder;
        }

        protected override void onCompleted(bool success)
        {
            
        }

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
