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

        public PromptAnswerTextArea(MyGUIQuestionProvider questionProvider, String text, ScrollView parent, int left, int right, int top)
        {
            this.questionProvider = questionProvider;
            this.parent = parent;

            int textAreaWidth = right - left;

            answerButton = parent.createWidgetT("Button", "PromptAnswerTextArea", left, top, textAreaWidth, 0, Align.Stretch, "") as Button;
            answerButton.SubWidgetText.setWordWrap(true);
            answerButton.Caption = text;
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
            questionProvider.questionSelected(this);
        }
    }
}
