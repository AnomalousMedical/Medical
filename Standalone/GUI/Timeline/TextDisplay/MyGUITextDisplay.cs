using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MyGUIPlugin;
using System.Drawing;
using Engine;
using Medical.Controller;

namespace Medical.GUI
{
    class MyGUITextDisplay : Component, ITextDisplay
    {
        private EditBox textBox;
        private MyGUITextDisplayFactory textFactory;
        private Vector2 position = new Vector2();
        private Size2 size = new Size2();
        private SceneViewWindow sceneWindow;
        private String fontName;
        private Vector3 scenePoint;
        private bool positionOnScenePoint;

        public event EventDelegate<ITextDisplay, String> TextEdited;

        public MyGUITextDisplay(MyGUITextDisplayFactory textFactory, SceneViewWindow sceneWindow)
            :base("Medical.GUI.Timeline.TextDisplay.MyGUITextDisplay.layout")
        {
            this.textFactory = textFactory;
            this.sceneWindow = sceneWindow;
            sceneWindow.Resized += sceneWindow_Resized;

            widget.Visible = false;
            textBox = (EditBox)widget.findWidget("TextBox");
        }

        public override void Dispose()
        {
            sceneWindow.Resized -= sceneWindow_Resized;
            sceneWindow.RenderingStarted -= sceneWindow_RenderingStarted;
            textFactory.textDisposed(this);
            base.Dispose();
        }

        public String addColorToSelectedText(Engine.Color color)
        {
            String colorText = textBox.Caption;
            String defaultColor = textBox.TextColor.ToHexString();
            if (textBox.IsTextSelection)
            {
                int selectionStart = findPositionInColorizedString((int)textBox.TextSelectionStart, colorText);
                String currentColor = findColorForPosition(selectionStart, colorText, defaultColor);
                colorText = colorText.Insert(selectionStart, color.ToHexString());

                int selectionEnd = findPositionInColorizedString((int)textBox.TextSelectionEnd, colorText);
                colorText = colorText.Insert(selectionEnd, currentColor);
            }
            else
            {
                uint selection = textBox.TextCursor;
                colorText = colorText.Insert(findPositionInColorizedString((int)selection, colorText), color.ToHexString());
            }
            return colorText;
        }

        private static int findPositionInColorizedString(int position, String colorizedString)
        {
            if (colorizedString.Length == 0)
            {
                return 0;
            }
            int letterCount = position;
            int currentIndex = 0;
            char currentLetter;
            while (letterCount >= 0 && currentIndex < colorizedString.Length)
            {
                currentLetter = colorizedString[currentIndex];
                if (currentLetter == '#')
                {
                    //Found a # sign, spin to the next whitespace character.
                    currentLetter = colorizedString[currentIndex + 1];
                    if (currentLetter == '#')
                    {
                        //Skip one place for the # sign.
                        if (letterCount == 0)
                        {
                            ++currentIndex;
                        }
                        else
                        {
                            currentIndex += 2;
                        }
                        --letterCount;
                    }
                    else
                    {
                        //Skip 7 additional places for the color
                        currentIndex += 7;
                    }
                }
                else
                {
                    ++currentIndex;
                    --letterCount;
                }
            }
            return currentIndex - 1;
        }

        private static String findColorForPosition(int position, String colorText, String defaultColor)
        {
            char currentLetter;
            while (position >= 0)
            {
                currentLetter = colorText[position];
                if (currentLetter == '#')
                {
                    if (position + 1 < colorText.Length && colorText[position + 1] != '#')
                    {
                        //Have a color string
                        return colorText.Substring(position, 7);
                    }
                }
                --position;
            }
            return defaultColor;
        }

        public void setText(String text)
        {
            textBox.Caption = text;
        }

