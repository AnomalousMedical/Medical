using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Medical.GUI
{
    enum MedicalStateMarkStatus
    {
        Normal,
        Hover,
        Selected,
    }

    class MedicalStateMark
    {
        private MedicalState state;

        public MedicalStateMark(MedicalState state)
        {
            this.state = state;
            Status = MedicalStateMarkStatus.Normal;
        }

        public void render(Graphics g, Pen pen, Rectangle boundsRect, Rectangle renderRect, int index, int maxIndex)
        {
            if (maxIndex > 1)
            {
                maxIndex = maxIndex - 1;
            }
            switch(Status)
            {
                case MedicalStateMarkStatus.Normal:
                    pen.Color = Color.Red;
                    break;
                case MedicalStateMarkStatus.Hover:
                    pen.Color = Color.Yellow;
                    break;
                case MedicalStateMarkStatus.Selected:
                    pen.Color = SystemColors.Highlight;
                    break;
            }
            int location = computeLocation(index, maxIndex, boundsRect.Width);
            renderRect.X = location;
            g.FillRectangle(pen.Brush, renderRect);

            pen.Color = Color.Black;
            g.DrawRectangle(pen, renderRect);

            //location += thumbHalfWidth;
            //p1.X = location;
            //p2.X = location; e.Graphics.DrawLine(pen, p1, p2);
            //p2.X = location; e.Graphics.DrawLine(pen, p1, p2);
        }

        public int computeLocation(int index, int maxIndex, int boundsRectWidth)
        {
            return (int)((float)index / maxIndex * boundsRectWidth);
        }

        public MedicalStateMarkStatus Status { get; set; }
    }
}
