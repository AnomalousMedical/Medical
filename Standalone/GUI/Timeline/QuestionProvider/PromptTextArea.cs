using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class PromptTextArea
    {
        private const int EXTRA_HEIGHT = 5;

        private ScrollView parent;
        private String text;
        private Edit questionEdit;

        public PromptTextArea(String text, ScrollView parent, int questionWidth)
        {
            this.parent = parent;
            this.text = text;

            questionEdit = parent.createWidgetT("Edit", "WordWrapSimple", 0, 0, questionWidth, 0, Align.Stretch, "") as Edit;
            questionEdit.Caption = text;
            questionEdit.EditReadOnly = true;
            Size2 textSize = questionEdit.getTextSize();
            questionEdit.setSize(questionWidth, (int)textSize.Height + EXTRA_HEIGHT);
        }

        public int Bottom
        {
            get
            {
                return questionEdit.Top + questionEdit.Height;
            }
        }
    }
}
