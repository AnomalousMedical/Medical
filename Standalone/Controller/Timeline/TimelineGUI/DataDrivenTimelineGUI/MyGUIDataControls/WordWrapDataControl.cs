using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical
{
    class WordWrapDataControl : DataControl
    {
        private EditBox edit;
        private TextBox text;
        private String valueName;

        public WordWrapDataControl(Widget parentWidget, NotesDataField field)
        {
            text = (TextBox)parentWidget.createWidgetT("TextBox", "TextBox", 0, 0, 10, 15, Align.Default, "");
            text.Caption = field.Name;
            text.setSize(text.getTextRegion().width, text.Height);

            edit = (EditBox)parentWidget.createWidgetT("Edit", "WordWrapWhite", 0, 16, 100, 20, Align.Default, "");
            edit.EditMultiLine = true;
            edit.EditWordWrap = true;
            edit.TextAlign = Align.Left | Align.Top;
            int height = edit.FontHeight * field.NumberOfLines;
            edit.setSize(edit.Width, height);

            valueName = field.Name;
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(edit);
            Gui.Instance.destroyWidget(text);
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            examSection.setValue<String>(valueName, edit.OnlyText);
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            edit.OnlyText = examSection.getValue<String>(valueName, edit.OnlyText);
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
            Height = (int)WorkingSize.Height;
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
