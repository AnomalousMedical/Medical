﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ActionViewButton
    {
        private Button button;
        private TimelineAction action;
        private int pixelsPerSecond;
        private float dragStartPos;
        private float dragStartTime;

        public event EventHandler Clicked;

        public ActionViewButton(int pixelsPerSecond, Button button, TimelineAction action)
        {
            this.pixelsPerSecond = pixelsPerSecond;
            this.action = action;
            this.button = button;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
            button.MouseDrag += new MyGUIEvent(button_MouseDrag);
            button.MouseButtonPressed += new MyGUIEvent(button_MouseButtonPressed);
            setDurationWidth();
        }

        void button_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me.Button == Engine.Platform.MouseButtonCode.MB_BUTTON0)
            {
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
                button.setPosition((int)(action.StartTime * pixelsPerSecond), button.Top);
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
                setDurationWidth();
            }
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            if (Clicked != null)
            {
                Clicked.Invoke(this, e);
            }
        }

        private void setDurationWidth()
        {
            int buttonWidth = (int)(action.Duration * pixelsPerSecond);
            if (buttonWidth < 10)
            {
                buttonWidth = 10;
            }
            button.setSize(buttonWidth, button.Height);
        }
    }
}
