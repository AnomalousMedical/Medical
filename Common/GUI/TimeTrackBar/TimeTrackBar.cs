using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Medical.GUI
{
    public delegate void TimeChanged(TimeTrackBar trackBar, double currentTime);
    public delegate void MarkMoved(TrackBarMark mark);

    public class TimeTrackBar : UserControl
    {
        public event TimeChanged TimeChanged;
        public event MarkMoved MarkMoved;

        private Rectangle trackRectangle = new Rectangle();
        private Rectangle ticksRectangle = new Rectangle();
        private Rectangle thumbRectangle = new Rectangle();
        private Rectangle markRectangle = new Rectangle();
        private int thumbValidZone = 1;
        private bool thumbClicked = false;
        private TrackBarThumbState thumbState = TrackBarThumbState.Normal;
        private float currentTime;
        private float maximumTime;
        private List<TrackBarMark> marks = new List<TrackBarMark>();
        private int selectedState = -1;
        Pen pen = new Pen(Color.Black);
        private TrackBarMark selectedMark = null;
        bool selectedMarkMoved = false;
        bool moveMarks = false;
        bool moveThumb = true;

        private ContextMenuStrip tickMenu = null;
        private TrackBarMark menuTargetMark = null;

        private Point barMenuLocation = new Point(0, 0);
        private ContextMenuStrip barMenu = null;

        public TimeTrackBar()
        {
            this.DoubleBuffered = true;
            this.Name = "PlaybackTrackBar";
            this.Size = new System.Drawing.Size(150, 47);
            currentTime = 0.0f;
            maximumTime = 0.0f;
            ChangeTimeOnSelection = false;

            // Calculate the initial sizes of the bar, 
            // thumb and ticks.
            SetupTrackBar();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            pen.Dispose();
        }

        public void addMark(TrackBarMark mark)
        {
            marks.Add(mark);
            calculateThumbPosition();
            Invalidate(this.ClientRectangle);
        }

        public void removeMark(TrackBarMark mark)
        {
            if (selectedState > 0 && selectedState < marks.Count)
            {
                selectedState = -1;
            }
            marks.Remove(mark);
            calculateThumbPosition();
            Invalidate(this.ClientRectangle);
        }

        public void clearMarks()
        {
            selectedState = -1;
            marks.Clear();
            currentTime = 0;
            calculateThumbPosition();
            Invalidate(this.ClientRectangle);
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

                thumbRectangle.X = (int)(CurrentTime / MaximumTime * thumbValidZone);
                thumbRectangle.Y = trackRectangle.Y - 8;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetupTrackBar();
            Invalidate(this.ClientRectangle);
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

            for (int i = 0; i < marks.Count; ++i)
            {
                marks[i].render(e.Graphics, pen, ticksRectangle, markRectangle, maximumTime);
            }
        }

        // Determine whether the user has clicked the track bar thumb.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int mark = findMarkAt(e.Location);
                if (mark == -1)
                {
                    if (moveThumb && this.thumbRectangle.Contains(e.Location))
                    {
                        thumbClicked = true;
                        thumbState = TrackBarThumbState.Pressed;
                        Cursor.Hide();
                        this.Invalidate(this.ClientRectangle);
                    }
                }
                else
                {
                    selectedMark = marks[mark];
                    selectedMarkMoved = false;
                    if (moveMarks)
                    {
                        Cursor.Hide();
                    }
                }
            }
        }

        // Redraw the track bar thumb if the user has moved it.
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (selectedMark != null)
                {
                    if (selectedState > 0 && selectedState < marks.Count)
                    {
                        marks[selectedState].Status = TrackMarkStatus.Normal;
                    }
                    selectedState = marks.IndexOf(selectedMark);
                    marks[selectedState].Status = TrackMarkStatus.Selected;
                    Invalidate(this.ClientRectangle);
                    if (selectedMarkMoved)
                    {
                        selectedMarkMoved = false;
                        if (MarkMoved != null)
                        {
                            MarkMoved.Invoke(selectedMark);
                        }
                    }
                    if (ChangeTimeOnSelection)
                    {
                        CurrentTime = selectedMark.Location;
                    }
                    selectedMark = null;
                    Cursor.Show();
                }
                else if (thumbClicked)
                {
                    if (e.Location.X > trackRectangle.X && e.Location.X < (trackRectangle.X + trackRectangle.Width - thumbRectangle.Width))
                    {
                        thumbClicked = false;
                        thumbState = TrackBarThumbState.Hot;
                        this.Invalidate(this.ClientRectangle);
                    }
                    Cursor.Show();
                    thumbClicked = false;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (!thumbClicked && selectedMark == null)
                {
                    int mark = findMarkAt(e.Location);
                    if (mark != -1)
                    {
                        if (tickMenu != null)
                        {
                            menuTargetMark = marks[mark];
                            tickMenu.Show(this, e.Location);
                        }
                    }
                    else if(barMenu != null)
                    {
                        barMenuLocation = e.Location;
                        barMenu.Show(this, e.Location);
                    }
                }
            }
        }

        private int findMarkAt(Point location)
        {
            for (int i = 0; i < marks.Count; ++i)
            {
                markRectangle.X = marks[i].computeLocation(maximumTime, ticksRectangle.Width);
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
                CurrentTime = e.Location.X / (float)thumbValidZone * MaximumTime;
            }
            else if (selectedMark != null && moveMarks)
            {
                float location = e.Location.X / (float)thumbValidZone * MaximumTime;
                if (location < 0.0)
                {
                    location = 0.0f;
                }
                if (location > maximumTime)
                {
                    location = maximumTime;
                }
                selectedMark.Location = location;
                selectedMarkMoved = true;
            }
            // The cursor is passing over the track.
            else
            {
                thumbState = thumbRectangle.Contains(e.Location) ? TrackBarThumbState.Hot : TrackBarThumbState.Normal;
                for (int i = 0; i < marks.Count; ++i)
                {
                    if (i != selectedState)
                    {
                        markRectangle.X = marks[i].computeLocation(maximumTime, ticksRectangle.Width);
                        marks[i].Status = markRectangle.Contains(e.Location) ? TrackMarkStatus.Hover : TrackMarkStatus.Normal;
                    }
                }
            }
            Invalidate(this.ClientRectangle);
        }

        private void calculateThumbPosition()
        {
            thumbRectangle.X = (int)(currentTime / MaximumTime * thumbValidZone);
        }

        public float CurrentTime
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
                    currentTime = 0.0f;
                }
                if (currentTime > maximumTime)
                {
                    currentTime = maximumTime;
                }
                calculateThumbPosition();
                if (TimeChanged != null)
                {
                    TimeChanged.Invoke(this, currentTime);
                }
                Invalidate(this.ClientRectangle);
            }
        }

        public float MaximumTime
        {
            get
            {
                return maximumTime;
            }
            set
            {
                maximumTime = value;
                calculateThumbPosition();
                this.Invalidate(this.ClientRectangle);
            }
        }

        public TrackBarMark SelectedMark
        {
            get
            {
                if (selectedState > 0 && selectedState < marks.Count)
                {
                    return marks[selectedState];
                }
                return null;
            }
            set
            {
                selectedState = marks.IndexOf(value);
            }
        }

        public bool MoveMarks
        {
            get
            {
                return moveMarks;
            }
            set
            {
                moveMarks = value;
            }
        }

        public bool MoveThumb
        {
            get
            {
                return moveThumb;
            }
            set
            {
                moveThumb = value;
            }
        }

        /// <summary>
        /// Set this to true to change the time to the selected mark when a
        /// timer mark is selected.
        /// </summary>
        public bool ChangeTimeOnSelection { get; set; }

        public TrackBarMark MenuTargetMark
        {
            get
            {
                return menuTargetMark;
            }
        }

        public ContextMenuStrip TickMenu
        {
            get
            {
                return tickMenu;
            }
            set
            {
                tickMenu = value;
            }
        }

        public ContextMenuStrip BarMenu
        {
            get
            {
                return barMenu;
            }
            set
            {
                barMenu = value;
            }
        }

        public Point BarMenuLocation
        {
            get
            {
                return barMenuLocation;
            }
        }

        public float BarMenuTime
        {
            get
            {
                float time = barMenuLocation.X / (float)thumbValidZone * MaximumTime;
                if (time < 0)
                {
                    time = 0;
                }
                else if (time > maximumTime)
                {
                    time = maximumTime;
                }
                return time;
            }
        }
    }
}
