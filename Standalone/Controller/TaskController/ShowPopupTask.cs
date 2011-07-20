using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical
{
    public class ShowPopupTask : Task
    {
        private PopupContainer popupContainer;

        public ShowPopupTask(PopupContainer popupContainer, String uniqueName, String name, String iconName, String category)
            :base(uniqueName, name, iconName, category)
        {
            this.popupContainer = popupContainer;
        }

        public override void clicked()
        {
            popupContainer.show(0, 0);
        }

        public override bool Active
        {
            get { return popupContainer.Visible; }
        }
    }
}
