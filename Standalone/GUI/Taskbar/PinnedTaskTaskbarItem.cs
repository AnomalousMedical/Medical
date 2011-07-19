using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class PinnedTaskTaskbarItem : TaskTaskbarItem
    {
        public event EventDelegate<TaskTaskbarItem> RemoveFromTaskbar;

        public PinnedTaskTaskbarItem(Task task)
            : base(task)
        {

        }

        public override void rightClicked(Widget source, EventArgs e)
        {
            if (RemoveFromTaskbar != null)
            {
                RemoveFromTaskbar.Invoke(this);
            }
        }
    }
}
