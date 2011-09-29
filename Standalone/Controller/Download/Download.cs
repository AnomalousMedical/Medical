using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class Download
    {
        public Download()
        {
            
        }

        /// <summary>
        /// This method will be called by the download thread when the download
        /// is completed. It will be executed on the download background thread
        /// and should use ThreadManager.invoke to call back to the main UI.
        /// </summary>
        /// <param name="success"></param>
        public abstract void completed(bool success);

        /// <summary>
        /// This method will be called by the download thread when there is a
        /// status update. It will be executed on the download background thread
        /// and should use ThreadManager.invoke to call back to the main UI.
        /// </summary>
        public abstract void updateStatus();

        public abstract String DestinationFolder { get; }

        public abstract String Type { get; }

        public abstract String AdditionalArgs { get; }

        public long TotalSize { get; set; }

        public long TotalRead { get; set; }

        public String FileName { get; set; }

        public Object UserObject { get; set; }

        public bool Successful { get; protected set; }
    }
}
