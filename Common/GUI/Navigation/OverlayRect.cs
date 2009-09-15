using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public struct OverlayRect
    {
        private float x0, x1, y0, y1;

        public OverlayRect(float x0, float y0, float x1, float y1)
        {
            this.x0 = x0;
            this.x1 = x1;
            this.y0 = y0;
            this.y1 = y1;
        }

        public float X0
        {
            get
            {
                return x0;
            }
            set
            {
                x0 = value;
            }
        }

        public float X1
        {
            get
            {
                return x1;
            }
            set
            {
                x1 = value;
            }
        }

        public float Y0
        {
            get
            {
                return y0;
            }
            set
            {
                y0 = value;
            }
        }

        public float Y1
        {
            get
            {
                return y1;
            }
            set
            {
                y1 = value;
            }
        }
    }
}
