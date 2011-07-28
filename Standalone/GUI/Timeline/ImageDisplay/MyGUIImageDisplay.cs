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

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                positionImage((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
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
                positionImage((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
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
                    positionImage((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
                }
            }
        }

        void sceneWindow_Resized(SceneViewWindow window)
        {
            positionImage((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
        }

        private void positionImage(int width, int height)
        {
            int left = (int)(position.x * width + sceneWindow.Location.x);
            int top = (int)(position.y * height + sceneWindow.Location.y);
            int newWidth, newHeight;

            if (keepAspectRatio)
            {
                if (height < width)
                {
                    newHeight = (int)(size.Height * height);
                    float heightRatio = (float)newHeight / bitmapSize.Height;
                    newWidth = (int)(heightRatio * bitmapSize.Width);
                }
                else
                {
                    newWidth = (int)(size.Width * width);
                    float widthRatio = (float)newWidth / bitmapSize.Width;
                    newHeight = (int)(widthRatio * bitmapSize.Height);
                }
            }
            else
            {
                newWidth = (int)(size.Width * width);
                newHeight = (int)(size.Height * height);
            }

            int right = left + newWidth;
            int windowRight = (int)(sceneWindow.Location.x + sceneWindow.WorkingSize.Width);
            if (right > windowRight)
            {
                left -= (int)(right - windowRight);
            }

            int bottom = top + newHeight;
            int windowBottom = (int)(sceneWindow.Location.y + sceneWindow.WorkingSize.Height);
            if (bottom > windowBottom)
            {
                top -= (int)(bottom - windowBottom);
            }

            widget.setPosition(left, top);
            widget.setSize(newWidth, newHeight);
        }
    }
}
