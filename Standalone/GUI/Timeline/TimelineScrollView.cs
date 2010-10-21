using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    delegate void CanvasSizeChanged(float newSize);

    class TimelineScrollView
    {
        private ScrollView scrollView;
        public event CanvasSizeChanged CanvasWidthChanged;
        public event CanvasSizeChanged CanvasHeightChanged;

        public TimelineScrollView(ScrollView scrollView)
        {
            this.scrollView = scrollView;
        }

        public Button createButton(float left)
        {
            Button button = scrollView.createWidgetT("Button", "Button", (int)left, 0, 10, 10, Align.Left | Align.Top, "") as Button;
            if (button.Right > scrollView.CanvasSize.Width)
            {
                CanvasWidth = button.Right;
            }
            return button;
        }

        public float CanvasWidth
        {
            get
            {
                return scrollView.CanvasSize.Width;
            }
            set
            {
                Size2 canvasSize = scrollView.CanvasSize;
                canvasSize.Width = value;
                scrollView.CanvasSize = canvasSize;
                if (CanvasWidthChanged != null)
                {
                    CanvasWidthChanged.Invoke(value);
                }
            }
        }

        public float CanvasHeight
        {
            get
            {
                return scrollView.CanvasSize.Height;
            }
            set
            {
                Size2 canvasSize = scrollView.CanvasSize;
                if (canvasSize.Height != value)
                {
                    canvasSize.Height = value;
                    scrollView.CanvasSize = canvasSize;
                    if (CanvasHeightChanged != null)
                    {
                        CanvasHeightChanged.Invoke(value);
                    }
                }
            }
        }

        public IntCoord ClientCoord
        {
            get
            {
                return scrollView.ClientCoord;
            }
        }

        public Vector2 CanvasPosition
        {
            get
            {
                return scrollView.CanvasPosition;
            }
            set
            {
                scrollView.CanvasPosition = value;
            }
        }

        public bool AllowMouseScroll
        {
            get
            {
                return scrollView.AllowMouseScroll;
            }
            set
            {
                scrollView.AllowMouseScroll = value;
            }
        }

        public int AbsoluteLeft
        {
            get
            {
                return scrollView.AbsoluteLeft;
            }
        }

        public int AbsoluteTop
        {
            get
            {
                return scrollView.AbsoluteTop;
            }
        }

        public int Width
        {
            get
            {
                return scrollView.Width;
            }
        }

        public int Height
        {
            get
            {
                return scrollView.Height;
            }
        }
    }
}
