using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Medical.GUI.Common
{
    [DefaultEvent("Click")]
    public class FancyButton : UserControl
    {
        private int currentIndex = 0;
        private int normalIndex = 0;
        private int hoverIndex = 0;
        private int clickIndex = 0;

        public FancyButton()
        {
            Capture = false;
            this.Size = new Size(32, 32);
        }

        public int NormalIndex
        {
            get
            {
                return normalIndex;
            }
            set
            {
                normalIndex = value;
                currentIndex = value;
                this.Invalidate();
            }
        }

        public int HoverIndex
        {
            get
            {
                return hoverIndex;
            }
            set
            {
                hoverIndex = value;
                this.Invalidate();
            }
        }

        public int ClickIndex
        {
            get
            {
                return clickIndex;
            }
            set
            {
                clickIndex = value;
                this.Invalidate();
            }
        }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (BackgroundImage == null)
            {
                base.OnPaint(pevent);
            }
            else
            {
                int image = currentIndex * ImageWidth;
                pevent.Graphics.DrawImage(BackgroundImage, pevent.ClipRectangle, new Rectangle(image, 0, ImageWidth, ImageHeight), GraphicsUnit.Pixel);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if ((Control.MouseButtons & MouseButtons.Left) != 0)
            {
                currentIndex = clickIndex;
            }
            else
            {
                currentIndex = hoverIndex;
            }
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            currentIndex = normalIndex;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            currentIndex = clickIndex;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            currentIndex = hoverIndex;
            this.Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }
    }
}