        public void show()
        {
            LayerManager.Instance.upLayerItem(widget);
            widget.Visible = true;
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
                positionText((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
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
                positionText((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
            }
        }

        public string FontName
        {
            get
            {
                return fontName;
            }
            set
            {
                fontName = value;
                textBox.Font = textFactory.getMyGUIFont(fontName, FontHeight);
            }
        }

        private int unscaledFontHeight = 0;
        public int FontHeight
        {
            get
            {
                return unscaledFontHeight;
            }
            set
            {
                textBox.Font = textFactory.getMyGUIFont(fontName, value);
                unscaledFontHeight = value;
                textBox.FontHeight = ScaleHelper.Scaled(value);
            }
        }

        public bool Editable
        {
            get
            {
                return !textBox.EditStatic;
            }
            set
            {
                textBox.EditStatic = !value;
                if (value)
                {
                    textBox.EventEditTextChange += textBox_EventEditTextChange;
                    textBox.NeedMouseFocus = true;
                }
                else
                {
                    textBox.EventEditTextChange -= textBox_EventEditTextChange;
                    textBox.NeedMouseFocus = false;
                }
            }
        }

        public Vector3 ScenePoint
        {
            get
            {
                return scenePoint;
            }
            set
            {
                scenePoint = value;
            }
        }

        public bool PositionOnScenePoint
        {
            get
            {
                return positionOnScenePoint;
            }
            set
            {
                if (positionOnScenePoint != value)
                {
                    positionOnScenePoint = value;
                    if (positionOnScenePoint)
                    {
                        sceneWindow.RenderingStarted += sceneWindow_RenderingStarted;
                    }
                    else
                    {
                        sceneWindow.RenderingStarted -= sceneWindow_RenderingStarted;
                        positionText((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
                    }
                }
            }
        }

        void textBox_EventEditTextChange(Widget source, EventArgs e)
        {
            if (TextEdited != null)
            {
                TextEdited.Invoke(this, textBox.Caption);
            }
        }

        public TextualAlignment TextAlign
        {
            get
            {
                return TextualAlignment.LeftTop;
            }
            set
            {
                switch (value)
                {
                    case TextualAlignment.LeftTop:
                        textBox.TextAlign = Align.Left | Align.Top;
                        break;
                    case TextualAlignment.LeftBottom:
                        textBox.TextAlign = Align.Left | Align.Bottom;
                        break;
                    case TextualAlignment.RightTop:
                        textBox.TextAlign = Align.Right | Align.Top;
                        break;
                    case TextualAlignment.RightBottom:
                        textBox.TextAlign = Align.Right | Align.Bottom;
                        break;
                    case TextualAlignment.TopCenter:
                        textBox.TextAlign = Align.HCenter | Align.Top;
                        break;
                    case TextualAlignment.BottomCenter:
                        textBox.TextAlign = Align.HCenter | Align.Bottom;
                        break;
                    case TextualAlignment.LeftCenter:
                        textBox.TextAlign = Align.Left | Align.VCenter;
                        break;
                    case TextualAlignment.RightCenter:
                        textBox.TextAlign = Align.Right | Align.VCenter;
                        break;
                    case TextualAlignment.Center:
                        textBox.TextAlign = Align.HCenter | Align.VCenter;
                        break;
                    default:
                        textBox.TextAlign = Align.Left | Align.Top;
                        break;
                }
            }
        }

        void sceneWindow_Resized(SceneViewWindow window)
        {
            positionText((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
        }

        private void positionText(int width, int height)
        {
            int left = (int)(position.x * width + sceneWindow.Location.x);
            int top = (int)(position.y * height + sceneWindow.Location.y);

            int newWidth = (int)(size.Width * width);
            int newHeight = (int)(size.Height * height);

            int right = left + newWidth;
            int windowRight = (int)(sceneWindow.Location.x + sceneWindow.WorkingSize.Width);
            if (right > windowRight)
            {
                left -= (int)(right - windowRight);
            }

            int bottom = top + newHeight;
            int windowBottom = (int)(sceneWindow.Location.y + sceneWindow.WorkingSize.Height);
            if (bottom > windowBottom)
            {
                top -= (int)(bottom - windowBottom);
            }

            widget.setPosition(left, top);
            widget.setSize(newWidth, newHeight);

            //Make sure the box is big enough for the text
            bool needsChange = false;
            Size2 textSize = textBox.getTextSize();
            if (textSize.Width > width)
            {
                textSize.Width = width;
            }
            if (textSize.Height > height)
            {
                textSize.Height = height;
            }
            if (newWidth < textSize.Width)
            {
                needsChange = true;
                //left -= (int)(textSize.Width - newWidth);
                newWidth = (int)textSize.Width;
            }
            if (newHeight < textSize.Height)
            {
                needsChange = true;
                //top -= (int)(textSize.Height - newHeight);
                newHeight = (int)textSize.Height;
            }

            if (needsChange)
            {
                widget.setPosition(left, top);
                widget.setSize(newWidth, newHeight);
            }
        }

        private void positionOnPoint()
        {
            Vector3 screenPos = sceneWindow.getScreenPosition(scenePoint);
            int windowLeft = sceneWindow.RenderXLoc;
            int windowTop = sceneWindow.RenderYLoc;
            int windowRight = windowLeft + sceneWindow.RenderWidth;
            int windowBottom = windowTop + sceneWindow.RenderHeight;

            int left = (int)screenPos.x + windowLeft;
            int top = (int)screenPos.y + windowTop;
            int right = left + widget.Width;
            int bottom = top + widget.Height;

            if (right > windowRight)
            {
                left += windowRight - right;
            }
            if (bottom > windowBottom)
            {
                top += windowBottom - bottom;
            }

            if (left < windowLeft)
            {
                left = windowLeft;
            }
            if (top < windowTop)
            {
                top = windowTop;
            }

            widget.setPosition(left, top);
        }

        void sceneWindow_RenderingStarted(SceneViewWindow window, bool currentCameraRender)
        {
            positionOnPoint();
        }
    }
}
