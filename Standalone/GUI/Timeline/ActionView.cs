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
            scrollView.MouseLostFocus += new MyGUIEvent(scrollView_MouseLostFocus);
            scrollView.MouseWheel += new MyGUIEvent(scrollView_MouseWheel);
            scrollView.KeyButtonPressed += new MyGUIEvent(scrollView_KeyButtonPressed);
            scrollView.KeyButtonReleased += new MyGUIEvent(scrollView_KeyButtonReleased);
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

        public void trimVisibleArea()
        {
            ActionViewButton rightmostButton = null;
            foreach (ActionViewRow row in rows.Values)
            {
                row.findRightmostButton(ref rightmostButton);
            }
            if (rightmostButton != null)
            {
                Size2 canvasSize = scrollView.CanvasSize;
                canvasSize.Width = rightmostButton.Right;
                scrollView.CanvasSize = canvasSize;
            }
            else
            {
                scrollView.CanvasSize = new Size2(0.0f, 0.0f);
            }
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

        private int PREVIEW_PADDING = 10;

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
            if (visibleSize + PREVIEW_PADDING < scrollView.CanvasSize.Width)
            {
                visibleSize -= PREVIEW_PADDING;
            }
            int rightSide = currentButton.Right;
            //If the button is longer than the display area tweak the right side value.
            if (currentButton.Width > clientCoord.width)
            {
                rightSide = currentButton.Left + clientCoord.width - PREVIEW_PADDING * 2;
            }
            //Ensure the right side is visible
            if (rightSide > visibleSize)
            {
                canvasPosition.x += rightSide - visibleSize;
                scrollView.CanvasPosition = canvasPosition;
            }
            //Ensure the left side is visible as well
            else if (currentButton.Left < canvasPosition.x + PREVIEW_PADDING)
            {
                canvasPosition.x = currentButton.Left - PREVIEW_PADDING;
                if (canvasPosition.x < 0.0f)
                {
                    canvasPosition.x = 0.0f;
                }
                scrollView.CanvasPosition = canvasPosition;
            }
        }

        void actionButton_Clicked(object sender, EventArgs e)
        {
            CurrentAction = sender as ActionViewButton;
        }

        void scrollView_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = e as KeyEventArgs;
            if (ke.Key == Engine.Platform.KeyboardButtonCode.KC_LCONTROL)
            {
                scrollView.AllowMouseScroll = true;
            }
        }

        void scrollView_KeyButtonPressed(Widget source, EventArgs e)
        {
            KeyEventArgs ke = e as KeyEventArgs;
            if (ke.Key == Engine.Platform.KeyboardButtonCode.KC_LCONTROL)
            {
                scrollView.AllowMouseScroll = false;
            }
        }

        void scrollView_MouseWheel(Widget source, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            pixelsPerSecond += (int)(10 * (me.RelativeWheelPosition / 120.0f));
            if (pixelsPerSecond < 10)
            {
                pixelsPerSecond = 10;
            }
            foreach (ActionViewRow row in rows.Values)
            {
                row.changePixelsPerSecond(pixelsPerSecond);
            }
            trimVisibleArea();
        }

        void scrollView_MouseLostFocus(Widget source, EventArgs e)
        {
            FocusEventArgs fe = e as FocusEventArgs;
            Widget newFocus = fe.OtherWidget;
            int absRight = scrollView.AbsoluteLeft + scrollView.Width;
            int absBottom = scrollView.AbsoluteTop + scrollView.Height;
            if (newFocus.AbsoluteLeft < scrollView.AbsoluteLeft || newFocus.AbsoluteLeft > absRight
                                        || newFocus.AbsoluteTop < scrollView.AbsoluteTop || newFocus.AbsoluteTop > absBottom)
            {
                scrollView.AllowMouseScroll = true;
            }
        }
    }
}
