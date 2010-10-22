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
        public event EventHandler BottomChanged;

        private List<ActionViewButton> buttons = new List<ActionViewButton>();
        private int yPosition;
        private int pixelsPerSecond;
        private Color color;
        private int ROW_HEIGHT = 19;
        private int STACKED_BUTTON_SPACE = 3;
        private int bottom;
        private bool processButtonChanges = true;

        public ActionViewRow(String name, int yPosition, int pixelsPerSecond, Color color)
        {
            this.Name = name;
            this.yPosition = yPosition;
            this.color = color;
            this.pixelsPerSecond = pixelsPerSecond;
            bottom = yPosition + ROW_HEIGHT;
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
            computeButtonPosition(viewButton);
            findLowestButton();
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
                closeGaps(removeMe, removeMe.Left, removeMe.Right);
                removeMe.Dispose();
            }
            findLowestButton();
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

        public void moveEntireRow(int newYPosition)
        {
            processButtonChanges = false;
            bottom = newYPosition + ROW_HEIGHT;
            int delta = newYPosition - yPosition;
            foreach (ActionViewButton button in buttons)
            {
                button._moveTop(button.Top + delta);
                if (button.Bottom > bottom)
                {
                    bottom = button.Bottom;
                }
            }
            yPosition = newYPosition;
            processButtonChanges = true;
        }

        public int Bottom
        {
            get
            {
                return bottom;
            }
        }

        public int Top
        {
            get
            {
                return yPosition;
            }
        }

        public String Name { get; private set; }

        public Color Color
        {
            get
            {
                return color;
            }
        }

        void viewButton_CoordChanged(object sender, EventArgs e)
        {
            if (processButtonChanges)
            {
                ActionViewButtonEventArgs avbe = e as ActionViewButtonEventArgs;
                ActionViewButton movedButton = sender as ActionViewButton;
                computeButtonPosition(movedButton);
                closeGaps(movedButton, avbe.OldLeft, avbe.OldRight);
                findLowestButton();
            }
        }

        private void computeButtonPosition(ActionViewButton movedButton)
        {
            //Find the buttons that currently intersect the moved button
            List<ActionViewButton> currentStackedButtons = new List<ActionViewButton>();
            findIntersectingButtons(currentStackedButtons, movedButton.Left, movedButton.Right);
            currentStackedButtons.Remove(movedButton);

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
        }

        private void closeGaps(ActionViewButton movedButton, int oldLeft, int oldRight)
        {
            //Move any buttons that can be moved up.
            //Find the buttons that intersect the old position
            List<ActionViewButton> formerStackedButtons = new List<ActionViewButton>();
            if (oldLeft == movedButton.Left && oldRight == movedButton.Right) //Did not move, could be removed.
            {
                findIntersectingButtons(formerStackedButtons, oldLeft, oldRight);
            }
            else if (oldLeft > movedButton.Left)//Moved left
            {
                findIntersectingButtons(formerStackedButtons, movedButton.Right, oldRight);
            }
            else //Moved right
            {
                findIntersectingButtons(formerStackedButtons, oldLeft, movedButton.Left);
            }
            formerStackedButtons.Remove(movedButton);

            //Sort the old stack by top.
            formerStackedButtons.Sort(topSortButtons);

            //Go through the list and find the index of the first gap.
            int gapIndex = findGapIndex(formerStackedButtons);
            if (gapIndex != -1)
            {
                for (int i = gapIndex; i < formerStackedButtons.Count; ++i)
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
                if (sortedStackedButtons[i].Bottom + STACKED_BUTTON_SPACE != sortedStackedButtons[i + 1].Bottom || //Index is not next expected y position
                    sortedStackedButtons[i].Bottom != sortedStackedButtons[i + 1].Bottom) //Y positions are not the same
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
                    (right >= compare.Left && right <= compare.Right) ||
                    (compare.Left >= left && compare.Left <= right) ||
                    (compare.Right >= left && compare.Right <= right))
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
            int lastButtonYPos = -1;
            foreach (ActionViewButton button in buttonStack)
            {
                if (lastButtonYPos != button.Top) //Make sure the next button is actually lower, its possible that it is not
                {
                    lastButtonYPos = button.Top;
                    if (button.Top == insertYPos)
                    {
                        insertYPos = button.Bottom + STACKED_BUTTON_SPACE;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            insert._moveTop(insertYPos);
        }

        int topSortButtons(ActionViewButton a1, ActionViewButton a2)
        {
            return a1.Top - a2.Top;
        }

        void findLowestButton()
        {
            int lowest = 0;
            foreach (ActionViewButton button in buttons)
            {
                if (button.Bottom > lowest)
                {
                    lowest = button.Bottom;
                }
            }
            if (lowest != bottom)
            {
                bottom = lowest + STACKED_BUTTON_SPACE;
                if (BottomChanged != null)
                {
                    BottomChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
