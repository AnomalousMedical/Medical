using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Medical.GUI
{
    public enum TrackMarkStatus
    {
        Normal,
        Hover,
        Selected,
    }

    public class TrackBarMark
    {
        private float location = 0.0f;

        public TrackBarMark()
        {
            Status = TrackMarkStatus.Normal;
        }

        public void render(Graphics g, Pen pen, Rectangle boundsRect, Rectangle renderRect, float maxValue)
        {
            switch(Status)
            {
                case TrackMarkStatus.Normal:
                    pen.Color = Color.Red;
                    break;
                case TrackMarkStatus.Hover:
                    pen.Color = Color.Yellow;
                    break;
                case TrackMarkStatus.Selected:
                    pen.Color = SystemColors.Highlight;
                    break;
            }
            int location = computeLocation(maxValue, boundsRect.Width);
            renderRect.X = location;
            g.FillRectangle(pen.Brush, renderRect);

            pen.Color = Color.Black;
            g.DrawRectangle(pen, renderRect);

            //location += thumbHalfWidth;
            //p1.X = location;
            //p2.X = location; e.Graphics.DrawLine(pen, p1, p2);
            //p2.X = location; e.Graphics.DrawLine(pen, p1, p2);
        }

        public int computeLocation(float maxValue, int boundsRectWidth)
        {
            return maxValue != 0.0f ? (int)(Location / maxValue * boundsRectWidth) : 0;
        }

        public TrackMarkStatus Status { get; set; }

        public virtual float Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }
    }
}
