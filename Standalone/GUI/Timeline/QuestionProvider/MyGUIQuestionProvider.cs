using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class MyGUIQuestionProvider : Dialog, IQuestionProvider
    {
        private ScrollView questionScroll;

        public MyGUIQuestionProvider()
            :base("Medical.GUI.Timeline.QuestionProvider.MyGUIQuestionProvider.layout")
        {
            questionScroll = window.findWidget("QuestionScroll") as ScrollView;
        }

        public void addQuestion(PromptQuestion question)
        {
            PromptTextArea textArea = new PromptTextArea(question.Text, questionScroll, window.Width);

            Size2 canvasSize = questionScroll.CanvasSize;
            questionScroll.CanvasSize = new Size2(window.Width, textArea.Bottom);
        }

        public void showPrompt(PromptAnswerSelected answerSelectedCallback)
        {
            this.open(true);
        }

        public void clear()
        {
            
        }
    }
}
