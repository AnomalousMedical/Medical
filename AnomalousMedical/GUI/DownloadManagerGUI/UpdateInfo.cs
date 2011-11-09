using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class UpdateInfo : DownloadGUIInfo
    {
        private String description;

        public UpdateInfo(String imageKey, String name, String description, ServerDownloadStatus status)
            :base(DownloadGUIPanel.RestartPanel, status)
        {
            this.ImageKey = imageKey;
            this.Name = name;
            this.description = description;
        }

        public override void getDescription(DescriptionFoundCallback descriptionFoundCallback)
        {
            descriptionFoundCallback.Invoke(description, this);
        }
    }
}
