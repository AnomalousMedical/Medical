using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace Medical.GUI
{
    public delegate void CurrentBlendChanged(MedicalStateTrackBar trackBar, double currentTime);

    public class MedicalStateTrackBar : UserControl
    {
        public event CurrentBlendChanged CurrentBlendChanged;

        private Rectangle trackRectangle = new Rectangle();
        private Rectangle ticksRectangle = new Rectangle();
        private Rectangle thumbRectangle = new Rectangle();
        private bool thumbClicked = false;
        private TrackBarThumbState thumbState = TrackBarThumbState.Normal;
        private double currentBlend;
        private double maxBlend;

        public MedicalStateTrackBar()
        {
            this.DoubleBuffered = true;
            this.Name = "PlaybackTrackBar";
            this.Size = new System.Drawing.Size(150, 47);
            currentBlend = 0.0;
            maxBlend = 1.0;

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

                thumbRectangle.X = (int)(CurrentBlend / MaxBlend * ticksRectangle.Width);
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
                CurrentBlend = e.Location.X / (float)ticksRectangle.Width * MaxBlend;
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
            thumbRectangle.X = (int)(CurrentBlend / MaxBlend * ticksRectangle.Width);
        }

        public double CurrentBlend
        {
            get
            {
                return currentBlend;
            }
            set
            {
                currentBlend = value;
                if (currentBlend < 0.0)
                {
                    currentBlend = 0.0;
                }
                if (currentBlend > maxBlend)
                {
                    currentBlend = maxBlend;
                }
                calculateThumbPosition();
                if (CurrentBlendChanged != null)
                {
                    CurrentBlendChanged.Invoke(this, currentBlend);
                }
                Invalidate();
            }
        }

        public double MaxBlend
        {
            get
            {
                return maxBlend;
            }
            set
            {
                maxBlend = value;
                calculateThumbPosition();
            }
        }
    }
}
