using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using Logging;

namespace Medical.GUI
{
    public partial class PictureWindow : UserControl
    {
        public event EventHandler TitleTextChanged;

        private static SaveFileDialog saveDialog = new SaveFileDialog();

        static PictureWindow()
        {
            saveDialog.Filter = "JPEG(*.jpg)|*.jpg;|PNG(*.png)|*.png;|TIFF(*.tiff)|*.tiff;|BMP(*.bmp)|*.bmp;";
        }
        
        public PictureWindow()
        {
            InitializeComponent();
            this.Disposed += new EventHandler(PictureWindow_Disposed);
        }

        void PictureWindow_Disposed(object sender, EventArgs e)
        {
            pictureBox.Image.Dispose();
        }

        public void initialize(Bitmap image)
        {
            this.AutoSize = true;
            pictureBox.Image = image;
            pictureBox.Size = image.Size;
            setResizeMode();
        }

        private void zoomStrechButton_Click(object sender, EventArgs e)
        {
            if (zoomStrechButton.Text == "Full")
            {
                setResizeMode();
            }
            else if (zoomStrechButton.Text == "Resize")
            {
                setFullMode();
            }
        }

        void setFullMode()
        {
            pictureBox.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox.Dock = DockStyle.None;
            zoomStrechButton.Text = "Full";
            //zoomStrechButton.Image = Resources.ResizeUpSmall;
        }

        void setResizeMode()
        {
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Dock = DockStyle.Fill;
            zoomStrechButton.Text = "Resize";
            //zoomStrechButton.Image = Resources.ResizeDownSmall;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult result = saveDialog.ShowDialog(this.TopLevelControl);
            if (result == DialogResult.OK)
            {
                ImageFormat format = ImageFormat.Jpeg;
                switch (saveDialog.FilterIndex)
                {
                    case 1:
                        format = ImageFormat.Jpeg;
                        break;
                    case 2:
                        format = ImageFormat.Png;
                        break;
                    case 3:
                        format = ImageFormat.Tiff;
                        break;
                    case 4:
                        format = ImageFormat.Bmp;
                        break;
                }
                pictureBox.Image.Save(saveDialog.FileName, format);
                this.Text = saveDialog.FileName;
                if (TitleTextChanged != null)
                {
                    TitleTextChanged.Invoke(this, EventArgs.Empty);
                }
                exploreButton.Enabled = true;
            }
        }

        private void exploreButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(this.Text))
            {
                try
                {
                    Process.Start("explorer.exe", "/select," + Path.GetFullPath(this.Text));
                }
                catch (Exception ex)
                {
                    Log.Default.sendMessage("Exception occured when opening explorer.exe:\n{0}.", LogLevel.Error, "Medical", ex.Message);
                }
            }
        }
    }
}
