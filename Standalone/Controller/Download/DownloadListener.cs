using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface DownloadListener
    {
        void updateStatus(Download download);

        void downloadCompleted(Download download);
    }
}
