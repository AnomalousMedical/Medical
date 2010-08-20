using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wx;
using System.Drawing.Imaging;

namespace Medical.GUI
{
    class ImageWindow : Frame
    {
        wx.Bitmap bmp;
        ImageViewer imageViewer;

        public ImageWindow(Window parent, String windowTitle, System.Drawing.Bitmap image)
            :base(parent, windowTitle, wxDefaultPosition, new System.Drawing.Size(640, 480))
        {
            imageViewer = new ImageViewer(this);

            String imageFile = (MedicalConfig.DocRoot + "/TempImage.png");
            image.Save(imageFile, ImageFormat.Png);
            image.Dispose();
            
            bmp = new wx.Bitmap(imageFile);

            this.Layout();

            imageViewer.Bitmap = bmp;

            this.Show();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            imageViewer.Dispose();
        }
    }
}
