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
    }
}
