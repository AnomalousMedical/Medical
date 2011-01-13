using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class MyGUIQuestionProvider : Component, IQuestionProvider
    {
        private ScrollView questionScroll;
        private List<PromptTextArea> textAreas = new List<PromptTextArea>();
        private PromptAnswerSelected answerSelectedCallback;
        private MyGUILayoutContainer layoutContainer;
        private GUIManager guiManager;

        public MyGUIQuestionProvider(GUIManager guiManager)
            :base("Medical.GUI.Timeline.QuestionProvider.MyGUIQuestionProvider.layout")
        {
            this.guiManager = guiManager;

            widget.Visible = false;
            questionScroll = widget.findWidget("QuestionScroll") as ScrollView;

            layoutContainer = new MyGUILayoutContainer(widget);
        }

        public override void Dispose()
        {
            clear();
            base.Dispose();
        }

        public void addQuestion(PromptQuestion question)
        {
            int verticalPosition = textAreas.Count > 0 ? textAreas[textAreas.Count - 1].Bottom : 0;
            PromptTextArea questionTextArea = new PromptQuestionTextArea(question.Text, questionScroll, 0, widget.Width, verticalPosition);
            textAreas.Add(questionTextArea);
            verticalPosition = questionTextArea.Bottom;

            PromptTextArea answerTextArea = null;
            foreach (PromptAnswer answer in question.Answers)
            {
                answerTextArea = new PromptAnswerTextArea(this, answer, questionScroll, 25, widget.Width, verticalPosition);
                textAreas.Add(answerTextArea);
                verticalPosition = answerTextArea.Bottom;
            }

            Size2 canvasSize = questionScroll.CanvasSize;
            questionScroll.CanvasSize = new Size2(widget.Width, verticalPosition);
        }

        public void showPrompt(PromptAnswerSelected answerSelectedCallback)
        {
            this.answerSelectedCallback = answerSelectedCallback;
            widget.Visible = true;
            InputManager.Instance.addWidgetModal(widget);
            layoutContainer.changeDesiredSize(questionScroll.CanvasSize);
            guiManager.changeLeftPanel(layoutContainer);
        }

        public void clear()
        {
            foreach (PromptTextArea textArea in textAreas)
            {
                textArea.Dispose();
            }
            textAreas.Clear();
        }

        public LayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
        }

        internal void answerSelected(PromptAnswer answer)
        {
            answerSelectedCallback(answer);
            InputManager.Instance.removeWidgetModal(widget);
            widget.Visible = false;
            guiManager.changeLeftPanel(null);
            answerSelectedCallback = null;
        }
    }
}
