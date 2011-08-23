using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical
{
    class NumericEditDataControl : DataControl
    {
        private Edit edit;
        private NumericEdit numberEdit;
        private StaticText text;

        public NumericEditDataControl(Widget parentWidget, NumericDataField numericField)
        {
            text = (StaticText)parentWidget.createWidgetT("StaticText", "StaticText", 0, 0, 10, 15, Align.Default, "");
            text.Caption = numericField.Name;
            text.setSize(text.getTextRegion().width, text.Height);

            edit = (Edit)parentWidget.createWidgetT("Edit", "Edit", 0, 16, 100, 20, Align.Default, "");
            numberEdit = new NumericEdit(edit);
            numberEdit.AllowFloat = numericField.AllowDecimalPlaces;
            numberEdit.MinValue = (float)numericField.MinValue;
            numberEdit.MaxValue = (float)numericField.MaxValue;
            numberEdit.DecimalValue = numericField.CurrentValue;
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(edit);
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
            text.setPosition((int)Location.x, (int)Location.y);
            text.setSize((int)WorkingSize.Width, text.Height);
            edit.setPosition((int)Location.x, (int)text.Bottom);
            edit.setSize((int)WorkingSize.Width, (int)WorkingSize.Height - text.Height);
        }

        public override Size2 DesiredSize
        {
            get 
            {  
                float width = text.Width;
                if(edit.Width > width)
                {
                    width = edit.Width;
                }
                float height = text.Height + edit.Height;
                return new Size2(width, height);
            }
        }

        public override bool Visible
        {
            get
            {
                return edit.Visible;
            }
            set
            {
                edit.Visible = text.Visible = value;
            }
        }
    }
}
