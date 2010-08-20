using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wx;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using Logging;

namespace Medical.GUI
{
    class ImageWindow : Frame
    {
        wx.Bitmap bmp;
        ImageViewer imageViewer;

        public ImageWindow(Window parent, String windowTitle, System.Drawing.Bitmap image)
            :base(parent, windowTitle, wxDefaultPosition, new System.Drawing.Size(640, 480))
        {
            //Button panel
            Panel buttonPanel = new Panel(this);
            BoxSizer buttonSizer = new BoxSizer(Orientation.wxHORIZONTAL);
            Button resizeButton = new Button(buttonPanel, "Resize");
            resizeButton.Click += new EventListener(resizeButton_Click);
            buttonSizer.Add(resizeButton, 1, 0);
            Button saveButton = new Button(buttonPanel, "Save");
            saveButton.Click += new EventListener(saveButton_Click);
            buttonSizer.Add(saveButton, 1, 0);
            Button exploreButton = new Button(buttonPanel, "Explore");
            exploreButton.Click += new EventListener(exploreButton_Click);
            buttonSizer.Add(exploreButton, 1, 0);
            buttonPanel.SetSizer(buttonSizer);

            BoxSizer formSizer = new BoxSizer(Orientation.wxVERTICAL);
            formSizer.Add(buttonPanel);

            //Bit of voodoo to get image into wxWidgets.
            String imageFile = (MedicalConfig.DocRoot + "/TempImage.png");
            image.Save(imageFile, ImageFormat.Png);
            image.Dispose();
            bmp = new wx.Bitmap(imageFile);
            try
            {
                File.Delete(imageFile);
            }
            catch (Exception e)
            {
                Logging.Log.Error("Failed to erase temp image {0}.", e.Message);
            }

            //Image Viewer
            imageViewer = new ImageViewer(this);
            imageViewer.Bitmap = bmp;
            formSizer.Add(imageViewer, 1, SizerFlag.wxEXPAND);

            SetSizer(formSizer);

            this.Layout();
            this.Show();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            imageViewer.Dispose();
        }

        void resizeButton_Click(object sender, Event e)
        {
            imageViewer.ScaleImage = !imageViewer.ScaleImage;
        }

        void exploreButton_Click(object sender, Event e)
        {
            if (File.Exists(this.Title))
            {
                try
                {
                    Process.Start("explorer.exe", "/select," + Path.GetFullPath(this.Title));
                }
                catch (Exception ex)
                {
                    Logging.Log.Default.sendMessage("Exception occured when opening explorer.exe:\n{0}.", LogLevel.Error, "Medical", ex.Message);
                }
            }
        }

        void saveButton_Click(object sender, Event e)
        {
            using (FileDialog saveFile = new FileDialog(this, "Choose location to save image.", Utils.GetHomeDir(), "", "JPEG(*.jpg)|*.jpg;|PNG(*.png)|*.png;|TIFF(*.tiff)|*.tiff;|BMP(*.bmp)|*.bmp;", WindowStyles.FD_SAVE|WindowStyles.FD_OVERWRITE_PROMPT))
            {
                if (saveFile.ShowModal() == ShowModalResult.CANCEL)
                {
                    return;
                }
                BitmapType format = BitmapType.wxBITMAP_TYPE_JPEG;
                switch (saveFile.FilterIndex)
                {
                    case 1:
                        format = BitmapType.wxBITMAP_TYPE_JPEG;
                        break;
                    case 2:
                        format = BitmapType.wxBITMAP_TYPE_PNG;
                        break;
                    case 3:
                        format = BitmapType.wxBITMAP_TYPE_TIF;
                        break;
                    case 4:
                        format = BitmapType.wxBITMAP_TYPE_BMP;
                        break;
                }
                imageViewer.Bitmap.SaveFile(saveFile.Path, format);
                this.Title = saveFile.Path;
            }
        }
    }
}
