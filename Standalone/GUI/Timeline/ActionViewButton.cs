using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.ComponentModel;
using Engine;

namespace Medical.GUI
{
    class ActionViewButtonEventArgs : EventArgs
    {
        public int OldLeft { get; private set; }
        public int OldRight { get; private set; }
        public int OldTop { get; private set; }
        public int OldBottom { get; private set; }

        internal void _setValues(Button button)
        {
            OldLeft = button.Left;
            OldTop = button.Top;
            OldRight = button.Right;
            OldBottom = button.Bottom;
        }
    }

    class ActionViewButton
    {
        private Button button;
        private TimelineAction action;
        private int pixelsPerSecond;
        private float dragStartPos;
        private float dragStartTime;

        public event EventHandler Clicked;
        public event EventHandler CoordChanged;

        private static ActionViewButtonEventArgs sharedEventArgs = new ActionViewButtonEventArgs();

        public ActionViewButton(int pixelsPerSecond, Button button, TimelineAction action)
        {
            this.pixelsPerSecond = pixelsPerSecond;
            this.action = action;
            this.button = button;
            button.MouseDrag += new MyGUIEvent(button_MouseDrag);
            button.MouseButtonPressed += new MyGUIEvent(button_MouseButtonPressed);
            updateDurationWidth();
        }

        /// <summary>
        /// Move the top of the button, should only be called by ActionViewRow.
        /// This will not fire the coordChanged event.
        /// </summary>
        /// <param name="top"></param>
        internal void _moveTop(int top)
        {
            button.setPosition(button.Left, top);
        }

        void button_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me.Button == Engine.Platform.MouseButtonCode.MB_BUTTON0)
            {
                if (Clicked != null)
                {
                    Clicked.Invoke(this, e);
                }

                dragStartPos = me.Position.x;
                dragStartTime = StartTime;
            }
        }

        void button_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            float newStartTime = dragStartTime + (me.Position.x - dragStartPos) / pixelsPerSecond;
            if(newStartTime < 0.0f)
            {
                newStartTime = 0.0f;
            }
            StartTime = newStartTime;
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(button);
        }

        public TimelineAction Action
        {
            get
            {
                return action;
            }
        }

        public bool StateCheck
        {
            get
            {
                return button.StateCheck;
            }
            set
            {
                button.StateCheck = value;
            }
        }

        public float StartTime
        {
            get
            {
                return action.StartTime;
            }
            set
            {
                action.StartTime = value;
                action._sortAction();
                updatePosition();
            }
        }

        public float Duration
        {
            get
            {
                return action.Duration;
            }
            set
            {
                action.Duration = value;
                updateDurationWidth();
            }
        }

        public int Top
        {
            get
            {
                return button.Top;
            }
        }

        public int Left
        {
            get
            {
                return button.Left;
            }
        }

        public int Right
        {
            get
            {
                return button.Right;
            }
        }

        public int Width
        {
            get
            {
                return button.Width;
            }
        }

        public int Bottom
        {
            get
            {
                return button.Bottom;
            }
        }

        public void setColor(Color color)
        {
            button.setColour(color);
        }

        internal void changePixelsPerSecond(int pixelsPerSecond)
        {
            this.pixelsPerSecond = pixelsPerSecond;
            updatePosition();
            updateDurationWidth();
        }

        private void updatePosition()
        {
            sharedEventArgs._setValues(button);
            button.setPosition((int)(action.StartTime * pixelsPerSecond), button.Top);
            if (CoordChanged != null)
            {
                CoordChanged.Invoke(this, sharedEventArgs);
            }
        }

        private void updateDurationWidth()
        {
            sharedEventArgs._setValues(button);
            int buttonWidth = (int)(action.Duration * pixelsPerSecond);
            if (buttonWidth < 10)
            {
                buttonWidth = 10;
            }
            button.setSize(buttonWidth, button.Height);
            if (CoordChanged != null)
            {
                CoordChanged.Invoke(this, sharedEventArgs);
            }
        }
    }
}
