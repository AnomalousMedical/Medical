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
        private Edit soundFileEdit;
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

            Button browseSoundButton = window.findWidget("SoundBrowser") as Button;
            browseSoundButton.MouseButtonClick += new MyGUIEvent(browseSoundButton_MouseButtonClick);

            soundFileEdit = window.findWidget("SoundFileEdit") as Edit;

            answerScroll = window.findWidget("AnswerScroll") as ScrollView;
            questionText = window.findWidget("QuestionText") as Edit;

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
            lastWidth = window.Width;
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            Ok = false;
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

        public String SoundFile
        {
            get
            {
                return soundFileEdit.Caption;
            }
            set
            {
                soundFileEdit.Caption = value;
            }
        }

        public bool Ok { get; private set; }

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
            Ok = true;
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

        private QuestionEditorAnswerRow addRow(String answerText, String timeline)
        {
            int yPos = rows.Count > 0 ? rows[rows.Count - 1].Bottom : 0;
            QuestionEditorAnswerRow row = new QuestionEditorAnswerRow(answerScroll, yPos, fileBrowser);
            row.RemoveRow += new EventHandler(row_RemoveRow);
            row.InsertAbove += new EventHandler(row_InsertAbove);
            row.InsertBelow += new EventHandler(row_InsertBelow);
            row.AnswerText = answerText;
            row.Timeline = timeline;
            rows.Add(row);

            answerScroll.CanvasSize = new Size2(row.Width, row.Bottom);

            return row;
        }

        void row_InsertBelow(object sender, EventArgs e)
        {
            QuestionEditorAnswerRow row = addRow("", "");
            rows.Remove(row);
            QuestionEditorAnswerRow relativeTo = sender as QuestionEditorAnswerRow;
            rows.Insert(rows.IndexOf(relativeTo) + 1, row);
            closeRowGaps();
        }

        void row_InsertAbove(object sender, EventArgs e)
        {
            QuestionEditorAnswerRow row = addRow("", "");
            rows.Remove(row);
            QuestionEditorAnswerRow relativeTo = sender as QuestionEditorAnswerRow;
            rows.Insert(rows.IndexOf(relativeTo), row);
            closeRowGaps();
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

        void browseSoundButton_MouseButtonClick(Widget source, EventArgs e)
        {
            fileBrowser.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            fileBrowser.ensureVisible();
            fileBrowser.promptForFile("*.ogg", fileChosen);
        }

        private void fileChosen(String filename)
        {
            soundFileEdit.Caption = filename;
        }
    }
}
