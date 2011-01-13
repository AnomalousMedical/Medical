using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public abstract class TaskbarItem : IDisposable
    {
        protected Button taskbarButton;
        protected Taskbar taskbar;

        public TaskbarItem(String name, String iconName)
        {
            this.Name = name;
            this.IconName = iconName;
        }

        public virtual void Dispose()
        {

        }

        public abstract void clicked(Widget source, EventArgs e);

        public virtual void rightClicked(Widget source, EventArgs e)
        {
            
        }

        public String IconName { get; private set; }

        public String Name { get; private set; }

        internal void _configureForTaskbar(Taskbar taskbar, Button taskbarButton)
        {
            if (this.taskbarButton != null)
            {
                throw new Exception("This item has already been configured. Only add a TaskbarItem to one taskbar.");
            }
            this.taskbar = taskbar;
            this.taskbarButton = taskbarButton;
            taskbarButton.StaticImage.setItemResource(IconName);
            taskbarButton.MouseButtonClick += clicked;
            taskbarButton.MouseButtonReleased += new MyGUIEvent(taskbarButton_MouseButtonReleased);
            taskbarButton.NeedToolTip = true;
            taskbarButton.EventToolTip += new MyGUIEvent(taskbarButton_EventToolTip);
        }

        void taskbarButton_EventToolTip(Widget source, EventArgs e)
        {
            TooltipManager.Instance.processTooltip(source, Name, (ToolTipEventArgs)e);
        }

        internal void setCoord(int x, int y, int width, int height)
        {
            taskbarButton.setCoord(x, y, width, height);
        }

        protected IntVector2 findGoodPosition(int width, int height)
        {
            switch (taskbar.Alignment)
            {
                case TaskbarAlignment.Left:
                    return new IntVector2(taskbarButton.AbsoluteLeft, taskbarButton.AbsoluteTop + taskbarButton.Height);
                case TaskbarAlignment.Right:
                    return new IntVector2(taskbarButton.AbsoluteLeft - width + taskbarButton.Width, taskbarButton.AbsoluteTop + taskbarButton.Height);
                case TaskbarAlignment.Top:
                    return new IntVector2(taskbarButton.AbsoluteLeft, taskbarButton.AbsoluteTop + taskbarButton.Height);
                case TaskbarAlignment.Bottom:
                    return new IntVector2(taskbarButton.AbsoluteLeft, taskbarButton.AbsoluteTop - height);
                default:
                    return new IntVector2(taskbarButton.AbsoluteLeft, taskbarButton.AbsoluteTop - height);
            }
        }

        void taskbarButton_MouseButtonReleased(Widget source, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me.Button == Engine.Platform.MouseButtonCode.MB_BUTTON1)
            {
                rightClicked(source, e);
            }
        }
    }
}
