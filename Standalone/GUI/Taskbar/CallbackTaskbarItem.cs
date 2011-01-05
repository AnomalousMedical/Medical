using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class CallbackTaskbarItem : TaskbarItem
    {
        public event EventHandler OnClicked;
        public event EventHandler OnRightClicked;

        public CallbackTaskbarItem(String name, String iconName)
            :base(name, iconName)
        {

        }

        public override void clicked(Widget source, EventArgs e)
        {
            if (OnClicked != null)
            {
                OnClicked.Invoke(source, e);
            }
        }

        public override void rightClicked(Widget source, EventArgs e)
        {
            if (OnRightClicked != null)
            {
                OnRightClicked.Invoke(source, e);
            }
        }
    }
}
