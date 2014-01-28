using Engine;
using Engine.Attributes;
using Engine.Editing;
using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public class ImageElementStyle : ElementStyleDefinition
    {
        [SingleEnum]
        public enum ImageTextAlign
        {
            None,
            Right,
            Left,
        }

        private ImageTextAlign textAlign = ImageTextAlign.None;
        private bool center = false;
        private bool fixedSize = true;
        private int width = 200;
        private Vector2 offset = new Vector2();

        public ImageElementStyle(Element imageElement)
        {
            String classes = imageElement.GetAttributeString("class");
            if (classes != null)
            {
                String[] splitClasses = classes.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                center = splitClasses.FirstOrDefault(c => "Center".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
                if (splitClasses.FirstOrDefault(c => "LeftText".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    textAlign = ImageTextAlign.Left;
                }
                else if (splitClasses.FirstOrDefault(c => "RightText".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    textAlign = ImageTextAlign.Right;
                }
            }
            InlineCssParser inlineCss = new InlineCssParser(imageElement.GetAttributeString("style"));
            if (inlineCss.contains("width"))
            {
                fixedSize = !inlineCss.isValuePercent("width");
                int? widthNull = inlineCss.intValue("width");
                if (widthNull.HasValue)
                {
                    width = widthNull.Value;
                }
            }
            int? x = null;
            int? y = inlineCss.intValue("margin-top");
            switch (textAlign)
            {
                case ImageTextAlign.None:
                case ImageTextAlign.Right:
                    x = inlineCss.intValue("margin-left");
                    break;
                case ImageTextAlign.Left:
                    x = inlineCss.intValue("margin-right");
                    break;
            }
            offset = new Vector2(x.GetValueOrDefault(0), y.GetValueOrDefault(0));
        }

        public override bool buildClassList(StringBuilder classes)
        {
            if (center)
            {
                classes.Append("Center ");
            }
            switch (TextAlign)
            {
                case ImageTextAlign.Left:
                    classes.Append("LeftText");
                    break;
                case ImageTextAlign.Right:
                    classes.Append("RightText");
                    break;
            }
            return true;
        }

        public override bool buildStyleAttribute(StringBuilder styleAttribute)
        {
            if (fixedSize)
            {
                styleAttribute.AppendFormat("width:{0}px;", width); 
            }
            else
            {
                styleAttribute.AppendFormat("width:{0}%;", width);
            }
            if (offset.y != 0.0f)
            {
                styleAttribute.AppendFormat("margin-top:{0}px;", offset.y);
            }
            if (offset.x != 0.0f)
            {
                switch (textAlign)
                {
                    case ImageTextAlign.None:
                    case ImageTextAlign.Right:
                        styleAttribute.AppendFormat("margin-left:{0}px;", offset.x);
                        break;
                    case ImageTextAlign.Left:
                        styleAttribute.AppendFormat("margin-right:{0}px;", offset.x);
                        break;
                }
            }
            return true;
        }

        public void changeSize(Element element, IntRect newRect, ResizeType resizeType, IntSize2 bounds)
        {
            bool changesMade = false;
            switch(textAlign)
            {
                case ImageTextAlign.None:
                case ImageTextAlign.Right:
                    if ((resizeType & ResizeType.Width) == ResizeType.Width)
                    {
                        int oldWidth = width;
                        if (fixedSize)
                        {
                            width = newRect.Width;
                        }
                        else
                        {
                            width = (int)(newRect.Width / element.OffsetParent.ClientWidth * 100.0f);
                        }
                        changesMade = true;
                    }
                    if ((resizeType & ResizeType.Left) == ResizeType.Left)
                    {
                        offset.x = newRect.Left;
                        changesMade = true;
                    }
                    break;
                case ImageTextAlign.Left:
                    if ((resizeType & ResizeType.Width) == ResizeType.Width)
                    {
                        offset.x = -newRect.Left;
                        changesMade = true;
                    }
                    if ((resizeType & ResizeType.Left) == ResizeType.Left)
                    {
                        int oldWidth = width;
                        if (fixedSize)
                        {
                            width = newRect.Width;
                        }
                        else
                        {
                            width = (int)(newRect.Width / element.OffsetParent.ClientWidth * 100.0f);
                        }
                        changesMade = true; 
                    }
                    break;
            }

            if ((resizeType & ResizeType.Top) == ResizeType.Top)
            {
                offset.y = newRect.Top;
                changesMade = true;
            }

            if (changesMade)
            {
                fireRefreshEditInterface();
            }
        }

        public Rect createCurrentRect(Element element)
        {
            float width = Width;
            if (!fixedSize)
            {
                width = element.ClientWidth;
            }
            return new Rect(offset.x, offset.y, width, 0);
        }

        [Editable]
        public bool Center
        {
            get
            {
                return center;
            }
            set
            {
                if (center != value)
                {
                    center = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (width != value)
                {
                    width = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public bool FixedSize
        {
            get
            {
                return fixedSize;
            }
            set
            {
                if (fixedSize != value)
                {
                    fixedSize = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public ImageTextAlign TextAlign
        {
            get
            {
                return textAlign;
            }
            set
            {
                if (textAlign != value)
                {
                    textAlign = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public Vector2 Offset
        {
            get
            {
                return offset;
            }
            set
            {
                if (offset != value)
                {
                    offset = value;
                    fireChanged();
                }
            }
        }
    }
}
