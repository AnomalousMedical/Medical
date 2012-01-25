using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical
{
    class TextDisplayDataControl : DataControl
    {
        private Edit text;

        public TextDisplayDataControl(Widget parentWidget, StaticTextDataField field)
        {
            text = (Edit)parentWidget.createWidgetT("Edit", "WordWrapSimple", 0, 16, 100, 20, Align.Default, "");
            text.EditMultiLine = true;
            text.EditWordWrap = true;
            text.EditStatic = true;
            text.NeedMouseFocus = false;
            text.ForwardMouseWheelToParent = true;
            text.Caption = field.Text;
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
            text.setSize((int)WorkingSize.Width, (int)WorkingSize.Height);
            int textHeight = (int)text.getTextSize().Height +(text.Height - text.ClientCoord.height);
            text.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, textHeight);
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
