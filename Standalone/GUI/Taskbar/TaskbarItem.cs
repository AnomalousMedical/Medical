using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

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
        }

        internal void setCoord(int x, int y, int width, int height)
        {
            taskbarButton.setCoord(x, y, width, height);
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
