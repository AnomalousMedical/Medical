using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine;
using Medical.Controller;
using MyGUIPlugin;

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
        private String fontName = "TimelineText";
        private int fontHeight = 14;
        private TextualAlignment textAlign = TextualAlignment.LeftTop;

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
                    textDisplay.Position = position;
                    textDisplay.Size = size;
                    textDisplay.FontName = fontName;
                    textDisplay.FontHeight = fontHeight;
                    textDisplay.TextAlign = textAlign;
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
                    textDisplay.Position = position;
                    textDisplay.Size = size;
                    textDisplay.FontName = fontName;
                    textDisplay.FontHeight = fontHeight;
                    textDisplay.TextAlign = textAlign;
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

        public String FontName
        {
            get
            {
                return fontName;
            }
            set
            {
                fontName = value;
                if (textDisplay != null)
                {
                    textDisplay.FontName = fontName;
                }
            }
        }

        public int FontHeight
        {
            get
            {
                return fontHeight;
            }
            set
            {
                fontHeight = value;
                if (textDisplay != null)
                {
                    textDisplay.FontHeight = fontHeight;
                }
            }
        }

        public TextualAlignment TextAlign
        {
            get
            {
                return textAlign;
            }
            set
            {
                textAlign = value;
                if (textDisplay != null)
                {
                    textDisplay.TextAlign = textAlign;
                }
            }
        }

        public String CameraName { get; set; }

        #region Saving

        private static String TEXT = "Text";
        private static String POSITION = "Position";
        private static String SIZE = "Size";
        private static String CAMERA_NAME = "CameraName";
        private static String FONT_NAME = "FontName";
        private static String FONT_HEIGHT = "FontHeight";
        private static String TEXT_ALIGN = "TextAlign";

        protected ShowTextAction(LoadInfo info)
            :base(info)
        {
            text = info.GetString(TEXT, text);
            position = info.GetValue<Vector2>(POSITION);
            size = info.GetValue<Size2>(SIZE);
            CameraName = info.GetValue(CAMERA_NAME, CameraName);
            fontName = info.GetValue(FONT_NAME, fontName);
            fontHeight = info.GetValue(FONT_HEIGHT, fontHeight);
            textAlign = info.GetValue<TextualAlignment>(TEXT_ALIGN, textAlign);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(TEXT, text);
            info.AddValue(POSITION, position);
            info.AddValue(SIZE, size);
            info.AddValue(CAMERA_NAME, CameraName);
            info.AddValue(FONT_NAME, fontName);
            info.AddValue(FONT_HEIGHT, fontHeight);
            info.AddValue(TEXT_ALIGN, textAlign);
        }

        #endregion
    }
}
