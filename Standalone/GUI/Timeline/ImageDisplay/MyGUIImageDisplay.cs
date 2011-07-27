using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MyGUIPlugin;
using System.Drawing;
using Engine;
using Medical.Controller;

namespace Medical.GUI
{
    class MyGUIImageDisplay : Component, IImageDisplay
    {
        private ImageAtlas imageAtlas;
        private StaticImage imageBox;
        private MyGUIImageDisplayFactory displayFactory;
        private Vector2 position = new Vector2();
        private Size2 size = new Size2();
        private Size bitmapSize;
        private bool keepAspectRatio = true;
        private SceneViewWindow sceneWindow;

        public MyGUIImageDisplay(MyGUIImageDisplayFactory displayFactory, SceneViewWindow sceneWindow)
            :base("Medical.GUI.Timeline.ImageDisplay.MyGUIImageDisplay.layout")
        {
            this.displayFactory = displayFactory;
            this.sceneWindow = sceneWindow;
            sceneWindow.Resized += sceneWindow_Resized;

            widget.Visible = false;
            imageBox = widget.findWidget("ImageBox") as StaticImage;
        }

        public override void Dispose()
        {
            sceneWindow.Resized -= sceneWindow_Resized;
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
                bitmapSize = bitmap.Size;
                Size2 imageSize = new Size2(bitmapSize.Width, bitmapSize.Height);
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

        private void adjustPosition(int width, int height)
        {
            widget.setPosition((int)(position.x * width + sceneWindow.Location.x), (int)(position.y * height + sceneWindow.Location.y));
        }

        private void adjustSize(int width, int height)
        {
            if (keepAspectRatio)
            {
                if (height < width)
                {
                    int newHeight = (int)(size.Height * height);
                    float heightRatio = (float)newHeight / bitmapSize.Height;
                    int newWidth = (int)(heightRatio * bitmapSize.Width);
                    widget.setSize(newWidth, newHeight);
                }
                else
                {
                    int newWidth = (int)(size.Width * width);
                    float widthRatio = (float)newWidth / bitmapSize.Width;
                    int newHeight = (int)(widthRatio * bitmapSize.Height);
                    widget.setSize(newWidth, newHeight);
                }
            }
            else
            {
                widget.setSize((int)(size.Width * width), (int)(size.Height * height));
            }
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
                adjustPosition((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
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
                adjustSize((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
            }
        }

        public bool KeepAspectRatio
        {
            get
            {
                return keepAspectRatio;
            }
            set
            {
                if (keepAspectRatio != value)
                {
                    keepAspectRatio = value;
                    adjustSize((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
                }
            }
        }

        void sceneWindow_Resized(SceneViewWindow window)
        {
            adjustPosition((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
            adjustSize((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
        }
    }
}
