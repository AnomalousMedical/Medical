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
        private EditBox text;
        private int extraHeight;
        private int lastTextHeight;
        private int variableLeft;

        private List<TextVariableEditor> variables = new List<TextVariableEditor>();

        public WriteEditor(AnalysisEditorComponentParent parent)
            : base("Medical.GUI.TextAnalysisEditor.WriteEditor.layout", parent)
        {
            text = (EditBox)widget.findWidget("Text");
            text.EventEditTextChange += new MyGUIEvent(text_EventEditTextChange);

            Button addVariable = (Button)widget.findWidget("AddVariable");
            addVariable.MouseButtonClick += new MyGUIEvent(addVariable_MouseButtonClick);
            variableLeft = addVariable.Left;

            extraHeight = widget.Height - text.Bottom;
        }

        public WriteEditor(AnalysisEditorComponent parent, Write writeAction)
            :this(parent)
        {
            Object[] variableNames = new Object[writeAction.NumData];
            int i = 0;
            foreach (DataRetriever dataRetriever in writeAction.Data)
            {
                TextVariableEditor variable = new TextVariableEditor(this, widget, dataRetriever);
                variables.Add(variable);
                variableNames[i++] = variable.VariableName;
            }
            text.OnlyText = String.Format(writeAction.Text, variableNames);
        }

        public override void Dispose()
        {
            foreach (TextVariableEditor variable in variables)
            {
                variable.Dispose();
            }
            base.Dispose();
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

        public override AnalysisAction createAction()
        {
            String formatWriteText = text.OnlyText;
            DataRetriever[] dataRetrievers = new DataRetriever[variables.Count];

            for (int i = 0; i < variables.Count; ++i)
            {
                TextVariableEditor variable = variables[i];
                formatWriteText = formatWriteText.Replace(variable.VariableName, String.Format("{{{0}}}", i));
                dataRetrievers[i] = variable.createDataRetriever();
            }

            return new Write(formatWriteText, dataRetrievers);
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
            int selectionLength = variableText.Length;
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
                    text.setTextSelection((uint)nextPosition, (uint)(nextPosition + selectionLength));
                    InputManager.Instance.setKeyFocusWidget(text);
                }
            }
            else
            {
                text.setTextSelection((uint)nextPosition, (uint)(nextPosition + selectionLength));
                InputManager.Instance.setKeyFocusWidget(text);
            }
        }

        public void removeVariable(TextVariableEditor variable)
        {
            variables.Remove(variable);
            requestLayout();
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
