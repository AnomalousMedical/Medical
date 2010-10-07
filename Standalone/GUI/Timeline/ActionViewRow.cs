using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ActionViewRow : IDisposable
    {
        private List<ActionViewButton> buttons = new List<ActionViewButton>();
        private int yPosition;
        private Color color;

        public ActionViewRow(int yPosition, Color color)
        {
            this.yPosition = yPosition;
            this.color = color;
        }

        public void Dispose()
        {
            foreach (ActionViewButton button in buttons)
            {
                button.Dispose();
            }
        }

        public ActionViewButton addButton(Button button, TimelineAction action)
        {
            ActionViewButton viewButton = new ActionViewButton(button, action);
            buttons.Add(viewButton);
            button.setPosition(button.Left, yPosition);
            button.setColour(color);
            return viewButton;
        }
    }
}
