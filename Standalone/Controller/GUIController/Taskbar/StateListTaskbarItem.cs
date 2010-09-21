using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class StateListTaskbarItem : TaskbarItem
    {
        private StateListPopup stateListPopup;

        public StateListTaskbarItem(MedicalStateController stateController)
            :base("States", "Joint")
        {
            stateListPopup = new StateListPopup(stateController);
        }

        public override void Dispose()
        {
            stateListPopup.Dispose();
            base.Dispose();
        }

        public override void clicked(Widget source, EventArgs e)
        {
            stateListPopup.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }
    }
}
