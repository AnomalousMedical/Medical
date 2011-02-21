using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Platform;

namespace Medical.GUI
{
    class MyGUIQuestionProvider : Dialog, IQuestionProvider, OSWindowListener
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
            questionEdit.Font = "font_DejaVuSans.Large_Question";

            PluginManager.Instance.RendererPlugin.PrimaryWindow.Handle.addListener(this);
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
            int answerWidth = answerScroll.Width - 20;
            foreach (PromptAnswer answer in question.Answers)
            {
                answerTextArea = new PromptAnswerTextArea(this, answer, answerScroll, 0, answerWidth, verticalPosition, light);
                textAreas.Add(answerTextArea);
                verticalPosition = answerTextArea.Bottom;
                light = !light;
            }

            Size2 canvasSize = answerScroll.CanvasSize;
            answerScroll.CanvasSize = new Size2(answerWidth, verticalPosition);

            int windowHeight = verticalPosition + 42 + answerScroll.Top;
            if(windowHeight > Gui.Instance.getViewHeight() - 100)
            {
                windowHeight = Gui.Instance.getViewHeight() - 100;
            }
            window.setSize(window.Width, windowHeight);
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

        #region OSWindowListener Members

        public void closed(OSWindow window)
        {
            
        }

        public void closing(OSWindow window)
        {
            
        }

        public void focusChanged(OSWindow window)
        {
            
        }

        public void moved(OSWindow window)
        {
            
        }

        public void resized(OSWindow window)
        {
            if (Visible)
            {
                center();
            }
        }

        #endregion
    }
}
