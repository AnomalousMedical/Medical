﻿using System;
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
    }
}
