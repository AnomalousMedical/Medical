using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class ShowPopupTaskbarItem : TaskbarItem
    {
        public event EventHandler RightClicked;

        private PopupContainer popupContainer;

        public ShowPopupTaskbarItem(PopupContainer popupContainer, String name, String iconName)
            :base(name, iconName)
        {
            this.popupContainer = popupContainer;
        }

        public override void clicked(Widget source, EventArgs e)
        {
            IntVector2 position = this.findGoodPosition(popupContainer.Width, popupContainer.Height);
            popupContainer.show(position.x, position.y);
        }

        public override void rightClicked(Widget source, EventArgs e)
        {
            if (RightClicked != null)
            {
                RightClicked.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
