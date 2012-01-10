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
        private String variableText;
        private StaticText name;
        private StaticText data;

        public TextVariableEditor(String variableText, TextVariableTextBody textBody, Widget parent)
            : base("Medical.GUI.TextAnalysisEditor.TextVariableEditor.layout", parent)
        {
            this.textBody = textBody;
            this.variableText = variableText;

            name = (StaticText)widget.findWidget("Name");
            name.Caption = VariableText;

            data = (StaticText)widget.findWidget("Data");

            Button insert = (Button)widget.findWidget("Insert");
            insert.MouseButtonClick += new MyGUIEvent(insert_MouseButtonClick);

            Button remove = (Button)widget.findWidget("Remove");
            remove.MouseButtonClick += new MyGUIEvent(remove_MouseButtonClick);

            Button find = (Button)widget.findWidget("Find");
            find.MouseButtonClick += new MyGUIEvent(find_MouseButtonClick);

            Button choose = (Button)widget.findWidget("Choose");
            choose.MouseButtonClick += new MyGUIEvent(choose_MouseButtonClick);
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

        public String VariableText
        {
            get
            {
                return variableText;
            }
            set
            {
                variableText = value;
                name.Caption = variableText;
            }
        }

        void insert_MouseButtonClick(Widget source, EventArgs e)
        {
            textBody.insertVariableString(VariableText);
        }

        void remove_MouseButtonClick(Widget source, EventArgs e)
        {
            textBody.removeVariable(this);
            Dispose();
        }

        void find_MouseButtonClick(Widget source, EventArgs e)
        {
            textBody.findNextInstance(VariableText);
        }

        void choose_MouseButtonClick(Widget source, EventArgs e)
        {
            textBody.openVariableBrowser(delegate(DataFieldInfo variable)
            {
                data.Caption = variable.ToString();
            });
        }
    }
}
