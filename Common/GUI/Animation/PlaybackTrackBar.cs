using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing;

namespace Medical.GUI
{
    public delegate void CurrentTimeChanged(PlaybackTrackBar trackBar, double currentTime);

    public class PlaybackTrackBar : UserControl
    {
        public event CurrentTimeChanged CurrentTimeChanged;

        private int numberTicks = 10;
        private Rectangle trackRectangle = new Rectangle();
        private Rectangle ticksRectangle = new Rectangle();
        private Rectangle thumbRectangle = new Rectangle();
        private bool thumbClicked = false;
        private TrackBarThumbState thumbState = TrackBarThumbState.Normal;
        private double currentTime;
        private double maxTime;
        private double tickDelta;

        public PlaybackTrackBar()
        {
            this.Location = new Point(10, 10);
            this.DoubleBuffered = true;
            this.Name = "PlaybackTrackBar";
            this.Size = new System.Drawing.Size(150, 47);
            currentTime = 0.0;
            maxTime = 60.0;
            tickDelta = 15.0f;
            calculateNumTicks();

            // Calculate the initial sizes of the bar, 
            // thumb and ticks.
            SetupTrackBar();
        }

        // Calculate the sizes of the bar, thumb, and ticks rectangle.
        private void SetupTrackBar()
        {
            using (Graphics g = this.CreateGraphics())
            {
                // Calculate the size of the track bar.
                trackRectangle.X = ClientRectangle.X + 2;
                trackRectangle.Y = ClientRectangle.Y + 28;
                trackRectangle.Width = ClientRectangle.Width - 4;
                trackRectangle.Height = 4;

                // Calculate the size of the rectangle in which to 
                // draw the ticks.
                ticksRectangle.X = trackRectangle.X + 4;
                ticksRectangle.Y = trackRectangle.Y - 8;
                ticksRectangle.Width = trackRectangle.Width - 8;
                ticksRectangle.Height = 4;

                // Calculate the size of the thumb.
                thumbRectangle.Size = TrackBarRenderer.GetTopPointingThumbSize(g, TrackBarThumbState.Normal);

                thumbRectangle.X = (int)(CurrentTime / MaxTime * ticksRectangle.Width);
                thumbRectangle.Y = trackRectangle.Y - 8;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetupTrackBar();
        }

        // Draw the track bar.
        protected override void OnPaint(PaintEventArgs e)
        {
            TrackBarRenderer.DrawHorizontalTrack(e.Graphics, trackRectangle);
            TrackBarRenderer.DrawTopPointingThumb(e.Graphics, thumbRectangle, thumbState);
            TrackBarRenderer.DrawHorizontalTicks(e.Graphics, ticksRectangle, numberTicks, EdgeStyle.Raised);
            //using (Pen pen = new Pen(Color.Red))
            //{
            //    pen.Width = 10;
            //    e.Graphics.DrawLine(pen, new Point(10, 0), new Point(10, 10));
            //}
        }

        // Determine whether the user has clicked the track bar thumb.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.thumbRectangle.Contains(e.Location))
            {
                thumbClicked = true;
                thumbState = TrackBarThumbState.Pressed;
                Cursor.Hide();
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
                Cursor.Show();
                thumbClicked = false;
            }
        }

        // Track cursor movements.
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // The user is moving the thumb.
            if (thumbClicked == true)
            {
                CurrentTime = e.Location.X / (float)ticksRectangle.Width * MaxTime;
            }
            // The cursor is passing over the track.
            else
            {
                thumbState = thumbRectangle.Contains(e.Location) ?  TrackBarThumbState.Hot : TrackBarThumbState.Normal;
            }

            Invalidate();
        }

        private void calculateThumbPosition()
        {
            thumbRectangle.X = (int)(CurrentTime / MaxTime * ticksRectangle.Width);
        }

        private void calculateNumTicks()
        {
            numberTicks = (int)(maxTime / tickDelta);
        }

        public double CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
                if (currentTime < 0.0)
                {
                    currentTime = 0.0;
                }
                if (currentTime > maxTime)
                {
                    currentTime = maxTime;
                }
                calculateThumbPosition();
                if (CurrentTimeChanged != null)
                {
                    CurrentTimeChanged.Invoke(this, currentTime);
                }
                Invalidate();
            }
        }

        public double MaxTime
        {
            get
            {
                return maxTime;
            }
            set
            {
                maxTime = value;
                calculateThumbPosition();
            }
        }

        public double TickDelta
        {
            get
            {
                return tickDelta;
            }
            set
            {
                tickDelta = value;
                calculateNumTicks();
            }
        }
    }
}
