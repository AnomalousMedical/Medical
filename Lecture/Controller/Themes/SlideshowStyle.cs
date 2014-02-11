using Engine;
using Engine.Editing;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture
{
    class SlideshowStyle : StyleDefinition
    {
        private Color? color = null;
        private Color? backgroundColor = null;
        private int? fontSize = null;
        private int? marginLeft = null;
        private int? marginRight = null;
        private int? marginTop = null;
        private int? marginBottom = null;

        public SlideshowStyle(String name, String css)
        {
            this.Name = name;
            RuleCssParser cssRules = new RuleCssParser(css);
            CssRule body = cssRules["body"];
            if (body != null)
            {
                Color = body.colorValue("color");
                Background = body.colorValue("background-color");
                FontSize = body.intValue("font-size");
            }
            CssRule content = cssRules["#Content"];
            if (content != null)
            {
                marginLeft = content.intValue("margin-left");
                marginRight = content.intValue("margin-right");
                marginTop = content.intValue("margin-top");
                marginBottom = content.intValue("margin-bottom");
            }
        }

        public bool buildStyleSheet(StringBuilder styleAttribute)
        {
            styleAttribute.Append("body{");
            if (color != null)
            {
                styleAttribute.AppendFormat("color:#{0:X6};", color.Value.toRGB());
            }
            if (backgroundColor != null)
            {
                styleAttribute.AppendFormat("background-color:#{0:X6};", backgroundColor.Value.toRGB());
            }
            if (fontSize != null)
            {
                styleAttribute.AppendFormat("font-size:{0}px;", fontSize);
            }
            styleAttribute.Append("}");

            styleAttribute.Append("#Content{");
            if (marginLeft != null)
            {
                styleAttribute.AppendFormat("margin-left:{0}px;", marginLeft);
            }
            if (marginRight != null)
            {
                styleAttribute.AppendFormat("margin-right:{0}px;", marginRight);
            }
            if (marginTop != null)
            {
                styleAttribute.AppendFormat("margin-top:{0}px;", marginTop);
            }
            if (marginBottom != null)
            {
                styleAttribute.AppendFormat("margin-bottom:{0}px;", marginBottom);
            }
            styleAttribute.Append("}");
            return true;
        }

        public String Name { get; private set; }

        protected override string EditInterfaceName
        {
            get
            {
                return Name;
            }
        }

        [Editable]
        public Color? Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color != value)
                {
                    color = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public Color? Background
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public int? FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                if (fontSize != value)
                {
                    fontSize = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public int? LeftMargin
        {
            get
            {
                return marginLeft;
            }
            set
            {
                if (marginLeft != value)
                {
                    marginLeft = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public int? RightMargin
        {
            get
            {
                return marginRight;
            }
            set
            {
                if (marginRight != value)
                {
                    marginRight = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public int? TopMargin
        {
            get
            {
                return marginTop;
            }
            set
            {
                if (marginTop != value)
                {
                    marginTop = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public int? BottomMargin
        {
            get
            {
                return marginBottom;
            }
            set
            {
                if (marginBottom != value)
                {
                    marginBottom = value;
                    fireChanged();
                }
            }
        }
    }
}
