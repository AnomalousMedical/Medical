using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using Logging;

namespace Medical.GUI
{
    class ImageWindow : IDisposable// : Frame
    {
        //wx.Bitmap bmp;
        //ImageViewer imageViewer;

        //private MenuItem exploreItem;
        
        //private const int SAVE_ID = 1000;
        //private const int EXPLORE_ID = 1001;
        //private const int CLOSE_ID = 1002;
        //private const int RESIZE_ID = 1003;
        

        public ImageWindow(WxOSWindow parent, String windowTitle, System.Drawing.Bitmap image)
            //:base(parent, windowTitle, wxDefaultPosition, new System.Drawing.Size(640, 480))
        {
            ////Menu
            //MenuBar menuBar = new MenuBar();

            //Menu fileMenu = new Menu();
            //fileMenu.Append(SAVE_ID, "&Save...\tCtrl+S", "Save this image to disk.");
            //exploreItem = fileMenu.Append(EXPLORE_ID, "&Explore...\tCtrl+E", "Open this image's location.");
            //exploreItem.Enabled = false;
            //fileMenu.Append(CLOSE_ID, "&Close...\tCtrl+C", "Close this window.");

            //Menu imageMenu = new Menu();
            //imageMenu.Append(RESIZE_ID, "&Resize...\tCtrl+R", "Change the scaling mode of this image.");

            //menuBar.Append(fileMenu, "&File");
            //menuBar.Append(imageMenu, "&Image");
            //this.MenuBar = menuBar;

            //BoxSizer formSizer = new BoxSizer(Orientation.wxVERTICAL);

            //EVT_MENU(SAVE_ID, save);
            //EVT_MENU(EXPLORE_ID, explore);
            //EVT_MENU(CLOSE_ID, menuClose);
            //EVT_MENU(RESIZE_ID, resizeImage);

            ////Bit of voodoo to get image into wxWidgets.
            //String imageFile = (MedicalConfig.DocRoot + "/TempImage.png");
            //image.Save(imageFile, ImageFormat.Png);
            //image.Dispose();
            //bmp = new wx.Bitmap(imageFile);
            //try
            //{
            //    File.Delete(imageFile);
            //}
            //catch (Exception e)
            //{
            //    Logging.Log.Error("Failed to erase temp image {0}.", e.Message);
            //}

            ////Image Viewer
            //imageViewer = new ImageViewer(this);
            //imageViewer.Bitmap = bmp;
            //formSizer.Add(imageViewer, 1, SizerFlag.wxEXPAND);

            //SetSizer(formSizer);

            //this.Layout();
            //this.Show();
        }

        public void Dispose()
        {
            //base.Dispose(disposing);
            //imageViewer.Dispose();
        }

        void resizeImage(/*object sender, Event e*/)
        {
            //imageViewer.ScaleImage = !imageViewer.ScaleImage;
        }

        void explore(/*object sender, Event e*/)
        {
//            if (File.Exists(this.Title))
//            {
//                try
//                {
//#if WINDOWS
//                    Process.Start("explorer.exe", "/select," + Path.GetFullPath(this.Title));
//#elif MAC_OSX   
//                    Process.Start("open", Path.GetDirectoryName(Path.GetFullPath(this.Title)));
//#endif
//                }
//                catch (Exception ex)
//                {
//                    Logging.Log.Default.sendMessage("Exception occured when opening explorer.exe:\n{0}.", LogLevel.Error, "Medical", ex.Message);
//                }
//            }
        }

        void save(/*object sender, Event e*/)
        {
            //Logging.Log.Debug("Save clicked");
            //using (FileDialog saveFile = new FileDialog(this, "Choose location to save image.", Utils.GetHomeDir(), "", "JPEG(*.jpg)|*.jpg;|PNG(*.png)|*.png;|TIFF(*.tiff)|*.tiff;|BMP(*.bmp)|*.bmp;", WindowStyles.FD_SAVE|WindowStyles.FD_OVERWRITE_PROMPT))
            //{
            //    if (saveFile.ShowModal() == ShowModalResult.CANCEL)
            //    {
            //        return;
            //    }
            //    BitmapType format = BitmapType.wxBITMAP_TYPE_JPEG;
            //    switch (saveFile.FilterIndex)
            //    {
            //        case 1:
            //            format = BitmapType.wxBITMAP_TYPE_JPEG;
            //            break;
            //        case 2:
            //            format = BitmapType.wxBITMAP_TYPE_PNG;
            //            break;
            //        case 3:
            //            format = BitmapType.wxBITMAP_TYPE_TIF;
            //            break;
            //        case 4:
            //            format = BitmapType.wxBITMAP_TYPE_BMP;
            //            break;
            //    }
            //    imageViewer.Bitmap.SaveFile(saveFile.Path, format);
            //    this.Title = saveFile.Path;
            //    exploreItem.Enabled = true;
            //}
        }

        void menuClose(/*object sender, Event e*/)
        {
            //this.Close();
        }
    }
}
