using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine;
using Medical.Controller;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical
{
    public class ShowTextAction : TimelineAction
    {
        private bool finished = false;
        private ITextDisplay textDisplay;
        private String text = "";
        private Size2 size;
        private Vector2 position;
        private String fontName = "TimelineText";
        private int fontHeight = 21;
        private TextualAlignment textAlign = TextualAlignment.LeftTop;
        private Vector3 scenePoint = Vector3.Zero;
        private bool positionOnScenePoint = false;

        public ShowTextAction()
        {
            size = new Size2(1.0f, 1.0f);
            position = new Vector2(0.0f, 0.0f);
        }

        public void setSelectionColor(Color color)
        {
            if (textDisplay != null)
            {
                text = textDisplay.addColorToSelectedText(color);
                textDisplay.setText(text);
            }
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
                    textDisplay.ScenePoint = scenePoint;
                    textDisplay.PositionOnScenePoint = positionOnScenePoint;
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
                    textDisplay.ScenePoint = scenePoint;
                    textDisplay.PositionOnScenePoint = positionOnScenePoint;
                    textDisplay.Editable = true;
                    textDisplay.TextEdited += new EventDelegate<ITextDisplay, string>(textDisplay_TextEdited);
                }
            }
        }

        public override void editingCompleted()
        {
            if (textDisplay != null)
            {
                textDisplay.Dispose();
                textDisplay = null;
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

        [Editable]
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

        [EditableMinMax(0, 1, 0.05f)]
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

        [EditableMinMax(0, 1, 0.05f)]
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

        [Editable]
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

        [EditableMinMax(0, 1000, 1)]
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

        [Editable]
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

        [Editable]
        public Vector3 ScenePoint
        {
            get
            {
                return scenePoint;
            }
            set
            {
                scenePoint = value;
                if (textDisplay != null)
                {
                    textDisplay.ScenePoint = scenePoint;
                }
            }
        }

        [Editable]
        public bool PositionOnScenePoint
        {
            get
            {
                return positionOnScenePoint;
            }
            set
            {
                positionOnScenePoint = value;
                if (textDisplay != null)
                {
                    textDisplay.PositionOnScenePoint = positionOnScenePoint;
                }
            }
        }

        void textDisplay_TextEdited(ITextDisplay source, string arg)
        {
            text = arg;
        }

        [Editable]
        public String CameraName { get; set; }

        public enum CustomQueries
        {
            ChooseColor
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            editInterface.addCommand(new EditInterfaceCommand("Set Color", (callback, caller) =>
                {
                    callback.runCustomQuery<Color>(CustomQueries.ChooseColor, delegate(Color color, ref String message)
                    {
                        setSelectionColor(color);
                        return true;
                    });
                }));
        }

        public override string TypeName
        {
            get
            {
                return "Show Text";
            }
        }

        #region Saving

        private static String TEXT = "Text";
        private static String POSITION = "Position";
        private static String SIZE = "Size";
        private static String CAMERA_NAME = "CameraName";
        private static String FONT_NAME = "FontName";
        private static String FONT_HEIGHT = "FontHeight";
        private static String TEXT_ALIGN = "TextAlign";
        private static String SCENE_POINT = "ScenePoint";
        private static String POSITION_ON_SCENE_POINT = "PositionOnScenePoint";

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
            scenePoint = info.GetVector3(SCENE_POINT, scenePoint);
            positionOnScenePoint = info.GetBoolean(POSITION_ON_SCENE_POINT, positionOnScenePoint);
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
            info.AddValue(SCENE_POINT, scenePoint);
            info.AddValue(POSITION_ON_SCENE_POINT, positionOnScenePoint);
        }

        #endregion
    }
}
