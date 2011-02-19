using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class PromptAnswerTextArea : PromptTextArea
    {
        private ScrollView parent;
        private Button answerButton;
        private MyGUIQuestionProvider questionProvider;
        private PromptAnswer answer;

        public PromptAnswerTextArea(MyGUIQuestionProvider questionProvider, PromptAnswer answer, ScrollView parent, int left, int right, int top, bool light)
        {
            this.questionProvider = questionProvider;
            this.parent = parent;
            this.answer = answer;

            int textAreaWidth = right - left;

            String skin = "DarkPromptAnswerTextArea";
            if (light)
            {
                skin = "PromptAnswerTextArea";
            }

            answerButton = parent.createWidgetT("Button", skin, left, top, textAreaWidth, 0, Align.Left | Align.Top, "") as Button;
            answerButton.SubWidgetText.setWordWrap(true);
            answerButton.Caption = answer.Text;
            answerButton.Font = "font_DejaVuSans.Large_Question";
            answerButton.TextAlign = Align.VCenter | Align.Left;
            Size2 textSize = answerButton.getTextSize();
            answerButton.setSize(textAreaWidth, (int)textSize.Height + EXTRA_HEIGHT);

            answerButton.MouseButtonClick += new MyGUIEvent(answerButton_MouseButtonClick);
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(answerButton);
        }

        public override int Top
        {
            get
            {
                return answerButton.Top;
            }
            set
            {
                answerButton.setPosition(answerButton.Left, value);
            }
        }

        public override int Bottom
        {
            get
            {
                return answerButton.Top + answerButton.Height;
            }
        }

        void answerButton_MouseButtonClick(Widget source, EventArgs e)
        {
            questionProvider.answerSelected(answer);
        }
    }
}
