using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ShowToothContactsTaskbarItem : TaskbarItem
    {
        public ShowToothContactsTaskbarItem()
            :base("Show Occlusal Contacts", "ShowTeethContactsIcon")
        {

        }

        #region TaskbarItem Members

        public override void clicked(Widget source, EventArgs e)
        {
            Button button = source as Button;
            button.StateCheck = !button.StateCheck;
            TeethController.HighlightContacts = button.StateCheck;
        }

        #endregion
    }
}
