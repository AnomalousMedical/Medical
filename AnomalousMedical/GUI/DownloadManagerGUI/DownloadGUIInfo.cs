using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    abstract class DownloadGUIInfo
    {
        public delegate void DescriptionFoundCallback(String caption, DownloadGUIInfo source);

        public DownloadGUIInfo(ServerDownloadStatus status)
        {
            this.Status = status;
        }

        public String Name { get; set; }

        public String ImageKey { get; set; }

        public ServerDownloadStatus Status { get; set; }

        public ButtonGridItem GUIItem { get; set; }

        public abstract void getDescription(DescriptionFoundCallback descriptionFoundCallback);
    }
}
