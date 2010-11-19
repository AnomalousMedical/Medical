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
        private MyGUIImageDisplayFactory displayFactory;
        private Vector2 position = new Vector2();
        private Size2 size = new Size2();

        public MyGUIImageDisplay(MyGUIImageDisplayFactory displayFactory)
            :base("Medical.GUI.Timeline.ImageDisplay.MyGUIImageDisplay.layout")
        {
            this.displayFactory = displayFactory;

            widget.Visible = false;
            imageBox = widget.findWidget("ImageBox") as StaticImage;
        }

        public override void Dispose()
        {
            displayFactory.displayDisposed(this);
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

        public void screenResized(int width, int height)
        {
            widget.setPosition((int)(position.x * Gui.Instance.getViewWidth()), (int)(position.y * Gui.Instance.getViewHeight()));
            widget.setSize((int)(size.Width * Gui.Instance.getViewWidth()), (int)(size.Height * Gui.Instance.getViewHeight()));
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                widget.setPosition((int)(value.x * Gui.Instance.getViewWidth()), (int)(value.y * Gui.Instance.getViewHeight()));
            }
        }

        public Size2 Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                widget.setSize((int)(value.Width * Gui.Instance.getViewWidth()), (int)(value.Height * Gui.Instance.getViewHeight()));
            }
        }
    }
}
