using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Show Image", 31 / 255f, 73 / 255f, 125 / 255f, GUIType = typeof(Medical.GUI.ShowImageProperties))]
    class ShowImageAction : TimelineAction
    {
        private bool finished = false;
        private IImageDisplay imageDisplay;
        private String imageFile;

        public ShowImageAction()
        {

        }

        public override void started(float timelineTime, Clock clock)
        {
            if (imageFile != null)
            {
                finished = false;
                imageDisplay = TimelineController.showImage(imageFile);
                if (imageDisplay == null)
                {
                    finished = true;
                }
            }
            else
            {
                finished = true;
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            if (imageDisplay != null)
            {
                imageDisplay.Dispose();
                imageDisplay = null;
            }
        }

        public override void update(float timelineTime, Clock clock)
        {
            finished = timelineTime > StartTime + Duration;
        }

        public override void editing()
        {
            if (imageFile != null)
            {
                imageDisplay = TimelineController.showImage(imageFile);
            }
        }

        public override void editingCompleted()
        {
            if (imageDisplay != null)
            {
                imageDisplay.Dispose();
            }
        }

        public override bool Finished
        {
            get
            {
                return finished;
            }
        }

        public String ImageFile
        {
            get
            {
                return imageFile;
            }
            set
            {
                imageFile = value;
            }
        }

        #region Saving

        private static String IMAGE_FILE = "ImageFile";

        protected ShowImageAction(LoadInfo info)
            :base(info)
        {
            imageFile = info.GetString(IMAGE_FILE, imageFile);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(IMAGE_FILE, imageFile);
        }

        #endregion
    }
}
