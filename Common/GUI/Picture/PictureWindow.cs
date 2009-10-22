using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using Logging;

namespace Medical.GUI
{
    public partial class PictureWindow : DockContent
    {
        private static SaveFileDialog saveDialog = new SaveFileDialog();
        private bool activateParentOnClose = false;

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
        }

        void setResizeMode()
        {
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Dock = DockStyle.Fill;
            zoomStrechButton.Text = "Resize";
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult result = saveDialog.ShowDialog(DockPanel.TopLevelControl);
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
                exploreButton.Enabled = true;
            }
        }

        private void exploreButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(this.Text))
            {
                try
                {
                    activateParentOnClose = true;
                    Process.Start("explorer.exe", "/select," + Path.GetFullPath(this.Text));
                }
                catch (Exception ex)
                {
                    Log.Default.sendMessage("Exception occured when opening explorer.exe:\n{0}.", LogLevel.Error, "Medical", ex.Message);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //Hack for when the explorer window is opened to keep the main form from moving to the background.
            if (activateParentOnClose)
            {
                Form topLevel = DockPanel.TopLevelControl as Form;
                if (topLevel != null)
                {
                    topLevel.Activate();
                }
            }
        }
    }
}
