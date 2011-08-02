using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine;
using Medical.Controller;

namespace Medical
{
    [TimelineActionProperties("Show Text")]
    public class ShowTextAction : TimelineAction
    {
        private bool finished = false;
        private ITextDisplay textDisplay;
        private String text;
        private Size2 size;
        private Vector2 position;
        private bool keepAspectRatio = true;

        public ShowTextAction()
        {
            size = new Size2(0.2f, 0.2f);
            position = new Vector2(0.0f, 0.0f);
        }

        public override void started(float timelineTime, Clock clock)
        {
            if (text != null)
            {
                finished = false;
                textDisplay = TimelineController.showText(text, CameraName);
                if (textDisplay == null)
                {
                    finished = true;
                }
                else
                {
                    textDisplay.KeepAspectRatio = keepAspectRatio;
                    textDisplay.Position = position;
                    textDisplay.Size = size;
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
            if (textDisplay != null)
            {
                textDisplay.Dispose();
                textDisplay = null;
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
            if (text != null)
            {
                textDisplay = TimelineController.showText(text, CameraName);
                if (textDisplay != null)
                {
                    textDisplay.KeepAspectRatio = keepAspectRatio;
                    textDisplay.Position = position;
                    textDisplay.Size = size;
                }
            }
        }

        public override void editingCompleted()
        {
            if (textDisplay != null)
            {
                textDisplay.Dispose();
            }
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        public override bool Finished
        {
            get
            {
                return finished;
            }
        }

        public String Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if (textDisplay != null)
                {
                    textDisplay.setText(text);
                }
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
                if(textDisplay != null)
                {
                    textDisplay.Position = position;
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
                if (textDisplay != null)
                {
                    textDisplay.Size = size;
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
                if (textDisplay != null)
                {
                    textDisplay.KeepAspectRatio = keepAspectRatio;
                }
            }
        }

        public String CameraName { get; set; }

        #region Saving

        private static String TEXT = "Text";
        private static String POSITION = "Position";
        private static String SIZE = "Size";
        private static String KEEP_ASPECT_RATIO = "KeepAspectRatio";
        private static String CAMERA_NAME = "CameraName";

        protected ShowTextAction(LoadInfo info)
            :base(info)
        {
            text = info.GetString(TEXT, text);
            position = info.GetValue<Vector2>(POSITION);
            size = info.GetValue<Size2>(SIZE);
            keepAspectRatio = info.GetValue(KEEP_ASPECT_RATIO, keepAspectRatio);
            CameraName = info.GetValue(CAMERA_NAME, CameraName);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(TEXT, text);
            info.AddValue(POSITION, position);
            info.AddValue(SIZE, size);
            info.AddValue(KEEP_ASPECT_RATIO, keepAspectRatio);
            info.AddValue(CAMERA_NAME, CameraName);
        }

        #endregion
    }
}
