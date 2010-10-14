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
        private int STACKED_BUTTON_SPACE = 3;

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
            viewButton.CoordChanged += viewButton_CoordChanged;
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
                removeMe.CoordChanged -= viewButton_CoordChanged;
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

        void viewButton_CoordChanged(object sender, EventArgs e)
        {
            ActionViewButtonEventArgs avbe = e as ActionViewButtonEventArgs;
            ActionViewButton movedButton = sender as ActionViewButton;

            //Find the buttons that currently intersect the moved button
            List<ActionViewButton> currentStackedButtons = new List<ActionViewButton>();
            findIntersectingButtons(currentStackedButtons, movedButton.Left, movedButton.Right);
            currentStackedButtons.Remove(movedButton);

            //Find the buttons that intersect the old position
            List<ActionViewButton> formerStackedButtons = new List<ActionViewButton>();
            findIntersectingButtons(formerStackedButtons, avbe.OldLeft, avbe.OldRight);
            formerStackedButtons.Remove(movedButton);

            //If there are no buttons currently intersecting the new position, put the button at the top.
            if (currentStackedButtons.Count == 0)
            {
                movedButton._moveTop(yPosition);
            }
            //Put the button at the first blank space that can be found
            else
            {
                insertButtonIntoStack(currentStackedButtons, movedButton);
            }

          //Move up any former buttons that can be moved up.
            //Remove all buttons that are in the current stack.
            //foreach (ActionViewButton button in currentStackedButtons)
            //{
            //    formerStackedButtons.Remove(button);
            //}

            //Sort the old stack by top.
            formerStackedButtons.Sort(topSortButtons);

            //Go through the list and find the index of the first gap.
            int gapIndex = findGapIndex(formerStackedButtons);
            if (gapIndex != -1)
            {
                for (int i = 0; i < formerStackedButtons.Count; ++i)
                {
                    moveUp(formerStackedButtons[i]);
                }
            }
        }

        private void moveUp(ActionViewButton button)
        {
            //Find the buttons that currently intersect the button
            List<ActionViewButton> stackedButtons = new List<ActionViewButton>();
            findIntersectingButtons(stackedButtons, button.Left, button.Right);
            stackedButtons.Remove(button);
            insertButtonIntoStack(stackedButtons, button);
        }

        private int findGapIndex(List<ActionViewButton> sortedStackedButtons)
        {
            for(int i = 0; i < sortedStackedButtons.Count - 1; ++i)
            {
                if (sortedStackedButtons[i].Bottom + STACKED_BUTTON_SPACE != sortedStackedButtons[i + 1].Bottom)
                {
                    return i;
                }
            }
            return -1;
        }

        void findIntersectingButtons(List<ActionViewButton> results, int left, int right)
        {
            foreach (ActionViewButton compare in buttons)
            {
                if ((left >= compare.Left && left <= compare.Right) ||
                    (right >= compare.Left && right <= compare.Right))
                {
                    results.Add(compare);
                }
            }
        }

        void insertButtonIntoStack(List<ActionViewButton> buttonStack, ActionViewButton insert)
        {
            //Sort the stack in order from top to bottom.
            buttonStack.Sort(topSortButtons);
            //Search the stack looking for a space in the buttons.
            int insertYPos = yPosition;
            foreach (ActionViewButton button in buttonStack)
            {
                if (button.Top == insertYPos)
                {
                    insertYPos = button.Bottom + STACKED_BUTTON_SPACE;
                }
                else
                {
                    break;
                }
            }
            insert._moveTop(insertYPos);
        }

        int topSortButtons(ActionViewButton a1, ActionViewButton a2)
        {
            return a1.Top - a2.Top;
        }
    }
}
