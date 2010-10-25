using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class TimelineMarker
    {
        private Widget widget;
        private int pixelsPerSecond;
        private float dragStartPos;
        private float dragStartTime;
        private float time = 0.0f;

        public event EventHandler CoordChanged;

        public TimelineMarker(ActionView actionView, ScrollView scrollView)
        {
            this.widget = scrollView.createWidgetT("Widget", "Separator1", 1, 0, 1, (int)scrollView.CanvasSize.Height, Align.Left | Align.Top, "");
            widget.Pointer = CursorManager.SIZE_HORZ;
            widget.MouseDrag += new MyGUIEvent(widget_MouseDrag);
            widget.MouseButtonPressed += new MyGUIEvent(widget_MouseButtonPressed);

            pixelsPerSecond = actionView.PixelsPerSecond;
            actionView.CanvasHeightChanged += new CanvasSizeChanged(actionView_CanvasHeightChanged);
            actionView.PixelsPerSecondChanged += new EventHandler(actionView_PixelsPerSecondChanged);
        }

        public float Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                int physicalPosition = (int)(time * pixelsPerSecond);
                if(physicalPosition < 1)
                {
                    physicalPosition = 1;
                }
                widget.setPosition(physicalPosition, widget.Top);
                if (CoordChanged != null)
                {
                    CoordChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int Left
        {
            get
            {
                return widget.Left;
            }
        }

        public int Right
        {
            get
            {
                return widget.Right;
            }
        }

        public int Width
        {
            get
            {
                return widget.Width;
            }
        }

        void widget_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me.Button == Engine.Platform.MouseButtonCode.MB_BUTTON0)
            {
                dragStartPos = me.Position.x;
                dragStartTime = Time;
            }
        }

        void widget_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            float newTime = dragStartTime + (me.Position.x - dragStartPos) / pixelsPerSecond;
            if (newTime < 0.0f)
            {
                newTime = 0.0f;
            }
            Time = newTime;
        }

        void actionView_CanvasHeightChanged(float newSize)
        {
            widget.setSize(widget.Width, (int)newSize);
        }

        void actionView_PixelsPerSecondChanged(object sender, EventArgs e)
        {
            this.pixelsPerSecond = ((ActionView)sender).PixelsPerSecond;
            Time = time;
        }
    }
}
