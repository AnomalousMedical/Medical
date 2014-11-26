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
            popupContainer.Hidden += new EventHandler(popupContainer_Hidden);
        }

        public ShowPopupTask(PopupContainer popupContainer, String uniqueName, String name, String iconName, String category, int weight)
            :this(popupContainer, uniqueName, name, iconName, category)
        {
            this.Weight = weight;
        }

        public override void clicked(TaskPositioner positioner)
        {
            if (!popupContainer.Visible)
            {
                IntVector2 loc;
                if (positioner != null)
                {
                    loc = positioner.findGoodWindowPosition(popupContainer.Width, popupContainer.Height);
                }
                else
                {
                    loc = new IntVector2(0, 0);
                }
                popupContainer.show(loc.x, loc.y);
            }
        }

        public override bool Active
        {
            get { return popupContainer.Visible; }
        }

        void popupContainer_Hidden(object sender, EventArgs e)
        {
            fireItemClosed();
        }
    }
}
