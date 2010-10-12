using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ActionView : IDisposable
    {
        private ScrollView scrollView;
        private int pixelsPerSecond = 100;
        private Dictionary<String, ActionViewRow> rows = new Dictionary<string, ActionViewRow>();
        private ActionViewButton currentButton;

        public event EventHandler ActiveActionChanged;

        public ActionView(ScrollView scrollView)
        {
            this.scrollView = scrollView;
            int y = 3;
            foreach (TimelineActionProperties actionProp in TimelineActionFactory.ActionProperties)
            {
                rows.Add(actionProp.TypeName, new ActionViewRow(y, pixelsPerSecond, actionProp.Color));
                y += 19;
            }
        }

        public void Dispose()
        {
            foreach (ActionViewRow row in rows.Values)
            {
                row.Dispose();
            }
        }

        public ActionViewButton addAction(TimelineAction action)
        {
            Button button = scrollView.createWidgetT("Button", "Button", (int)(pixelsPerSecond * action.StartTime), 0, 10, 10, Align.Left | Align.Top, "") as Button;
            ActionViewButton actionButton = rows[action.TypeName].addButton(button, action);
            actionButton.Clicked += new EventHandler(actionButton_Clicked);
            if (button.Right > scrollView.CanvasSize.Width)
            {
                Size2 canvasSize = scrollView.CanvasSize;
                canvasSize.Width = button.Right;
                scrollView.CanvasSize = canvasSize;
            }
            if (button.Bottom > scrollView.CanvasSize.Height)
            {
                Size2 canvasSize = scrollView.CanvasSize;
                canvasSize.Height = button.Bottom;
                scrollView.CanvasSize = canvasSize;
            }
            return actionButton;
        }

        public void removeAction(TimelineAction action)
        {
            ActionViewButton button = rows[action.TypeName].removeButton(action);
            if (button == CurrentAction)
            {
                //Null the internal property first as you do not want to toggle the state of the button that has already been disposed.
                currentButton = null;
                CurrentAction = null;
            }
        }

        public void setCurrentAction(TimelineAction action)
        {
            ActionViewButton button = rows[action.TypeName].findButtonForAction(action);
            CurrentAction = button;
        }

        public void removeAllActions()
        {
            foreach (ActionViewRow row in rows.Values)
            {
                row.removeAllActions();
            }
            currentButton = null;
            CurrentAction = null;
            scrollView.CanvasSize = new Size2(0.0f, 0.0f);
        }

        public ActionViewButton CurrentAction
        {
            get
            {
                return currentButton;
            }
            set
            {
                if (currentButton != null)
                {
                    currentButton.StateCheck = false;
                    currentButton.CoordChanged -= currentButton_CoordChanged;
                }
                currentButton = value;
                if (currentButton != null)
                {
                    currentButton.StateCheck = true;
                    currentButton.CoordChanged += currentButton_CoordChanged;
                }
                if (ActiveActionChanged != null)
                {
                    ActiveActionChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        void currentButton_CoordChanged(object sender, EventArgs e)
        {
            Size2 canvasSize = scrollView.CanvasSize;
            //Ensure the canvas is large enough.
            if (currentButton.Right > scrollView.CanvasSize.Width)
            {
                canvasSize.Width = currentButton.Right;
                scrollView.CanvasSize = canvasSize;
            }
            if (currentButton.Bottom > scrollView.CanvasSize.Height)
            {
                canvasSize = scrollView.CanvasSize;
                canvasSize.Height = currentButton.Bottom;
                scrollView.CanvasSize = canvasSize;
            }

            //Ensure the button is still visible.
            Vector2 canvasPosition = scrollView.CanvasPosition;
            IntCoord clientCoord = scrollView.ClientCoord;

            float visibleSize = canvasPosition.x + clientCoord.width;
            int rightSide = currentButton.Right;
            //If the button is longer than the display area tweak the right side value.
            if (currentButton.Width > clientCoord.width)
            {
                rightSide = currentButton.Left + clientCoord.width;
            }
            //Ensure the right side is visible
            if (rightSide > visibleSize)
            {
                canvasPosition.x += rightSide - visibleSize;
                scrollView.CanvasPosition = canvasPosition;
            }
            //Ensure the left side is visible as well
            else if (currentButton.Left < canvasPosition.x)
            {
                canvasPosition.x = currentButton.Left;
                scrollView.CanvasPosition = canvasPosition;
            }
        }

        void actionButton_Clicked(object sender, EventArgs e)
        {
            CurrentAction = sender as ActionViewButton;
        }
    }
}
