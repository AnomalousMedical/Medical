using Engine;
using Engine.Editing;
using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public class TextElementStyle : ElementStyleDefinition
    {
        private bool bold = false;
        private bool italic = false;
        private bool center = false;
        private Color? color = null;
        private Color? backgroundColor = null;

        public TextElementStyle(Element element)
        {
            String classes = element.GetAttributeString("class");
            if (classes != null)
            {
                String[] splitClasses = classes.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                center = splitClasses.FirstOrDefault(c => "Center".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
                italic = splitClasses.FirstOrDefault(c => "Italic".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
                bold = splitClasses.FirstOrDefault(c => "Bold".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
            }
            InlineCssParser inlineCss = new InlineCssParser(element.GetAttributeString("style"));
            if (inlineCss.contains("color"))
            {
                color = inlineCss.colorValue("color");
            }
            if (inlineCss.contains("background-color"))
            {
                backgroundColor = inlineCss.colorValue("background-color");
            }
        }

        public override bool buildClassList(StringBuilder classes)
        {
            if (bold)
            {
                classes.Append("Bold ");
            }
            if (italic)
            {
                classes.Append("Italic ");
            }
            if (center)
            {
                classes.Append("Center ");
            }
            return true;
        }

        public override bool buildStyleAttribute(StringBuilder styleAttribute)
        {
            if (color != null)
            {
                styleAttribute.AppendFormat("color:#{0:X6};", color.Value.toRGB());
            }
            if (backgroundColor != null)
            {
                styleAttribute.AppendFormat("background-color:#{0:X6};", backgroundColor.Value.toRGB());
            }
            return true;
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
        public bool Bold
        {
            get
            {
                return bold;
            }
            set
            {
                if (bold != value)
                {
                    bold = value;
                    fireChanged();
                }
            }
        }

        [Editable]
        public bool Italic
        {
            get
            {
                return italic;
            }
            set
            {
                if (italic != value)
                {
                    italic = value;
                    fireChanged();
                }
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
    }
}
