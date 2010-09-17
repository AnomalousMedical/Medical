using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Logging;

namespace Medical.GUI
{
    class DummyTaskbarItem : TaskbarItem
    {
        public DummyTaskbarItem(String iconName, String name)
            :base(name, iconName)
        {
            
        }

        public override void clicked(Widget source, EventArgs e)
        {
            Log.Debug("Taskbar item {0} clicked", Name);
        }
    }
}
