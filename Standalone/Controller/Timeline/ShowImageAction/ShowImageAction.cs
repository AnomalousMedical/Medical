using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine;

namespace Medical
{
    [TimelineActionProperties("Show Image", 31 / 255f, 73 / 255f, 125 / 255f, GUIType = typeof(Medical.GUI.ShowImageProperties))]
    class ShowImageAction : TimelineAction
    {
        private bool finished = false;
        private IImageDisplay imageDisplay;
        private String imageFile;
        private Size2 size;
        private Vector2 position;
        private bool keepAspectRatio = true;

        public ShowImageAction()
        {
            size = new Size2(0.2f, 0.2f);
            position = new Vector2(0.0f, 0.0f);
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
                else
                {
                    imageDisplay.KeepAspectRatio = keepAspectRatio;
                    imageDisplay.Position = position;
                    imageDisplay.Size = size;
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
                if (imageDisplay != null)
                {
                    imageDisplay.KeepAspectRatio = keepAspectRatio;
                    imageDisplay.Position = position;
                    imageDisplay.Size = size;
                }
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

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                if(imageDisplay != null)
                {
                    imageDisplay.Position = position;
                }
            }
        }

        public Size2 Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                if (imageDisplay != null)
                {
                    imageDisplay.Size = size;
                }
            }
        }

        public bool KeepAspectRatio
        {
            get
            {
                return keepAspectRatio;
            }
            set
            {
                keepAspectRatio = value;
                if (imageDisplay != null)
                {
                    imageDisplay.KeepAspectRatio = keepAspectRatio;
                }
            }
        }

        #region Saving

        private static String IMAGE_FILE = "ImageFile";
        private static String POSITION = "Position";
        private static String SIZE = "Size";
        private static String KEEP_ASPECT_RATIO = "KeepAspectRatio";

        protected ShowImageAction(LoadInfo info)
            :base(info)
        {
            imageFile = info.GetString(IMAGE_FILE, imageFile);
            position = info.GetValue<Vector2>(POSITION);
            size = info.GetValue<Size2>(SIZE);
            keepAspectRatio = info.GetValue(KEEP_ASPECT_RATIO, keepAspectRatio);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(IMAGE_FILE, imageFile);
            info.AddValue(POSITION, position);
            info.AddValue(SIZE, size);
            info.AddValue(KEEP_ASPECT_RATIO, keepAspectRatio);
        }

        #endregion
    }
}
