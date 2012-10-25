using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.GUI.AnomalousMvc;

namespace Medical.GUI
{
    class TextEditorComponent : LayoutComponent
    {
        private EditBox text;
        private bool allowColorString = true;
        private TextHighlighter textHighlighter = null;

        public TextEditorComponent(MyGUIViewHost viewHost, TextEditorView view)
            : base("Medical.GUI.TextEditor.TextEditorComponent.layout", viewHost)
        {
            text = (EditBox)widget.findWidget("Text");

            this.textHighlighter = view.TextHighlighter;
            if (textHighlighter != null)
            {
                text.EventEditTextChange += new MyGUIEvent(text_EventEditTextChange);
            }

            Text = view.Text;
            MaxLength = view.MaxLength;
            WordWrap = view.WordWrap;
        }

        public void cut()
        {
            text.cut();
        }

        public void copy()
        {
            text.copy();
        }

        public void paste()
        {
            text.paste();
        }

        public void selectAll()
        {
            text.setTextSelection(0, uint.MaxValue);
        }

        internal void insertText(String insert)
        {
            text.insertText(insert, text.TextCursor);
        }

        public String Text
        {
            get
            {
                return text.OnlyText;
            }
            set
            {
                StringBuilder cleanedValue = cleanStringForMyGUI(value);
                colorString(cleanedValue);
                allowColorString = false;
                text.Caption = cleanedValue.ToString();
                allowColorString = true;
            }
        }

        public uint MaxLength
        {
            get
            {
                return text.MaxTextLength;
            }
            set
            {
                text.MaxTextLength = value;
            }
        }

        public bool WordWrap
        {
            get
            {
                return text.EditWordWrap;
            }
            set
            {
                text.EditWordWrap = value;
            }
        }

        void text_EventEditTextChange(Widget source, EventArgs e)
        {
            uint cursor = text.TextCursor;
            StringBuilder sb = new StringBuilder(text.OnlyText.Replace("#", "##"));
            colorString(sb);
            text.Caption = sb.ToString();
            text.TextCursor = cursor;
        }

        private StringBuilder cleanStringForMyGUI(String input)
        {
            StringBuilder sb = new StringBuilder(input.Length + 100);
            for (int i = 0; i < input.Length; ++i)
            {
                switch(input[i])
                {
                    case '\r':
                        //Do nothing
                        break;
                    case '#':
                        sb.Append("##");
                        break;
                    default:
                        sb.Append(input[i]);
                        break;

                }
            }
            return sb;
        }

        private void colorString(StringBuilder input)
        {
            if (allowColorString && textHighlighter != null)
            {
                textHighlighter.colorString(input);
            }
        }
    }
}
