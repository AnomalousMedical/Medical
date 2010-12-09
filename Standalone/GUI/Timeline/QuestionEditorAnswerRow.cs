using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class QuestionEditorAnswerRow : Component
    {
        private const int ROW_SIZE_ADJUST = 8;

        public QuestionEditorAnswerRow(Widget parent, int yPos)
            :base("Medical.GUI.Timeline.QuestionEditorAnswerRow.layout")
        {
            widget.setPosition(0, yPos);
            widget.setSize(parent.Width - ROW_SIZE_ADJUST, widget.Height);
            widget.attachToWidget(parent);


            Button removeButton = widget.findWidget("RemoveButton") as Button;
            removeButton.MouseButtonClick += new MyGUIEvent(removeButton_MouseButtonClick);


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
            
        }
    }
}
