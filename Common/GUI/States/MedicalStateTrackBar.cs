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
        private Rectangle markRectangle = new Rectangle();
        private int thumbValidZone = 1;
        private bool thumbClicked = false;
        private TrackBarThumbState thumbState = TrackBarThumbState.Normal;
        private double currentBlend;
        private double maxBlend;
        private List<MedicalStateMark> states = new List<MedicalStateMark>();
        private int selectedState = -1;
        Pen pen = new Pen(Color.Black);

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            pen.Dispose();
        }

        public void addState(MedicalState state, int index)
        {
            states.Insert(index, new MedicalStateMark(state));
            calculateThumbPosition();
            Invalidate();
        }

        public void removeState(MedicalState state)
        {

        }

        public void clearStates()
        {
            states.Clear();
            currentBlend = 0;
            calculateThumbPosition();
            Invalidate();
        }

        // Calculate the sizes of the bar, thumb, and ticks rectangle.
        private void SetupTrackBar()
        {
            using (Graphics g = this.CreateGraphics())
            {
                if (TrackBarRenderer.IsSupported)
                {
                    // Calculate the size of the thumb.
                    thumbRectangle.Size = TrackBarRenderer.GetTopPointingThumbSize(g, TrackBarThumbState.Normal);
                }
                else
                {
                    thumbRectangle.Size = new Size(11, 19);
                }

                // Calculate the size of the mark rectangle
                markRectangle.Y = trackRectangle.Y;
                markRectangle.Width = 10;
                markRectangle.Height = 10;

                // Calculate the size of the track bar.
                trackRectangle.X = ClientRectangle.X;
                trackRectangle.Y = ClientRectangle.Y + 28;
                trackRectangle.Width = ClientRectangle.Width;
                trackRectangle.Height = 4;

                // Calculate the size of the rectangle in which to 
                // draw the ticks.
                ticksRectangle.X = trackRectangle.X;
                ticksRectangle.Y = trackRectangle.Y - 8;
                ticksRectangle.Width = trackRectangle.Width - markRectangle.Width;
                ticksRectangle.Height = 4;

                thumbValidZone = trackRectangle.Width - thumbRectangle.Width;

                thumbRectangle.X = (int)(CurrentBlend / MaxBlend * thumbValidZone);
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
            if (TrackBarRenderer.IsSupported)
            {
                TrackBarRenderer.DrawHorizontalTrack(e.Graphics, trackRectangle);
                TrackBarRenderer.DrawTopPointingThumb(e.Graphics, thumbRectangle, thumbState);
            }
            else
            {
                e.Graphics.FillRectangle(pen.Brush, trackRectangle);
                e.Graphics.FillRectangle(pen.Brush, thumbRectangle);
            }

            for (int i = 0; i < states.Count; ++i)
            {
                states[i].render(e.Graphics, pen, ticksRectangle, markRectangle, i, states.Count);
            }
        }

        // Determine whether the user has clicked the track bar thumb.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (findMarkAt(e.Location) == -1)
            {
                if (this.thumbRectangle.Contains(e.Location))
                {
                    thumbClicked = true;
                    thumbState = TrackBarThumbState.Pressed;
                    Cursor.Hide();
                }

                this.Invalidate();
            }
        }

        // Redraw the track bar thumb if the user has moved it.
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (thumbClicked == false)
            {
                int index = findMarkAt(e.Location);
                if (index != -1)
                {
                    if (selectedState > 0 && selectedState < states.Count)
                    {
                        states[selectedState].Status = MedicalStateMarkStatus.Normal;
                    }
                    selectedState = index;
                    states[selectedState].Status = MedicalStateMarkStatus.Selected;
                    CurrentBlend = index;
                    Invalidate();
                }
            }
            else
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

        private int findMarkAt(Point location)
        {
            int maxIndex = states.Count;
            if (maxIndex > 1)
            {
                --maxIndex;
            }
            for (int i = 0; i < states.Count; ++i)
            {
                markRectangle.X = states[i].computeLocation(i, maxIndex, ticksRectangle.Width);
                if (markRectangle.Contains(location))
                {
                    return i;
                }
            }
            return -1;
        }

        // Track cursor movements.
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // The user is moving the thumb.
            if (thumbClicked == true)
            {
                CurrentBlend = e.Location.X / (float)thumbValidZone * MaxBlend;
            }
            // The cursor is passing over the track.
            else
            {
                thumbState = thumbRectangle.Contains(e.Location) ? TrackBarThumbState.Hot : TrackBarThumbState.Normal;
                int maxIndex = states.Count;
                if (maxIndex > 1)
                {
                    --maxIndex;
                }
                for (int i = 0; i < states.Count; ++i)
                {
                    if (i != selectedState)
                    {
                        markRectangle.X = states[i].computeLocation(i, maxIndex, ticksRectangle.Width);
                        states[i].Status = markRectangle.Contains(e.Location) ? MedicalStateMarkStatus.Hover : MedicalStateMarkStatus.Normal;
                    }
                }
            }
            Invalidate();
        }

        private void calculateThumbPosition()
        {
            thumbRectangle.X = (int)(currentBlend / MaxBlend * thumbValidZone);
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
