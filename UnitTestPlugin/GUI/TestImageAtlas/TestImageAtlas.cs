using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using OgrePlugin;
using System.Drawing;
using System.Reflection;
using MyGUIPlugin;
using Engine;
using Logging;
using FreeImageAPI;

namespace UnitTestPlugin.GUI
{
    class TestImageAtlas : MDIDialog
    {
        private PagedImageAtlas imageAtlas = new PagedImageAtlas("Test", MyGUIInterface.Instance.CommonResourceGroup.FullName);

        public TestImageAtlas()
            : base("UnitTestPlugin.GUI.TestImageAtlas.TestImageAtlas.layout")
        {
            using (var bitmap = new FreeImageBitmap(Assembly.GetCallingAssembly().GetManifestResourceStream("UnitTestPlugin.Resources.LegacyLogoSmall.jpg")))
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Log.Debug("Adding LegacyLogoSmall to image atlas");
                ImageAtlasPage page = imageAtlas.addImage("LegacyLogoSmall", bitmap);
                ImageBox imageBox1 = (ImageBox)window.findWidget("ImageBox1");
                Rectangle coord;
                if (page.tryGetImageLocation("LegacyLogoSmall", out coord))
                {
                    imageBox1.setImageTexture(page.TextureName);
                    imageBox1.setImageCoord(new IntCoord(coord.Left, coord.Top, coord.Width, coord.Height));
                }
            }
            using (var bitmap = new FreeImageBitmap(Assembly.GetCallingAssembly().GetManifestResourceStream("UnitTestPlugin.Resources.DownloadIcon.png")))
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                for (int i = 0; i < 10; ++i)
                {
                    String imageName = "DownloadIcon" + i;
                    Log.Debug("Adding DownloadIcon to image atlas");
                    ImageAtlasPage page = imageAtlas.addImage(imageName, bitmap);
                    Log.Debug("Page is {0}", page.TextureName);
                    ImageBox imageBox1 = (ImageBox)window.findWidget("ImageBox2");
                    Rectangle coord;
                    if (page.tryGetImageLocation(imageName, out coord))
                    {
                        imageBox1.setImageTexture(page.TextureName);
                        imageBox1.setImageCoord(new IntCoord(coord.Left, coord.Top, coord.Width, coord.Height));
                    }
                }
            }
            using (var bitmap = new FreeImageBitmap(Assembly.GetCallingAssembly().GetManifestResourceStream("UnitTestPlugin.Resources.LegacyLogoSmall.jpg")))
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Log.Debug("Adding LegacyLogoSmall1 to image atlas");
                ImageAtlasPage page = imageAtlas.addImage("LegacyLogoSmall1", bitmap);
                ImageBox imageBox1 = (ImageBox)window.findWidget("ImageBox1");
                Rectangle coord;
                if (page.tryGetImageLocation("LegacyLogoSmall1", out coord))
                {
                    imageBox1.setImageTexture(page.TextureName);
                    imageBox1.setImageCoord(new IntCoord(coord.Left, coord.Top, coord.Width, coord.Height));
                }
            }
        }

        public override void Dispose()
        {
            imageAtlas.Dispose();
            base.Dispose();
        }
    }
}
