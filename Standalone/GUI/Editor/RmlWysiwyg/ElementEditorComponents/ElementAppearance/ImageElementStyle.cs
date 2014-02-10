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
        private int? textSideMargin = 0;
        private int? textBottomMargin = 0;
        private bool _break = false;

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
                _break = splitClasses.FirstOrDefault(c => "Break".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
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
                    textSideMargin = inlineCss.intValue("margin-right");
                    break;
                case ImageTextAlign.Left:
                    x = inlineCss.intValue("margin-right");
                    textSideMargin = inlineCss.intValue("margin-left");
                    break;
            }
            textBottomMargin = inlineCss.intValue("margin-bottom");
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
            if (_break)
            {
                classes.Append("Break ");
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
            if (textSideMargin.HasValue)
            {
                switch (textAlign)
                {
                    case ImageTextAlign.None:
                    case ImageTextAlign.Right:
                        styleAttribute.AppendFormat("margin-right:{0}px;", textSideMargin.Value);
                        break;
                    case ImageTextAlign.Left:
                        styleAttribute.AppendFormat("margin-left:{0}px;", textSideMargin.Value);
                        break;
                }
            }
            if (textBottomMargin.HasValue)
            {
                styleAttribute.AppendFormat("margin-bottom:{0}px;", textBottomMargin.Value);
            }
            return true;
        }

        public void changeSize(Element element, IntRect newRect, ResizeType resizeType, IntSize2 bounds)
        {
            bool changesMade = false;
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
                if (offset.x < 0)
                {
                    offset.x = 0;
                }
                changesMade = true;
            }
            if ((resizeType & ResizeType.Top) == ResizeType.Top)
            {
                offset.y = newRect.Top;
                if (offset.y < 0)
                {
                    offset.y = 0;
                }
                changesMade = true;
            }

            if (changesMade)
            {
                fireRefreshEditInterface();
            }
        }

        public Rect createCurrentRect(Element element, out bool leftAnchor)
        {
            leftAnchor = textAlign != ImageTextAlign.Left;
            float width = Width;
            if (!fixedSize)
            {
                width = element.ClientWidth;
            }
            if (textAlign == ImageTextAlign.Left)
            {
                return new Rect(offset.x, element.ClientLeft, width, 0);
            }
            else
            {
                return new Rect(offset.x, offset.y, width, 0);
            }
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

        [EditableMinMax(0, int.MaxValue, 1)]
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

        [EditableMinMax(0, int.MaxValue, 1)]
        public int? TextSideMargin
        {
            get
            {
                return textSideMargin;
            }
            set
            {
                if (textSideMargin != value)
                {
                    textSideMargin = value;
                    fireChanged();
                }
            }
        }

        [EditableMinMax(0, int.MaxValue, 1)]
        public int? TextBottomMargin
        {
            get
            {
                return textBottomMargin;
            }
            set
            {
                if (textBottomMargin != value)
                {
                    textBottomMargin = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public bool Break
        {
            get
            {
                return _break;
            }
            set
            {
                if (_break != value)
                {
                    _break = value;
                    fireChanged();
                }
            }
        }
    }
}
