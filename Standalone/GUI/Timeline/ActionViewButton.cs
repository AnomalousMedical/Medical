using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ActionViewButton
    {
        private Button button;
        private TimelineAction action;

        public ActionViewButton(Button button, TimelineAction action)
        {
            this.action = action;
            this.button = button;
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(button);
        }
    }
}
