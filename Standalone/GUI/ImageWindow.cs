using System;
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
    public class ImageWindow : IDisposable// : Frame
    {
        private IntPtr nativeWindow = IntPtr.Zero;

        public ImageWindow(NativeOSWindow parent, String windowTitle, System.Drawing.Bitmap image)
            :this(parent, windowTitle, image, true, ".png", ImageFormat.Png)
        {
            
        }

        public ImageWindow(NativeOSWindow parent, String windowTitle, System.Drawing.Bitmap image, bool allowSaving, String extension, ImageFormat imageFormat)
        {
            //Bit of voodoo to get image into wxWidgets.
            String imageFile = String.Format("{0}/TempImage{1}", MedicalConfig.UserDocRoot, extension);
            image.Save(imageFile, imageFormat);
            image.Dispose();

            nativeWindow = ImageWindow_new(parent._NativePtr, windowTitle, imageFile, MedicalConfig.UserDocRoot, allowSaving);

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

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr ImageWindow_new(IntPtr parent, String windowTitle, String imageFile, String homeDir, bool allowSaving);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void ImageWindow_delete(IntPtr window);
    }
}
