using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    enum DownloadGUIPanel
    {
        InstallPanel,
        UninstallPanel,
        RestartPanel,
        DownloadingPanel
    }

    abstract class DownloadGUIInfo
    {
        public delegate void DescriptionFoundCallback(String caption, DownloadGUIInfo source);

        public DownloadGUIInfo(DownloadGUIPanel panel, ServerDownloadStatus status)
        {
            this.Panel = panel;
            this.Status = status;
        }

        public String Name { get; set; }

        public String ImageKey { get; set; }

        public DownloadGUIPanel Panel { get; protected set; }

        public ServerDownloadStatus Status { get; set; }

        public ButtonGridItem GUIItem { get; set; }

        public abstract void getDescription(DescriptionFoundCallback descriptionFoundCallback);
    }
}
