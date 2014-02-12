using Engine;
using Engine.Editing;
using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public class TextElementStyle : ElementStyleDefinition, HighlightProvider
    {
        private bool bold = false;
        private bool italic = false;
        private bool center = false;
        private Color? color = null;
        private Color? backgroundColor = null;
        private int? fontSize = null;
        private int? marginTop = null;
        private int? marginBottom = null;
        private bool _break = false;
        private bool flow = false;

        private String marginSource = "margin";
        private bool usePadding = false;

        public TextElementStyle(Element element, bool usePadding)
        {
            this.usePadding = usePadding;
            if (usePadding)
            {
                marginSource = "padding";
            }
            String classes = element.GetAttributeString("class");
            if (classes != null)
            {
                String[] splitClasses = classes.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                center = splitClasses.FirstOrDefault(c => "Center".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
                italic = splitClasses.FirstOrDefault(c => "Italic".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
                bold = splitClasses.FirstOrDefault(c => "Bold".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
                _break = splitClasses.FirstOrDefault(c => "Break".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
                flow = splitClasses.FirstOrDefault(c => "Flow".Equals(c, StringComparison.InvariantCultureIgnoreCase)) != null;
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
            if (inlineCss.contains("font-size"))
            {
                fontSize = inlineCss.intValue("font-size");
            }
            marginTop = inlineCss.intValue(marginSource + "-top");
            marginBottom = inlineCss.intValue(marginSource + "-bottom");
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
            if (_break)
            {
                classes.Append("Break ");
            }
            if (flow)
            {
                classes.Append("Flow ");
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
            if (fontSize != null)
            {
                styleAttribute.AppendFormat("font-size:{0}px;", fontSize);
            }
            if (marginTop != null)
            {
                styleAttribute.AppendFormat(marginSource + "-top:{0}px;", marginTop);
            }
            if (marginBottom != null)
            {
                styleAttribute.AppendFormat(marginSource + "-bottom:{0}px;", marginBottom);
            }
            return true;
        }

        public void changeSize(IntRect newRect, ResizeType resizeType, IntSize2 bounds)
        {
            bool changesMade = false;
            if ((resizeType & ResizeType.Top) == ResizeType.Top)
            {
                marginTop = newRect.Top;
                if (marginTop < 0)
                {
                    marginTop = 0;
                }
                changesMade = true;
            }

            if ((resizeType & ResizeType.Height) == ResizeType.Height)
            {
                marginBottom = newRect.Height;
                if (marginBottom < 0)
                {
                    marginBottom = 0;
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
            leftAnchor = true;
            return new Rect(0, marginTop.GetValueOrDefault(), 0, marginBottom.GetValueOrDefault());
        }

        [Editable(PrettyName = "Font Color")]
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

        [Editable(PrettyName="Background Color")]
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

        [EditableMinMax(0, 250, 1, PrettyName="Font Size")]
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

        [Editable(Advanced=true)]
        public bool Flow
        {
            get
            {
                return flow;
            }
            set
            {
                if (flow != value)
                {
                    flow = value;
                    fireChanged();
                }
            }
        }

        [EditableMinMax(0, int.MaxValue, 1, Advanced = true, PrettyName="Top Margin")]
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

        [EditableMinMax(0, int.MaxValue, 1, Advanced = true, PrettyName = "Bottom Margin")]
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

        public IntRect getAdditionalHighlightAreaRect(Element element)
        {
            if (usePadding)
            {
                return new IntRect(0, marginTop.GetValueOrDefault(), 0, -marginTop.GetValueOrDefault());
            }
            else
            {
                return new IntRect(0, 0, 0, marginBottom.GetValueOrDefault());
            }
        }
    }
}
