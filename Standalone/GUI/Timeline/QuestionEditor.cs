using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class QuestionEditor : Dialog
    {
        private Edit questionText;
        private ScrollView answerScroll;
        private int lastWidth;
        private TimelineFileBrowserDialog fileBrowser;
        private PromptQuestion currentSourceQuestion = null;

        private List<QuestionEditorAnswerRow> rows = new List<QuestionEditorAnswerRow>();

        public QuestionEditor(TimelineFileBrowserDialog fileBrowser)
            :base("Medical.GUI.Timeline.QuestionEditor.layout")
        {
            this.fileBrowser = fileBrowser;

            Button applyButton = window.findWidget("ApplyButton") as Button;
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button addAnswerButton = window.findWidget("AddAnswerButton") as Button;
            addAnswerButton.MouseButtonClick += new MyGUIEvent(addAnswerButton_MouseButtonClick);

            answerScroll = window.findWidget("AnswerScroll") as ScrollView;
            questionText = window.findWidget("QuestionText") as Edit;

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
            lastWidth = window.Width;
        }

        public void clear()
        {
            currentSourceQuestion = null;
            questionText.Caption = "";
            foreach (QuestionEditorAnswerRow row in rows)
            {
                row.Dispose();
            }
            rows.Clear();
        }

        public PromptQuestion Question
        {
            get
            {
                return currentSourceQuestion;
            }
            set
            {
                setData(value);
            }
        }

        private void setData(PromptQuestion question)
        {
            if (question != null)
            {
                questionText.Caption = question.Text;

                foreach (PromptAnswer answer in question.Answers)
                {
                    String timeline = "";
                    PromptLoadTimelineAction loadTimeline = answer.Action as PromptLoadTimelineAction;
                    if (loadTimeline != null)
                    {
                        timeline = loadTimeline.TargetTimeline;
                    }
                    addRow(answer.Text, timeline);
                }
            }
            currentSourceQuestion = question;
        }

        private PromptQuestion createQuestion()
        {
            PromptQuestion question = new PromptQuestion(questionText.Caption);
            foreach (QuestionEditorAnswerRow row in rows)
            {
                PromptAnswer answer = new PromptAnswer(row.AnswerText);
                String timeline = row.Timeline;
                if (timeline != null && timeline != "")
                {
                    answer.Action = new PromptLoadTimelineAction(timeline);
                }
                question.addAnswer(answer);
            }
            return question;
        }

        void addAnswerButton_MouseButtonClick(Widget source, EventArgs e)
        {
            addRow("", "");
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            currentSourceQuestion = createQuestion();
            this.close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            PromptQuestion tempQuestionStorage = currentSourceQuestion;
            this.clear();
            this.setData(tempQuestionStorage);
            this.close();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            if (window.Width != lastWidth)
            {
                lastWidth = window.Width;
                int scrollWidth = answerScroll.Width;
                foreach (QuestionEditorAnswerRow row in rows)
                {
                    row.Width = scrollWidth;
                }
                adjustCanvasForRows();
            }
        }

        private void addRow(String answerText, String timeline)
        {
            int yPos = rows.Count > 0 ? rows[rows.Count - 1].Bottom : 0;
            bool wasVisible = this.Visible;

            this.Visible = true;
            QuestionEditorAnswerRow row = new QuestionEditorAnswerRow(answerScroll, yPos, fileBrowser);
            row.RemoveRow += new EventHandler(row_RemoveRow);
            row.AnswerText = answerText;
            row.Timeline = timeline;
            rows.Add(row);
            this.Visible = wasVisible;

            answerScroll.CanvasSize = new Size2(row.Width, row.Bottom);
        }

        void row_RemoveRow(object sender, EventArgs e)
        {
            QuestionEditorAnswerRow row = sender as QuestionEditorAnswerRow;
            rows.Remove(row);
            row.Dispose();
            closeRowGaps();
        }

        private void closeRowGaps()
        {
            int yPos = 0;
            foreach (QuestionEditorAnswerRow moveRow in rows)
            {
                moveRow.Top = yPos;
                yPos = moveRow.Bottom;
            }
            adjustCanvasForRows();
        }

        private void adjustCanvasForRows()
        {
            QuestionEditorAnswerRow lastRow = rows.Count > 0 ? rows[rows.Count - 1] : null;
            if (lastRow != null)
            {
                answerScroll.CanvasSize = new Size2(lastRow.Width, lastRow.Bottom);
            }
            else
            {
                answerScroll.CanvasSize = new Size2(0, 0);
            }
        }
    }
}
