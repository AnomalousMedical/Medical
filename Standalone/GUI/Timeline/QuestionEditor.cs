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
        private TimelineController timelineController;

        private List<QuestionEditorAnswerRow> rows = new List<QuestionEditorAnswerRow>();

        public QuestionEditor(TimelineFileBrowserDialog fileBrowser, TimelineController timelineController)
            :base("Medical.GUI.Timeline.QuestionEditor.layout")
        {
            this.fileBrowser = fileBrowser;
            this.timelineController = timelineController;

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
            PromptQuestion question = null;
            if (questionText.Caption == null || questionText.Caption == "")
            {
                MessageBox.show("You must enter some text for the question.", "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
            else
            {
                if (rows.Count > 0)
                {
                    question = new PromptQuestion(questionText.Caption);
                    int i = 0;
                    foreach (QuestionEditorAnswerRow row in rows)
                    {
                        if (row.AnswerText != null && row.AnswerText != "")
                        {
                            PromptAnswer answer = new PromptAnswer(row.AnswerText);
                            String timeline = row.Timeline;
                            if (timeline != null && timeline != "")
                            {
                                if (timelineController.listResourceFiles(timeline).Length > 0)
                                {
                                    answer.Action = new PromptLoadTimelineAction(timeline);
                                }
                                else
                                {
                                    MessageBox.show(String.Format("Answer number {0} points to a timeline that does not exist. Please correct it or leave it blank to specify that you do not wish to load another timeline.", i), "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                                    question = null;
                                    break;
                                }
                            }
                            question.addAnswer(answer);
                        }
                        else
                        {
                            MessageBox.show(String.Format("Answer number {0} does not have any text. Please add some.", i), "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                            question = null;
                            break;
                        }
                        ++i;
                    }
                }
                else
                {
                    MessageBox.show("Cannot create a question with no answers. Please add some.", "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                    question = null;
                }
            }
            return question;
        }

        void addAnswerButton_MouseButtonClick(Widget source, EventArgs e)
        {
            addRow("", "");
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            PromptQuestion newQuestion = createQuestion();
            if (newQuestion != null)
            {
                currentSourceQuestion = newQuestion;
                this.close();
            }
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
