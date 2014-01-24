using Engine;
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
        private bool center = false;
        private bool fixedSize = true;
        private int width = 200;

        public ImageElementStyle(Element imageElement)
        {
            String classes = imageElement.GetAttributeString("class");
            if (classes != null)
            {
                String[] splitClasses = classes.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                center = splitClasses.FirstOrDefault(c => "Center".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
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
        }

        public override bool buildClassList(StringBuilder classes)
        {
            if (center)
            {
                classes.Append("Center ");
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
            return true;
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
    }
}
