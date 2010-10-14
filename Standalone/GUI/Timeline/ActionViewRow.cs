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
        private int pixelsPerSecond;
        private Color color;
        private int ROW_HEIGHT = 19;

        public ActionViewRow(int yPosition, int pixelsPerSecond, Color color)
        {
            this.yPosition = yPosition;
            this.color = color;
            this.pixelsPerSecond = pixelsPerSecond;
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
            ActionViewButton viewButton = new ActionViewButton(pixelsPerSecond, button, action);
            buttons.Add(viewButton);
            button.setPosition(button.Left, yPosition);
            button.setColour(color);
            return viewButton;
        }

        public ActionViewButton removeButton(TimelineAction action)
        {
            ActionViewButton removeMe = null;
            foreach (ActionViewButton button in buttons)
            {
                if (button.Action == action)
                {
                    removeMe = button;
                    break;
                }
            }
            if (removeMe != null)
            {
                buttons.Remove(removeMe);
                removeMe.Dispose();
            }
            return removeMe;
        }

        public ActionViewButton findButtonForAction(TimelineAction action)
        {
            foreach (ActionViewButton button in buttons)
            {
                if (button.Action == action)
                {
                    return button;
                }
            }
            return null;
        }

        public void removeAllActions()
        {
            foreach (ActionViewButton button in buttons)
            {
                button.Dispose();
            }
            buttons.Clear();
        }

        public void changePixelsPerSecond(int pixelsPerSecond)
        {
            this.pixelsPerSecond = pixelsPerSecond;
            foreach (ActionViewButton button in buttons)
            {
                button.changePixelsPerSecond(pixelsPerSecond);
            }
        }

        public void findRightmostButton(ref ActionViewButton rightmostButton)
        {
            if (rightmostButton == null && buttons.Count > 0)
            {
                rightmostButton = buttons[0];
            }
            foreach (ActionViewButton button in buttons)
            {
                if (button.Right > rightmostButton.Right)
                {
                    rightmostButton = button;
                }
            }
        }

        public int Bottom
        {
            get
            {
                return yPosition + ROW_HEIGHT;
            }
        }
    }
}
