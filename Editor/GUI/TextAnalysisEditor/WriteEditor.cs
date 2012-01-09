using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.Exam;
using MyGUIPlugin;

namespace Medical.GUI
{
    class WriteEditor : AnalysisEditorComponent, TextVariableTextBody
    {
        private Edit text;
        private int extraHeight;
        private int lastTextHeight;
        private int variableLeft;

        private List<TextVariableEditor> variables = new List<TextVariableEditor>();

        public WriteEditor(AnalysisEditorComponentParent parent)
            : base("Medical.GUI.TextAnalysisEditor.WriteEditor.layout", parent)
        {
            text = (Edit)widget.findWidget("Text");
            text.EventEditTextChange += new MyGUIEvent(text_EventEditTextChange);

            Button addVariable = (Button)widget.findWidget("AddVariable");
            addVariable.MouseButtonClick += new MyGUIEvent(addVariable_MouseButtonClick);
            variableLeft = addVariable.Left;

            extraHeight = widget.Height - text.Bottom;
        }

        public override void layout(int left, int top, int width)
        {
            widget.setCoord(left, top, width, 100);
            int newTextHeight = TextHeight;
            text.setSize(text.Width, newTextHeight);
            lastTextHeight = newTextHeight;

            int currentTop = text.Bottom;
            int variableWidth = width - variableLeft;
            foreach (TextVariableEditor variable in variables)
            {
                variable.layout(variableLeft, currentTop, variableWidth);
                currentTop += variable.Height;
            }

            widget.setSize(width, currentTop + extraHeight);
        }

        public void addVariable(TextVariableEditor variable)
        {
            variables.Add(variable);
            requestLayout();
        }

        public void insertVariableString(string variableText)
        {
            text.addText(variableText);
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

        void text_EventEditTextChange(Widget source, EventArgs e)
        {
            if (TextHeight != lastTextHeight)
            {
                requestLayout();
            }
        }

        void addVariable_MouseButtonClick(Widget source, EventArgs e)
        {
            addVariable(new TextVariableEditor(this, Widget));
        }
    }
}
