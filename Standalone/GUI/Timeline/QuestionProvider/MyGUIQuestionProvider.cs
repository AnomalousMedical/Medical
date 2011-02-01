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
        private ScrollView answerScroll;
        private Edit questionEdit;
        private List<PromptTextArea> textAreas = new List<PromptTextArea>();
        private PromptAnswerSelected answerSelectedCallback;
        private GUIManager guiManager;

        public MyGUIQuestionProvider(GUIManager guiManager)
            :base("Medical.GUI.Timeline.QuestionProvider.MyGUIQuestionProvider.layout")
        {
            this.guiManager = guiManager;

            answerScroll = window.findWidget("Answers") as ScrollView;
            questionEdit = window.findWidget("Question") as Edit;
        }

        public override void Dispose()
        {
            clear();
            base.Dispose();
        }

        public void addQuestion(PromptQuestion question)
        {
            bool light = true;
            int verticalPosition = 0;
            questionEdit.Caption = question.Text;

            PromptTextArea answerTextArea = null;
            foreach (PromptAnswer answer in question.Answers)
            {
                answerTextArea = new PromptAnswerTextArea(this, answer, answerScroll, 0, answerScroll.Width, verticalPosition, light);
                textAreas.Add(answerTextArea);
                verticalPosition = answerTextArea.Bottom;
                light = !light;
            }

            Size2 canvasSize = answerScroll.CanvasSize;
            answerScroll.CanvasSize = new Size2(answerScroll.Width, verticalPosition);
        }

        public void showPrompt(PromptAnswerSelected answerSelectedCallback)
        {
            int halfWidth = Gui.Instance.getViewWidth() / 2;
            int halfHeight = Gui.Instance.getViewHeight() / 2;
            this.Position = new Vector2(halfWidth - window.Width / 2, halfHeight - window.Height / 2);
            this.answerSelectedCallback = answerSelectedCallback;
            LayerManager.Instance.upLayerItem(window);
            InputManager.Instance.addWidgetModal(window);
            window.setVisibleSmooth(true);
        }

        public void clear()
        {
            questionEdit.Caption = "";
            foreach (PromptTextArea textArea in textAreas)
            {
                textArea.Dispose();
            }
            textAreas.Clear();
        }

        internal void answerSelected(PromptAnswer answer)
        {
            answerSelectedCallback(answer);
            InputManager.Instance.removeWidgetModal(window);
            window.setVisibleSmooth(false);
            answerSelectedCallback = null;
        }
    }
}
