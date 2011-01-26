﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using Logging;
using System.Runtime.InteropServices;

namespace Medical.GUI
{
    class ImageWindow : IDisposable// : Frame
    {
        private IntPtr nativeWindow = IntPtr.Zero;

        public ImageWindow(WxOSWindow parent, String windowTitle, System.Drawing.Bitmap image)
        //:base(parent, windowTitle, wxDefaultPosition, new System.Drawing.Size(640, 480))
        {
            //Bit of voodoo to get image into wxWidgets.
            String imageFile = (MedicalConfig.DocRoot + "/TempImage.png");
            image.Save(imageFile, ImageFormat.Png);
            image.Dispose();

            nativeWindow = ImageWindow_new(parent._NativeOSWindow, windowTitle, imageFile, MedicalConfig.DocRoot);

            try
            {
                File.Delete(imageFile);
            }
            catch (Exception e)
            {
                Logging.Log.Error("Failed to erase temp image {0}.", e.Message);
            }
        }

        public void Dispose()
        {
            if (nativeWindow != IntPtr.Zero)
            {
                ImageWindow_delete(nativeWindow);
                nativeWindow = IntPtr.Zero;
            }
        }

        [DllImport("OSHelper")]
        private static extern IntPtr ImageWindow_new(IntPtr parent, String windowTitle, String imageFile, String homeDir);

        [DllImport("OSHelper")]
        private static extern void ImageWindow_delete(IntPtr window);
    }
}
