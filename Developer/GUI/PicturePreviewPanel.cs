using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class PicturePreviewPanel : UserControl
    {
        ImageRenderer renderer;
        Bitmap currentBitmap;

        public PicturePreviewPanel()
        {
            ImageProperties = new ImageRendererProperties();
            InitializeComponent();
        }

        public void initialize(ImageRenderer imageRenderer, int width, int height)
        {
            this.renderer = imageRenderer;
            ImageProperties.Width = width;
            ImageProperties.Height = height;
        }

        public void saveBitmap(String filename)
        {
            if (currentBitmap == null)
            {
                refreshImage();
            }
            currentBitmap.Save(filename);
        }

        public void refreshImage()
        {
            previewPicture.Image = null;
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
            }
            currentBitmap = renderer.renderImage(ImageProperties);
            previewPicture.Image = currentBitmap;
        }

        public ImageRendererProperties ImageProperties { get; set; }

        private void refreshImageButton_Click(object sender, EventArgs e)
        {
            refreshImage();
        }
    }
}
