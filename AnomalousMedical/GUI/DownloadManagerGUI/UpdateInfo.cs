using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class UpdateInfo : DownloadGUIInfo
    {
        private String description;

        public UpdateInfo(String imageKey, String name, String description, ServerDownloadStatus status, bool autoStartUpdate)
            :base(status)
        {
            this.ImageKey = imageKey;
            this.Name = name;
            this.description = description;
            this.AutoStartUpdate = autoStartUpdate;
        }

        public override void getDescription(DescriptionFoundCallback descriptionFoundCallback)
        {
            descriptionFoundCallback.Invoke(description, this);
        }
    }
}
