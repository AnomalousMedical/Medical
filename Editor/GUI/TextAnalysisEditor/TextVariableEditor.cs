using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TextVariableEditor : Component
    {
        private TextVariableTextBody textBody;

        public TextVariableEditor(TextVariableTextBody textBody, Widget parent)
            : base("Medical.GUI.TextAnalysisEditor.TextVariableEditor.layout", parent)
        {
            this.textBody = textBody;
        }

        public void layout(int left, int top, int width)
        {
            widget.setCoord(left, top, width, widget.Height);
        }

        public int Height
        {
            get
            {
                return widget.Height;
            }
        }
    }
}
