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

        public TextVariableEditor(TextVariableTextBody textBody, Widget widget, DataRetriever dataRetriever)
            : this(textBody, widget)
        {
            StringBuilder sb = new StringBuilder();
            foreach(String section in dataRetriever.ExamSections)
            {
                sb.Append(".");
                sb.Append(section);
            }
            sb.Remove(0, 1);
            dataFieldInfo = new DataFieldInfo(sb.ToString(), dataRetriever.DataPoint);
            name.Caption = dataFieldInfo.ToString();
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
                return null;
            }
        }

        void insert_MouseButtonClick(Widget source, EventArgs e)
        {
            String variableName = VariableName;
            if (variableName != null)
            {
                textBody.insertVariableString(variableName);
            }
        }

        void remove_MouseButtonClick(Widget source, EventArgs e)
        {
            textBody.removeVariable(this);
            Dispose();
        }

        void find_MouseButtonClick(Widget source, EventArgs e)
        {
            String variableName = VariableName;
            if (variableName != null)
            {
                textBody.findNextInstance(variableName);
            }
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
