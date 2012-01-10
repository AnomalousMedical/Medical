using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.Exam;

namespace Medical.GUI
{
    class TextVariableEditor : Component
    {
        private TextVariableTextBody textBody;
        private String variableText;
        private StaticText name;
        private DataFieldInfo dataFieldInfo = null;

        public TextVariableEditor(TextVariableTextBody textBody, Widget parent)
            : base("Medical.GUI.TextAnalysisEditor.TextVariableEditor.layout", parent)
        {
            this.textBody = textBody;

            name = (StaticText)widget.findWidget("Name");

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

        public DataRetriever createDataRetriever()
        {
            if (dataFieldInfo != null)
            {
                return dataFieldInfo.createDataRetriever();
            }
            return null;
        }

        public int Height
        {
            get
            {
                return widget.Height;
            }
        }

        public String VariableName
        {
            get
            {
                if (dataFieldInfo != null)
                {
                    return String.Format("$({0})", dataFieldInfo.FullName);
                }
                return "";
            }
        }

        void insert_MouseButtonClick(Widget source, EventArgs e)
        {
            textBody.insertVariableString(VariableName);
        }

        void remove_MouseButtonClick(Widget source, EventArgs e)
        {
            textBody.removeVariable(this);
            Dispose();
        }

        void find_MouseButtonClick(Widget source, EventArgs e)
        {
            textBody.findNextInstance(VariableName);
        }

        void choose_MouseButtonClick(Widget source, EventArgs e)
        {
            textBody.openVariableBrowser(delegate(DataFieldInfo variable)
            {
                dataFieldInfo = variable;
                name.Caption = variable.ToString();
            });
        }
    }
}
