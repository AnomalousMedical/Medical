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

        internal override void addToPinnedTasksList(PinnedTaskSerializer pinnedTaskSerializer)
        {
            pinnedTaskSerializer.addPinnedTask(Task);
        }

        protected override void customizeMenu()
        {
            addMenuItem("Remove", fireRemoved);
        }

        private void fireRemoved()
        {
            if (RemoveFromTaskbar != null)
            {
                RemoveFromTaskbar.Invoke(this);
            }
        }
    }
}
