using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    delegate void ActionViewRowEvent(ActionViewRow row);

    class ActionView : IDisposable
    {
        private TimelineScrollView timelineScrollView;
        private int pixelsPerSecond = 100;
        private Dictionary<String, int> rowIndexes = new Dictionary<string, int>();
        private List<ActionViewRow> rows = new List<ActionViewRow>();
        private ActionViewButton currentButton;

        public event EventHandler ActiveActionChanged;
        public event ActionViewRowEvent RowPositionChanged;
        public event CanvasSizeChanged CanvasWidthChanged
        {
            add
            {
                timelineScrollView.CanvasWidthChanged += value;
            }
            remove
            {
                timelineScrollView.CanvasWidthChanged -= value;
            }
        }

        public event CanvasSizeChanged CanvasHeightChanged
        {
            add
            {
                timelineScrollView.CanvasHeightChanged += value;
            }
            remove
            {
                timelineScrollView.CanvasHeightChanged -= value;
            }
        }

        private const int PREVIEW_PADDING = 10;
        private const int TRACK_START_Y = 3;


        public ActionView(ScrollView scrollView)
        {
            timelineScrollView = new TimelineScrollView(scrollView);
            scrollView.MouseLostFocus += new MyGUIEvent(scrollView_MouseLostFocus);
            scrollView.MouseWheel += new MyGUIEvent(scrollView_MouseWheel);
            scrollView.KeyButtonPressed += new MyGUIEvent(scrollView_KeyButtonPressed);
            scrollView.KeyButtonReleased += new MyGUIEvent(scrollView_KeyButtonReleased);
            int y = TRACK_START_Y;
            foreach (TimelineActionProperties actionProp in TimelineActionFactory.ActionProperties)
            {
                ActionViewRow actionViewRow = new ActionViewRow(actionProp.TypeName, y, pixelsPerSecond, actionProp.Color);
                actionViewRow.BottomChanged += new EventHandler(actionViewRow_BottomChanged);
                rows.Add(actionViewRow);
                rowIndexes.Add(actionProp.TypeName, rows.Count - 1);
                y = actionViewRow.Bottom;
            }
            timelineScrollView.CanvasHeight = y;
        }

        public void Dispose()
        {
            foreach (ActionViewRow row in rows)
            {
                row.Dispose();
            }
        }

        public ActionViewButton addAction(TimelineAction action)
        {
            Button button = timelineScrollView.createButton(pixelsPerSecond * action.StartTime);
            ActionViewButton actionButton = rows[rowIndexes[action.TypeName]].addButton(button, action);
            actionButton.Clicked += new EventHandler(actionButton_Clicked);
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
            timelineScrollView.CanvasWidth = 0.0f;
            timelineScrollView.CanvasHeight = rows.Count != 0 ? rows[rows.Count - 1].Bottom : 0.0f;
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
                timelineScrollView.CanvasWidth = rightmostButton.Right;
            }
            else
            {
                timelineScrollView.CanvasWidth = 0.0f;
                timelineScrollView.CanvasHeight = rows.Count != 0 ? rows[rows.Count - 1].Bottom : 0.0f;
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

        public IEnumerable<ActionViewRow> Rows
        {
            get
            {
                return rows;
            }
        }

        void currentButton_CoordChanged(object sender, EventArgs e)
        {
            float canvasWidth = timelineScrollView.CanvasWidth;
            //Ensure the canvas is large enough.
            if (currentButton.Right > canvasWidth)
            {
                timelineScrollView.CanvasWidth = currentButton.Right;
            }

            //Ensure the button is still visible.
            Vector2 canvasPosition = timelineScrollView.CanvasPosition;
            int clientWidth = timelineScrollView.ClientCoord.width;

            float visibleSize = canvasPosition.x + clientWidth;
            if (visibleSize + PREVIEW_PADDING < canvasWidth)
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
                timelineScrollView.CanvasPosition = canvasPosition;
            }
            //Ensure the left side is visible as well
            else if (currentButton.Left < canvasPosition.x + PREVIEW_PADDING)
            {
                canvasPosition.x = currentButton.Left - PREVIEW_PADDING;
                if (canvasPosition.x < 0.0f)
                {
                    canvasPosition.x = 0.0f;
                }
                timelineScrollView.CanvasPosition = canvasPosition;
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
                timelineScrollView.AllowMouseScroll = true;
            }
        }

        void scrollView_KeyButtonPressed(Widget source, EventArgs e)
        {
            KeyEventArgs ke = e as KeyEventArgs;
            if (ke.Key == Engine.Platform.KeyboardButtonCode.KC_LCONTROL)
            {
                timelineScrollView.AllowMouseScroll = false;
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
            if (newFocus == null)
            {
                timelineScrollView.AllowMouseScroll = true;
            }
            else
            {
                int absRight = timelineScrollView.AbsoluteLeft + timelineScrollView.Width;
                int absBottom = timelineScrollView.AbsoluteTop + timelineScrollView.Height;
                if (newFocus.AbsoluteLeft < timelineScrollView.AbsoluteLeft || newFocus.AbsoluteLeft > absRight
                                            || newFocus.AbsoluteTop < timelineScrollView.AbsoluteTop || newFocus.AbsoluteTop > absBottom)
                {
                    timelineScrollView.AllowMouseScroll = true;
                }
            }
        }

        void fireRowPositionChanged(ActionViewRow row)
        {
            if (RowPositionChanged != null)
            {
                RowPositionChanged.Invoke(row);
            }
        }

        void actionViewRow_BottomChanged(object sender, EventArgs e)
        {
            bool shiftRow = false;
            int lastBottom = 0;
            foreach (ActionViewRow row in rows)
            {
                if (shiftRow)
                {
                    row.moveEntireRow(lastBottom);
                    fireRowPositionChanged(row);
                }
                else
                {
                    shiftRow = row == sender;
                }
                lastBottom = row.Bottom;
            }
            timelineScrollView.CanvasHeight = lastBottom;
        }
    }
}
