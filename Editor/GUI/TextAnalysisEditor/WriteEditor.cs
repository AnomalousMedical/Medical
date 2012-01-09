using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.Exam;
using MyGUIPlugin;

namespace Medical.GUI
{
    class WriteEditor : AnalysisEditorComponent
    {
        private Edit text;
        private int extraHeight;
        private int lastTextHeight;

        public WriteEditor(AnalysisEditorComponentParent parent)
            : base("Medical.GUI.TextAnalysisEditor.WriteEditor.layout", parent)
        {
            text = (Edit)widget.findWidget("Text");
            text.EventEditTextChange += new MyGUIEvent(text_EventEditTextChange);

            extraHeight = widget.Height - text.Height;
        }

        public override void layout(int left, int top, int width)
        {
            widget.setCoord(left, top, width, 100);
            int newTextHeight = TextHeight;
            text.setSize(text.Width, newTextHeight);
            lastTextHeight = newTextHeight;
            widget.setSize(width, text.Height + extraHeight);
        }

        void text_EventEditTextChange(Widget source, EventArgs e)
        {
            if (TextHeight != lastTextHeight)
            {
                requestLayout();
            }
        }

        public int TextHeight
        {
            get
            {
                int height = (int)text.getTextSize().Height + 5;
                if (height < 25)
                {
                    return 25;
                }
                return height;
            }
        }
    }
}
