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
        private Dictionary<String, int> rowIndexes = new Dictionary<string, int>();
        private List<ActionViewRow> rows = new List<ActionViewRow>();
        private ActionViewButton currentButton;
        private List<IActionViewExtension> extensions = new List<IActionViewExtension>();

        public event EventHandler ActiveActionChanged;

        private const int PREVIEW_PADDING = 10;
        private const int TRACK_START_Y = 3;


        public ActionView(ScrollView scrollView)
        {
            this.scrollView = scrollView;
            scrollView.MouseLostFocus += new MyGUIEvent(scrollView_MouseLostFocus);
            scrollView.MouseWheel += new MyGUIEvent(scrollView_MouseWheel);
            scrollView.KeyButtonPressed += new MyGUIEvent(scrollView_KeyButtonPressed);
            scrollView.KeyButtonReleased += new MyGUIEvent(scrollView_KeyButtonReleased);
            int y = TRACK_START_Y;
            foreach (TimelineActionProperties actionProp in TimelineActionFactory.ActionProperties)
            {
                rows.Add(new ActionViewRow(y, pixelsPerSecond, actionProp.Color));
                rowIndexes.Add(actionProp.TypeName, rows.Count - 1);
                y += 19;
            }
            Size2 canvasSize = scrollView.CanvasSize;
            canvasSize.Height = y;
            scrollView.CanvasSize = canvasSize;
        }

        public void Dispose()
        {
            foreach (ActionViewRow row in rows)
            {
                row.Dispose();
            }
        }

        public void addExtension(IActionViewExtension extension)
        {
            extensions.Add(extension);
            extension.pixelsPerSecondChanged(pixelsPerSecond);
            extension.scrollCanvasSizeChanged(scrollView.CanvasSize);
            extension.scrollPositionChanged(scrollView.CanvasPosition);
        }

        public void removeExtension(IActionViewExtension extension)
        {
            extensions.Remove(extension);
        }

        public ActionViewButton addAction(TimelineAction action)
        {
            Button button = scrollView.createWidgetT("Button", "Button", (int)(pixelsPerSecond * action.StartTime), 0, 10, 10, Align.Left | Align.Top, "") as Button;
            ActionViewButton actionButton = rows[rowIndexes[action.TypeName]].addButton(button, action);
            actionButton.Clicked += new EventHandler(actionButton_Clicked);
            if (button.Right > scrollView.CanvasSize.Width)
            {
                Size2 canvasSize = scrollView.CanvasSize;
                canvasSize.Width = button.Right;
                scrollView.CanvasSize = canvasSize;
            }
            return actionButton;
        }

        public void removeAction(TimelineAction action)
        {
            ActionViewButton button = rows[rowIndexes[action.TypeName]].removeButton(action);
            if (button == CurrentAction)
            {
                //Null the internal property first as you do not want to toggle the state of the button that has already been disposed.
                currentButton = null;
                CurrentAction = null;
            }
        }

        public void setCurrentAction(TimelineAction action)
        {
            ActionViewButton button = rows[rowIndexes[action.TypeName]].findButtonForAction(action);
            CurrentAction = button;
        }

        public void removeAllActions()
        {
            foreach (ActionViewRow row in rows)
            {
                row.removeAllActions();
            }
            currentButton = null;
            CurrentAction = null;
            scrollView.CanvasSize = new Size2(0.0f, rows.Count != 0 ? rows[rows.Count - 1].Bottom : 0.0f);
        }

        public void trimVisibleArea()
        {
            ActionViewButton rightmostButton = null;
            foreach (ActionViewRow row in rows)
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
                scrollView.CanvasSize = new Size2(0.0f, rows.Count != 0 ? rows[rows.Count - 1].Bottom : 0.0f);
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

        public int PixelsPerSecond
        {
            get
            {
                return pixelsPerSecond;
            }
            set
            {
                pixelsPerSecond = value;
                if (pixelsPerSecond < 10)
                {
                    pixelsPerSecond = 10;
                }
                foreach (ActionViewRow row in rows)
                {
                    row.changePixelsPerSecond(pixelsPerSecond);
                }
                trimVisibleArea();
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

            //Ensure the button is still visible.
            Vector2 canvasPosition = scrollView.CanvasPosition;
            int clientWidth = scrollView.ClientCoord.width;

            float visibleSize = canvasPosition.x + clientWidth;
            if (visibleSize + PREVIEW_PADDING < scrollView.CanvasSize.Width)
            {
                visibleSize -= PREVIEW_PADDING;
            }
            int rightSide = currentButton.Right;
            //If the button is longer than the display area tweak the right side value.
            if (currentButton.Width > clientWidth)
            {
                rightSide = currentButton.Left + clientWidth - PREVIEW_PADDING * 2;
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
            PixelsPerSecond += (int)(10 * (me.RelativeWheelPosition / 120.0f));
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

        void firePixelsPerSecondChanged()
        {
            foreach (IActionViewExtension extension in extensions)
            {
                extension.pixelsPerSecondChanged(pixelsPerSecond);
            }
        }

        void fireScrollCanvasSizeChanged()
        {
            Size2 size = scrollView.CanvasSize;
            foreach (IActionViewExtension extension in extensions)
            {
                extension.scrollCanvasSizeChanged(size);
            }
        }

        void firePositionChanged()
        {
            Vector2 position = scrollView.CanvasPosition;
            foreach (IActionViewExtension extension in extensions)
            {
                extension.scrollPositionChanged(position);
            }
        }
    }
}
