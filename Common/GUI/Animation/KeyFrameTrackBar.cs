using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing;

namespace Medical.GUI.Animation
{
    class KeyFrameTrackBar : UserControl
    {
        private const int minTickDelta = 10;

        private Rectangle trackRectangle = new Rectangle();
        private Rectangle ticksRectangle = new Rectangle();
        private Rectangle thumbRectangle = new Rectangle();
        private int currentTickPosition = 0;
        private bool thumbClicked = false;
        private TrackBarThumbState thumbState = TrackBarThumbState.Normal;
        int tickSkip = 1;
        private List<KeyFrameMark> keyFrames = new List<KeyFrameMark>();

        private int numKeyFrames = 100;

        public KeyFrameTrackBar()
        {
            this.Location = new Point(10, 10);
            this.DoubleBuffered = true;

            // Calculate the initial sizes of the bar, 
            // thumb and ticks.
            SetupTrackBar();
        }

        public void addKeyFrame(int time)
        {
            keyFrames.Add(new KeyFrameMark(time));
            Invalidate();
        }

        public void removeKeyFrame(int index)
        {
            keyFrames.RemoveAt(index);
            Invalidate();
        }

        public void clearKeyFrames()
        {
            keyFrames.Clear();
            Invalidate();
        }

        // Calculate the sizes of the bar, thumb, and ticks rectangle.
        private void SetupTrackBar()
        {
            using (Graphics g = this.CreateGraphics())
            {
                // Calculate the size of the track bar.
                trackRectangle.X = ClientRectangle.X;
                trackRectangle.Y = ClientRectangle.Y + 28;
                trackRectangle.Width = ClientRectangle.Width;
                trackRectangle.Height = 4;

                // Calculate the size of the thumb.
                thumbRectangle.Size = TrackBarRenderer.GetTopPointingThumbSize(g, TrackBarThumbState.Normal);

                // Calculate the size of the rectangle in which to draw the ticks.
                ticksRectangle.X = trackRectangle.X;
                ticksRectangle.Y = trackRectangle.Y - 8;
                ticksRectangle.Width = trackRectangle.Width;
                ticksRectangle.Height = 4;

                float tickSpace = ((float)ticksRectangle.Width) / ((float)numKeyFrames);
                if (tickSpace > minTickDelta)
                {
                    tickSkip = 1;
                }
                else
                {
                    int denom = ticksRectangle.Width / minTickDelta;
                    if (denom != 0)
                    {
                        tickSkip = numKeyFrames / denom;
                    }
                    else
                    {
                        tickSkip = numKeyFrames - 1;
                    }
                }

                calculateThumbPosition();
                thumbRectangle.Y = trackRectangle.Y - 8;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetupTrackBar();
            Invalidate();
        }

        // Draw the track bar.
        protected override void OnPaint(PaintEventArgs e)
        {
            TrackBarRenderer.DrawHorizontalTrack(e.Graphics, trackRectangle);
            TrackBarRenderer.DrawTopPointingThumb(e.Graphics, thumbRectangle, thumbState);
            using (Pen pen = new Pen(Color.Black))
            {
                Point p1 = new Point(0, ticksRectangle.Y);
                Point p2 = new Point(0, ticksRectangle.Y + ticksRectangle.Height);
                int thumbHalfWidth = (thumbRectangle.Width / 2);
                for (int i = 0; i < numKeyFrames; i += tickSkip)
                {
                    int location = (int)((float)i / numKeyFrames * ticksRectangle.Width) + thumbHalfWidth;
                    p1.X = location;
                    p2.X = location;e.Graphics.DrawLine(pen, p1, p2);
                }

                Color fillColor = Color.FromArgb(155, Color.Red);
                Color borderColor = Color.Black;
                Rectangle rect = new Rectangle(0, trackRectangle.Y, minTickDelta, 10);
                p2.Y = rect.Y;
                foreach (KeyFrameMark keyFrame in keyFrames)
                {
                    pen.Color = fillColor;
                    int location = (int)((float)keyFrame.Time / numKeyFrames * ticksRectangle.Width);
                    rect.X = location;
                    e.Graphics.FillRectangle(pen.Brush, rect);
                    
                    pen.Color = borderColor;
                    e.Graphics.DrawRectangle(pen, rect);

                    location += thumbHalfWidth;
                    p1.X = location;
                    p2.X = location; e.Graphics.DrawLine(pen, p1, p2);
                    p2.X = location; e.Graphics.DrawLine(pen, p1, p2);
                }
            }
        }

        // Determine whether the user has clicked the track bar thumb.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.thumbRectangle.Contains(e.Location))
            {
                thumbClicked = true;
                thumbState = TrackBarThumbState.Pressed;
            }

            this.Invalidate();
        }

        // Redraw the track bar thumb if the user has moved it.
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (thumbClicked == true)
            {
                if (e.Location.X > trackRectangle.X && e.Location.X < (trackRectangle.X + trackRectangle.Width - thumbRectangle.Width))
                {
                    thumbClicked = false;
                    thumbState = TrackBarThumbState.Hot;
                    this.Invalidate();
                }

                thumbClicked = false;
            }
        }

        // Track cursor movements.
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // The user is moving the thumb.
            if (thumbClicked == true)
            {
                currentTickPosition = (int)(e.Location.X / (float)ticksRectangle.Width * numKeyFrames);
                // Track movements to the next tick to the right, if 
                // the cursor has moved halfway to the next tick.
                if (currentTickPosition > numKeyFrames - 1)
                {
                    currentTickPosition = numKeyFrames - 1;
                }

                // Track movements to the next tick to the left, if 
                // cursor has moved halfway to the next tick.
                else if (currentTickPosition < 0)
                {
                    currentTickPosition = 0;
                }

                calculateThumbPosition();
            }

            // The cursor is passing over the track.
            else
            {
                thumbState = thumbRectangle.Contains(e.Location) ? TrackBarThumbState.Hot : TrackBarThumbState.Normal;
            }

            Invalidate();
        }

        private void calculateThumbPosition()
        {
            thumbRectangle.X = (int)((float)currentTickPosition / numKeyFrames * ticksRectangle.Width);
        }

        public int CurrentTickPosition
        {
            get
            {
                return currentTickPosition;
            }
            set
            {
                currentTickPosition = value;
                calculateThumbPosition();
            }
        }
    }
}
