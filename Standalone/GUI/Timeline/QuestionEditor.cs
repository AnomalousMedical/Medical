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

        private List<QuestionEditorAnswerRow> rows = new List<QuestionEditorAnswerRow>();

        public QuestionEditor()
            :base("Medical.GUI.Timeline.QuestionEditor.layout")
        {
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

        void addAnswerButton_MouseButtonClick(Widget source, EventArgs e)
        {
            int yPos = rows.Count > 0 ? rows[rows.Count - 1].Bottom : 0;

            QuestionEditorAnswerRow row = new QuestionEditorAnswerRow(answerScroll, yPos);
            rows.Add(row);

            answerScroll.CanvasSize = new Size2(row.Width, row.Bottom);
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
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
}
