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
            text.insertText(variableText, text.TextCursor);
        }

        public void findNextInstance(String variableText)
        {
            String currentText = text.OnlyText;
            uint cursorPos = text.TextCursor;
            int nextPosition = currentText.IndexOf(variableText, (int)cursorPos);
            if (nextPosition < 0)
            {
                nextPosition = currentText.IndexOf(variableText);
                if (nextPosition < 0)
                {
                    MessageBox.show(String.Format("The value '{0}' is not in this text.", variableText), "Find", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo);
                }
                if (nextPosition == cursorPos)
                {
                    MessageBox.show(String.Format("No more occurances of '{0}' found.", variableText), "Find", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo);
                }
                else
                {
                    text.setTextSelection((uint)nextPosition, (uint)nextPosition + 3);
                    InputManager.Instance.setKeyFocusWidget(text);
                }
            }
            else
            {
                text.setTextSelection((uint)nextPosition, (uint)nextPosition + 3);
                InputManager.Instance.setKeyFocusWidget(text);
            }
        }

        public void removeVariable(TextVariableEditor variable)
        {
            int index = variables.IndexOf(variable);
            if (index != -1)
            {
                object[] remapVars = new object[variables.Count];
                for (int i = 0; i < index; ++i)
                {
                    remapVars[i] = String.Format("{{{0}}}", i);
                }
                //Need to update all other existing variables and text with certain variables in it.
                variables.RemoveAt(index);
                remapVars[index] = "|Removed|";
                for (int i = index; i < variables.Count; ++i)
                {
                    String newVar = String.Format("{{{0}}}", i);
                    remapVars[i + 1] = newVar;
                    variables[i].VariableText = newVar;
                }
                text.OnlyText = String.Format(text.OnlyText, remapVars);
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

        void text_EventEditTextChange(Widget source, EventArgs e)
        {
            if (TextHeight != lastTextHeight)
            {
                requestLayout();
            }
        }

        void addVariable_MouseButtonClick(Widget source, EventArgs e)
        {
            addVariable(new TextVariableEditor(String.Format("{{{0}}}", variables.Count), this, Widget));
        }
    }
}
