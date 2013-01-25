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
        private ImageBox imageBox;
        private MyGUIImageDisplayFactory displayFactory;
        private Vector2 position = new Vector2();
        private Size2 size = new Size2();
        private Size bitmapSize;
        private bool keepAspectRatio = true;
        private SceneViewWindow sceneWindow;
        private ImageAlignment alignment;

        public MyGUIImageDisplay(MyGUIImageDisplayFactory displayFactory, SceneViewWindow sceneWindow)
            :base("Medical.GUI.Timeline.ImageDisplay.MyGUIImageDisplay.layout")
        {
            this.displayFactory = displayFactory;
            this.sceneWindow = sceneWindow;
            sceneWindow.Resized += sceneWindow_Resized;

            widget.Visible = false;
            imageBox = widget.findWidget("ImageBox") as ImageBox;
            SuppressLayout = false;
        }

        public override void Dispose()
        {
            sceneWindow.Resized -= sceneWindow_Resized;
            displayFactory.displayDisposed(this);
            base.Dispose();
            if (imageAtlas != null)
            {
                imageAtlas.Dispose();
                imageAtlas = null;
            }
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
                imageAtlas = new ImageAtlas("MyGUIImageDisplay_", imageSize);
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

        public ImageAlignment Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                if (alignment != value)
                {
                    alignment = value;
                    positionImage((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
                }
            }
        }
        
        /// <summary>
        /// Suppress the layout. Use this when setting multiple properties at once.
        /// </summary>
        public bool SuppressLayout { get; set; }

        /// <summary>
        /// If you used SuppressLayout and want to explicitly layout the control. SuppressLayout must be false or this will still do nothing.
        /// </summary>
        public void layout()
        {
            positionImage((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
        }

        void sceneWindow_Resized(SceneViewWindow window)
        {
            positionImage((int)sceneWindow.WorkingSize.Width, (int)sceneWindow.WorkingSize.Height);
        }

        private void positionImage(int width, int height)
        {
            if (SuppressLayout)
            {
                return;
            }

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

            int left, top;
            switch (alignment)
            {
                case ImageAlignment.LeftTop:
                    left = 0;
                    top = 0;
                    break;
                case ImageAlignment.LeftBottom:
                    left = 0;
                    top = height;
                    break;
                case ImageAlignment.RightTop:
                    left = width;
                    top = 0;
                    break;
                case ImageAlignment.RightBottom:
                    left = width;
                    top = height;
                    break;
                case ImageAlignment.TopCenter:
                    left = width / 2 - newWidth / 2;
                    top = 0;
                    break;
                case ImageAlignment.BottomCenter:
                    left = width / 2 - newWidth / 2;
                    top = height;
                    break;
                case ImageAlignment.LeftCenter:
                    left = 0;
                    top = height / 2 - newHeight / 2;
                    break;
                case ImageAlignment.RightCenter:
                    left = width;
                    top = height / 2 - newHeight / 2;
                    break;
                case ImageAlignment.Center:
                    left = width / 2 - newWidth / 2;
                    top = height / 2 - newHeight / 2;
                    break;
                case ImageAlignment.Specify:
                    left = (int)(position.x * width);
                    top = (int)(position.y * height);
                    break;
                default:
                    left = 0;
                    top = 0;
                    break;
            }

            left += (int)sceneWindow.Location.x;
            top += (int)sceneWindow.Location.y;

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
