using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ActionViewRow
    {
        private List<ActionViewButton> buttons = new List<ActionViewButton>();
        private int yPosition;

        public ActionViewRow(int yPosition)
        {
            this.yPosition = yPosition;
        }

        public void addButton(Button button, TimelineAction action)
        {
            ActionViewButton viewButton = new ActionViewButton(button, action);
            buttons.Add(viewButton);
            button.setPosition(button.Left, yPosition);
        }
    }
}
