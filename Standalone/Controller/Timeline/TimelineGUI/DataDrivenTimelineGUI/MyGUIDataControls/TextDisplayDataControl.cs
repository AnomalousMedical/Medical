using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Medical.GUI;

namespace Medical
{
    class TextDisplayDataControl : DataControl
    {
        private static MyGUIDynamicFontManager timelineText = new MyGUIDynamicFontManager();
        private int indentation;

        static TextDisplayDataControl()
        {
            timelineText.addFont("TimelineText.10", 10);
            timelineText.addFont("TimelineText.25", 25);
            timelineText.addFont("TimelineText.50", 50);
            timelineText.addFont("TimelineText.100", 100);
        }

        private EditBox text;

        public TextDisplayDataControl(Widget parentWidget, StaticTextDataField field)
        {
            text = (EditBox)parentWidget.createWidgetT("Edit", "WordWrapSimple", 0, 16, 100, 20, Align.Default, "");
            text.EditMultiLine = true;
            text.EditWordWrap = true;
            text.EditStatic = true;
            text.NeedMouseFocus = false;
            text.Caption = field.Text;
            text.Font = timelineText.getFont(field.FontHeight);
            text.FontHeight = field.FontHeight;
            this.indentation = field.Indentation;
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(text);
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            
        }

        public override void bringToFront()
        {
            
        }

        public override void setAlpha(float alpha)
        {
            
        }

        public override void layout()
        {
            text.setSize((int)WorkingSize.Width - indentation, (int)WorkingSize.Height);
            int textHeight = (int)text.getTextSize().Height +(text.Height - text.ClientCoord.height);
            text.setCoord((int)Location.x + indentation, (int)Location.y, (int)WorkingSize.Width - indentation, textHeight);
            Height = textHeight;
        }

        public override Size2 DesiredSize
        {
            get
            {
                return text.getTextSize();
            }
        }

        public override bool Visible
        {
            get
            {
                return text.Visible;
            }
            set
            {
                text.Visible = value;
            }
        }
    }
}
