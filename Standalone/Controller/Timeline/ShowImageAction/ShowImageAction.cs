using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine;
using Medical.Controller;
using Medical.Editor;
using Engine.Editing;

namespace Medical
{
    [TimelineActionProperties("Show Image")]
    public class ShowImageAction : TimelineAction
    {
        private bool finished = false;
        private IImageDisplay imageDisplay;
        private String imageFile;
        private Size2 size;
        private Vector2 position;
        private bool keepAspectRatio = true;
        private ImageAlignment alignment;

        public ShowImageAction()
        {
            size = new Size2(0.2f, 0.2f);
            position = new Vector2(0.0f, 0.0f);
            alignment = ImageAlignment.Specify;
        }

        public override void started(float timelineTime, Clock clock)
        {
            if (imageFile != null)
            {
                finished = false;
                imageDisplay = TimelineController.showImage(imageFile, CameraName);
                if (imageDisplay == null)
                {
                    finished = true;
                }
                else
                {
                    setupImageDisplay();
                }
            }
            else
            {
                finished = true;
            }
        }

        public override void skipTo(float timelineTime)
        {
            if (timelineTime <= EndTime)
            {
                started(timelineTime, null);
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

        public override void capture()
        {
            SceneViewWindow currentWindow = TimelineController.SceneViewController.ActiveWindow;
            CameraName = currentWindow.Name;
        }

        public override void editing()
        {
            if (imageFile != null)
            {
                imageDisplay = TimelineController.showImage(imageFile, CameraName);
                if (imageDisplay != null)
                {
                    setupImageDisplay();
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

        public override void findFileReference(TimelineStaticInfo info)
        {
            if (info.matchesPattern(ImageFile))
            {
                info.addMatch(this.GetType(), "Image file reference", ImageFile);
            }
        }

        public override bool Finished
        {
            get
            {
                return finished;
            }
        }

        [EditableFile("*.png", "Choose Image File")]
        public String ImageFile
        {
            get
            {
                return imageFile;
            }
            set
            {
                editingCompleted();
                imageFile = value;
                editing();
            }
        }

        [Editable]
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

        [Editable]
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

        [Editable]
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

        [Editable]
        public ImageAlignment Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                alignment = value;
                if (imageDisplay != null)
                {
                    imageDisplay.Alignment = alignment;
                }
            }
        }

        [Editable]
        public String CameraName { get; set; }

        private void setupImageDisplay()
        {
            imageDisplay.SuppressLayout = true;
            imageDisplay.KeepAspectRatio = keepAspectRatio;
            imageDisplay.Position = position;
            imageDisplay.Size = size;
            imageDisplay.Alignment = alignment;
            imageDisplay.SuppressLayout = false;
            imageDisplay.layout();
        }

        #region Saving

        private static String IMAGE_FILE = "ImageFile";
        private static String POSITION = "Position";
        private static String SIZE = "Size";
        private static String KEEP_ASPECT_RATIO = "KeepAspectRatio";
        private static String CAMERA_NAME = "CameraName";
        private static String IMAGE_ALIGNMENT = "ImageAlignment";

        protected ShowImageAction(LoadInfo info)
            :base(info)
        {
            imageFile = info.GetString(IMAGE_FILE, imageFile);
            position = info.GetValue<Vector2>(POSITION);
            size = info.GetValue<Size2>(SIZE);
            keepAspectRatio = info.GetValue(KEEP_ASPECT_RATIO, keepAspectRatio);
            CameraName = info.GetValue(CAMERA_NAME, CameraName);
            alignment = info.GetValue<ImageAlignment>(IMAGE_ALIGNMENT, ImageAlignment.Specify);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(IMAGE_FILE, imageFile);
            info.AddValue(POSITION, position);
            info.AddValue(SIZE, size);
            info.AddValue(KEEP_ASPECT_RATIO, keepAspectRatio);
            info.AddValue(CAMERA_NAME, CameraName);
            info.AddValue(IMAGE_ALIGNMENT, alignment);
        }

        #endregion
    }
}
