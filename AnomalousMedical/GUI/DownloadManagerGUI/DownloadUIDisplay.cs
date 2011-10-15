using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    interface DownloadUIDisplay
    {
        void updateStatus(ServerDownloadInfo downloadInfo, String status);
        void downloadSuccessful(ServerDownloadInfo downloadInfo);
        void downloadFailed(ServerDownloadInfo downloadInfo);
        void downloadCanceled(ServerDownloadInfo downloadInfo);
    }
}
