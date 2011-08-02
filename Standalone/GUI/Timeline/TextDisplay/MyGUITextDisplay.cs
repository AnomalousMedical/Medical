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
        private Widget textBox;
        private MyGUITextDisplayFactory textFactory;
        private Vector2 position = new Vector2();
        private Size2 size = new Size2();
        private SceneViewWindow sceneWindow;

        public MyGUITextDisplay(MyGUITextDisplayFactory textFactory, SceneViewWindow sceneWindow)
            :base("Medical.GUI.Timeline.TextDisplay.MyGUITextDisplay.layout")
        {
            this.textFactory = textFactory;
            this.sceneWindow = sceneWindow;
            sceneWindow.Resized += sceneWindow_Resized;

            widget.Visible = false;
            textBox = widget.findWidget("TextBox");
        }

        public override void Dispose()
        {
            sceneWindow.Resized -= sceneWindow_Resized;
            textFactory.textDisposed(this);
            base.Dispose();
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
                return textBox.FontName;
            }
            set
            {
                textBox.FontName = value;
            }
        }

        public int FontHeight
        {
            get
            {
                return textBox.FontHeight;
            }
            set
            {
                textBox.FontHeight = value;
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
        }
    }
}
