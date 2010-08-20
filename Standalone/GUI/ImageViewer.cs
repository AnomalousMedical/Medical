using wx;
using System.Drawing;
using System;

namespace Medical.GUI
{
    public class ImageViewer : ScrolledWindow
    {
        private wx.Bitmap masterBitmap;
        private wx.Bitmap scaledBitmap;
        private bool scaleImage = true;

        public ImageViewer(Window parent)
            : base(parent)
        {
            BackgroundColour = Colour.wxLIGHT_GREY;

            this.EVT_SIZE(OnSize);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (scaledBitmap != null)
            {
                scaledBitmap.Dispose();
            }
        }

        /**
         * The bitmap to be displayed.
         */
        public wx.Bitmap Bitmap
        {
            set
            {
                masterBitmap = value;

                if (scaleImage)
                {
                    scaleMasterImage();
                }

                if (scaleImage)
                {
                    SetScrollbars(1, 1, 0, 0, 0, 0, true);
                }
                else
                {
                    SetScrollbars(1, 1, masterBitmap.Width, masterBitmap.Height, 0, 0, true);
                }

                // Redraw the window
                Refresh();
            }
        }

        /**
         * Override the OnDraw method so we can draw the bitmap.
         */
        public override void OnDraw(DC dc)
        {
            if (masterBitmap != null)
            {
                if (scaleImage)
                {
                    dc.DrawBitmap(scaledBitmap, (this.Width - scaledBitmap.Width) / 2, (this.Height - scaledBitmap.Height) / 2, false);
                }
                else
                {
                    dc.DrawBitmap(masterBitmap, 0, 0, false);
                }
            }
        }

        private void OnSize(object sender, Event evt)
        {
            evt.Skip();
            if (scaleImage)
            {
                scaleMasterImage();
                Refresh();
            }
        }

        private void scaleMasterImage()
        {
            if (scaledBitmap != null)
            {
                scaledBitmap.Dispose();
            }
            wx.Image image = masterBitmap.ConvertToImage();
            Size windowSize = this.Parent.ClientSize;

            double sx = (double)windowSize.Width / image.Width;
            double sy = (double)windowSize.Height / image.Height;
            double scale = Math.Min(sx, sy);

            image.Rescale((int)(image.Width * scale), (int)(image.Height * scale));
            scaledBitmap = new wx.Bitmap(image);
            image.Dispose();
        }
    }
}