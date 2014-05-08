using Engine;
using Medical;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture.GUI
{
    class SlideTaskbarItem : IDisposable, TaskPositioner
    {
        Button taskButton;
        Task task;

        public SlideTaskbarItem(Task task, Widget parent, IntRect position)
        {
            this.task = task;

            taskButton = (Button)parent.createWidgetT("Button", "TaskbarButton", position.Left, position.Top, position.Width, position.Height, Align.Left | Align.Top, task.UniqueName);
            taskButton.NeedToolTip = true;
            taskButton.ImageBox.setItemResource(task.IconName);
            taskButton.MouseButtonClick += new MyGUIEvent(taskButton_MouseButtonClick);
            taskButton.EventToolTip += new MyGUIEvent(taskButton_EventToolTip);
            if (task.Dragable)
            {
                taskButton.MouseDrag += taskButton_MouseDrag;
                taskButton.MouseButtonPressed += taskButton_MouseButtonPressed;
                taskButton.MouseButtonReleased += taskButton_MouseButtonReleased;
            }
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(taskButton);
        }

        public void setPosition(int left, int top)
        {
            taskButton.setPosition(left, top);
        }

        void taskButton_EventToolTip(Widget source, EventArgs e)
        {
            TooltipManager.Instance.processTooltip(source, task.Name, (ToolTipEventArgs)e);
        }

        void taskButton_MouseButtonClick(Widget source, EventArgs e)
        {
            task.clicked(this);
        }

        void taskButton_MouseDrag(Widget source, EventArgs e)
        {
            task.dragged(((MouseEventArgs)e).Position);
        }

        void taskButton_MouseButtonPressed(Widget source, EventArgs e)
        {
            task.dragStarted(((MouseEventArgs)e).Position);
        }

        void taskButton_MouseButtonReleased(Widget source, EventArgs e)
        {
            task.dragEnded(((MouseEventArgs)e).Position);
        }

        public int Width
        {
            get
            {
                return taskButton.Width;
            }
        }

        public IntVector2 findGoodWindowPosition(int width, int height)
        {
            return new IntVector2(taskButton.AbsoluteLeft, taskButton.AbsoluteTop + taskButton.Height);
        }
    }
}
