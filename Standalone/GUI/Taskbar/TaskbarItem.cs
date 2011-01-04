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

        internal Button TaskbarButton
        {
            get
            {
                return taskbarButton;
            }
            set
            {
                this.taskbarButton = value;
                taskbarButton.StaticImage.setItemResource(IconName);
                taskbarButton.MouseButtonClick += clicked;
                taskbarButton.MouseButtonReleased += new MyGUIEvent(taskbarButton_MouseButtonReleased);
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
