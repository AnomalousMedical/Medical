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
            InitializeComponent();
        }

        public void initialize(ImageRenderer imageRenderer)
        {
            this.renderer = imageRenderer;
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
            ImageRendererProperties properties = new ImageRendererProperties();
            properties.Width = previewPicture.Size.Width;
            properties.Height = previewPicture.Size.Height;
            currentBitmap = renderer.renderImage(properties);
            previewPicture.Image = currentBitmap;
        }

        private void refreshImageButton_Click(object sender, EventArgs e)
        {
            refreshImage();
        }
    }
}
