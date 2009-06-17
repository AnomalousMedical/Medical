using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Medical.GUI
{
    class MedicalStateMark
    {
        private MedicalState state;

        public MedicalStateMark(MedicalState state)
        {
            this.state = state;
        }

        public void render(Graphics g, Pen pen, Rectangle boundsRect, Rectangle renderRect, int index, int maxIndex)
        {
            if (maxIndex > 1)
            {
                maxIndex = maxIndex - 1;
            }
            pen.Color = Color.Red;
            int location = (int)((float)index / maxIndex * boundsRect.Width);
            renderRect.X = location;
            g.FillRectangle(pen.Brush, renderRect);

            pen.Color = Color.Black;
            g.DrawRectangle(pen, renderRect);

            //location += thumbHalfWidth;
            //p1.X = location;
            //p2.X = location; e.Graphics.DrawLine(pen, p1, p2);
            //p2.X = location; e.Graphics.DrawLine(pen, p1, p2);
        }
    }
}
