using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class QuestionEditorAnswerRow : Component
    {
        public event EventHandler RemoveRow;

        private const int ROW_SIZE_ADJUST = 8;

        private Edit answerText;
        private Edit timelineText;

        public QuestionEditorAnswerRow(Widget parent, int yPos)
            :base("Medical.GUI.Timeline.QuestionEditorAnswerRow.layout")
        {
            widget.setPosition(0, yPos);
            widget.setSize(parent.Width - ROW_SIZE_ADJUST, widget.Height);
            widget.attachToWidget(parent);

            Button removeButton = widget.findWidget("RemoveButton") as Button;
            removeButton.MouseButtonClick += new MyGUIEvent(removeButton_MouseButtonClick);

            Button browseButton = widget.findWidget("BrowseButton") as Button;
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);

            answerText = widget.findWidget("AnswerText") as Edit;
            timelineText = widget.findWidget("TimelineText") as Edit;
        }

        public String AnswerText
        {
            get
            {
                return answerText.Caption;
            }
            set
            {
                answerText.Caption = value;
            }
        }

        public String Timeline
        {
            get
            {
                return timelineText.Caption;
            }
            set
            {
                timelineText.Caption = value;
            }
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {

        }

        public int Width
        {
            get
            {
                return widget.Width;
            }
            set
            {
                widget.setSize(value - ROW_SIZE_ADJUST, widget.Height);
            }
        }

        public int Top
        {
            get
            {
                return widget.Top;
            }
            set
            {
                widget.setPosition(widget.Left, value);
            }
        }

        public int Bottom
        {
            get
            {
                return widget.Top + widget.Height;
            }
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (RemoveRow != null)
            {
                RemoveRow.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
