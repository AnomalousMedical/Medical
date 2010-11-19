using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MyGUIPlugin;
using System.Drawing;
using Engine;

namespace Medical.GUI
{
    class MyGUIImageDisplay : Component, IImageDisplay
    {
        private ImageAtlas imageAtlas;
        private StaticImage imageBox;

        public MyGUIImageDisplay()
            :base("Medical.GUI.Timeline.ImageDisplay.MyGUIImageDisplay.layout")
        {
            widget.Visible = false;
            imageBox = widget.findWidget("ImageBox") as StaticImage;
        }

        public override void Dispose()
        {
            base.Dispose();
            imageAtlas.Dispose();
        }

        public void setImage(Stream image)
        {
            using (Bitmap bitmap = new Bitmap(image))
            {
                if (imageAtlas != null)
                {
                    imageAtlas.Dispose();
                }
                Size2 imageSize = new Size2(bitmap.Size.Width, bitmap.Size.Height);
                imageAtlas = new ImageAtlas("MyGUIImageDisplay_", imageSize, imageSize);
                String imageKey = imageAtlas.addImage(this, bitmap);
                imageBox.setItemResource(imageKey);
            }
        }

        public void show()
        {
            widget.Visible = true;
        }
    }
}
