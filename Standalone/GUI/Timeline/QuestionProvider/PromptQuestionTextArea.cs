using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class PromptQuestionTextArea : PromptTextArea
    {
        private ScrollView parent;
        private StaticText questionText;

        public PromptQuestionTextArea(String text, ScrollView parent, int left, int right, int top)
        {
            this.parent = parent;

            int textAreaWidth = right - left;

            questionText = parent.createWidgetT("StaticText", "PromptQuestionTextArea", left, top, textAreaWidth, 0, Align.Stretch, "") as StaticText;
            questionText.SubWidgetText.setWordWrap(true);
            questionText.Caption = text;
            Size2 textSize = questionText.getTextSize();
            questionText.setSize(textAreaWidth, (int)textSize.Height + EXTRA_HEIGHT);
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(questionText);
        }

        public override int Top
        {
            get
            {
                return questionText.Top;
            }
            set
            {
                questionText.setPosition(questionText.Left, value);
            }
        }

        public override int Bottom
        {
            get
            {
                return questionText.Top + questionText.Height;
            }
        }
    }
}
