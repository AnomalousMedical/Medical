using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    abstract class ServerDownloadInfo
    {
        public abstract void startDownload(DownloadController downloadController, DownloadListener downloadListener, Object callbackObject);

        public String Name { get; set; }

        public String ImageKey { get; set; }

        public Download Download { get; protected set; }
    }
}
