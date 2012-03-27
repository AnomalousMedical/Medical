using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Platform;

namespace Medical.GUI
{
    class MyGUIQuestionProvider : Dialog, IQuestionProvider
    {
        private ScrollView answerScroll;
        private EditBox questionEdit;
        private List<PromptTextArea> textAreas = new List<PromptTextArea>();
        private PromptAnswerSelected answerSelectedCallback;
        private GUIManager guiManager;
        private int verticalPosition = 0;

        public MyGUIQuestionProvider(GUIManager guiManager)
            :base("Medical.GUI.Timeline.QuestionProvider.MyGUIQuestionProvider.layout")
        {
            this.guiManager = guiManager;
            guiManager.ScreenSizeChanged += new ScreenSizeChanged(guiManager_ScreenSizeChanged);

            answerScroll = window.findWidget("Answers") as ScrollView;
            questionEdit = window.findWidget("Question") as EditBox;
            questionEdit.Font = "font_DejaVuSans.Large_Question";
        }

        void guiManager_ScreenSizeChanged(int width, int height)
        {
            if (Visible)
            {
                int windowHeight = verticalPosition + 47 + answerScroll.Top;
                if (windowHeight > height - 100)
                {
                    windowHeight = height - 100;
                }
                window.setSize(window.Width, windowHeight);
                window.setPosition((width - window.Width) / 2, (height - window.Height) / 2);
            }
        }

        public override void Dispose()
        {
            clear();
            base.Dispose();
        }

        public void addQuestion(PromptQuestion question)
        {
            bool light = true;
            verticalPosition = 0;
            questionEdit.Caption = question.Text;

            PromptTextArea answerTextArea = null;
            int answerWidth = answerScroll.Width - 20;
            foreach (PromptAnswer answer in question.Answers)
            {
                answerTextArea = new PromptAnswerTextArea(this, answer, answerScroll, 0, answerWidth, verticalPosition, light);
                textAreas.Add(answerTextArea);
                verticalPosition = answerTextArea.Bottom;
                light = !light;
            }

            answerScroll.CanvasSize = new Size2(answerWidth, verticalPosition);

            int windowHeight = verticalPosition + 47 + answerScroll.Top;
            if(windowHeight > RenderManager.Instance.ViewHeight - 100)
            {
                windowHeight = RenderManager.Instance.ViewHeight - 100;
            }
            window.setSize(window.Width, windowHeight);
        }

        public void showPrompt(PromptAnswerSelected answerSelectedCallback)
        {
            int halfWidth = RenderManager.Instance.ViewWidth / 2;
            int halfHeight = RenderManager.Instance.ViewHeight / 2;
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
