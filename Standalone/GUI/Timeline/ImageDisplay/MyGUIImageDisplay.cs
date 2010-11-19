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
            LayerManager.Instance.upLayerItem(widget);
            widget.Visible = true;
        }

        public Vector2 Position
        {
            get
            {
                return new Vector2((float)widget.Left / Gui.Instance.getViewWidth(), (float)widget.Top / Gui.Instance.getViewHeight());
            }
            set
            {
                widget.setPosition((int)(value.x * Gui.Instance.getViewWidth()), (int)(value.y * Gui.Instance.getViewHeight()));
            }
        }

        public Size2 Size
        {
            get
            {
                return new Size2((float)widget.Width / Gui.Instance.getViewWidth(), (float)widget.Height / Gui.Instance.getViewHeight());
            }
            set
            {
                widget.setSize((int)(value.Width * Gui.Instance.getViewWidth()), (int)(value.Height * Gui.Instance.getViewHeight()));
            }
        }
    }
}
