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
        private List<PromptTextArea> textAreas = new List<PromptTextArea>();

        public MyGUIQuestionProvider()
            :base("Medical.GUI.Timeline.QuestionProvider.MyGUIQuestionProvider.layout")
        {
            questionScroll = window.findWidget("QuestionScroll") as ScrollView;
        }

        public void addQuestion(PromptQuestion question)
        {
            int verticalPosition = textAreas.Count > 0 ? textAreas[textAreas.Count - 1].Bottom : 0;
            PromptTextArea questionTextArea = new PromptQuestionTextArea(question.Text, questionScroll, 0, window.Width, verticalPosition);
            textAreas.Add(questionTextArea);
            verticalPosition = questionTextArea.Bottom;

            PromptTextArea answerTextArea = null;
            foreach (PromptAnswer answer in question.Answers)
            {
                answerTextArea = new PromptAnswerTextArea(this, answer.Text, questionScroll, 25, window.Width, verticalPosition);
                textAreas.Add(answerTextArea);
                verticalPosition = answerTextArea.Bottom;
            }

            Size2 canvasSize = questionScroll.CanvasSize;
            questionScroll.CanvasSize = new Size2(window.Width, verticalPosition);
        }

        public void showPrompt(PromptAnswerSelected answerSelectedCallback)
        {
            this.open(true);
        }

        public void clear()
        {
            foreach (PromptTextArea textArea in textAreas)
            {
                textArea.Dispose();
            }
            textAreas.Clear();
        }

        internal void questionSelected(PromptAnswerTextArea answerText)
        {
            Logging.Log.Debug("Answer selected");
        }
    }
}
