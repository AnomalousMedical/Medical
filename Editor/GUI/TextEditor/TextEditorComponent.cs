using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.GUI.AnomalousMvc;
using Irony.Parsing;
using Medical.Irony;

namespace Medical.GUI
{
    class TextEditorComponent : LayoutComponent
    {
        private EditBox text;
        private bool allowColorString = true;

        public TextEditorComponent(MyGUIViewHost viewHost, TextEditorView view)
            : base("Medical.GUI.TextEditor.TextEditorComponent.layout", viewHost)
        {
            text = (EditBox)widget.findWidget("Text");
            text.EventEditTextChange += new MyGUIEvent(text_EventEditTextChange);
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
                String cleanedValue = cleanStringForMyGUI(value);
                cleanedValue = colorString(cleanedValue);
                allowColorString = false;
                text.Caption = cleanedValue;
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
            text.Caption = colorString(text.OnlyText);
            text.TextCursor = cursor;
        }

        private String cleanStringForMyGUI(String input)
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
            return sb.ToString();
        }

        private String colorString(String input)
        {
            if (allowColorString)
            {
                LanguageData language = new LanguageData(new XmlGrammar());
                Parser parser = new Parser(language);
                ParseTree parseTree = parser.Parse(input);

                int additionalOffset = 0;
                foreach (Token token in parseTree.Tokens)
                {
                    int tokenStart = token.Location.Position;
                    int tokenEnd = tokenStart + token.Length;

                    //yep as slow and shitty as it looks
                    input = input.Insert(tokenStart + additionalOffset, getColor(token));
                    additionalOffset += 7;
                    input = input.Insert(tokenEnd + additionalOffset, "#000000");
                    additionalOffset += 7;
                }
            }
            return input;
        }

        private String getColor(Token token)
        {
            if (token.EditorInfo != null)
            {
                switch (token.EditorInfo.Color)
                {
                    case TokenColor.Comment:
                        return "#348000";
                    case TokenColor.Identifier:
                        return "#800000";
                    case TokenColor.Keyword:
                        return "#800000";
                    case TokenColor.Number:
                        return "#000000";
                    case TokenColor.String:
                        return "#0034FF";
                    case TokenColor.Text:
                        return "#0034FF";
                    default:
                        return "#000000";
                }
            }
            else
            {
                return "#000000";
            }
        }
    }
}
