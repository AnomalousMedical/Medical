using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ActionProperties
    {
        private ScrollView actionPropertiesScroll;
        private TimelineAction action;

        private NumericEdit startTime;
        private NumericEdit duration;

        public ActionProperties(ScrollView actionPropertiesScroll)
        {
            this.actionPropertiesScroll = actionPropertiesScroll;

            startTime = new NumericEdit(actionPropertiesScroll.findWidget("StartTime") as Edit);
            startTime.ValueChanged += new MyGUIEvent(startTime_ValueChanged);
            duration = new NumericEdit(actionPropertiesScroll.findWidget("Duration") as Edit);
            duration.ValueChanged += new MyGUIEvent(duration_ValueChanged);
        }

        public TimelineAction CurrentAction
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
                if (action != null)
                {
                    startTime.FloatValue = action.StartTime;
                    duration.FloatValue = action.Duration;
                }
                else
                {
                    startTime.FloatValue = 0.0f;
                    duration.FloatValue = 0.0f;
                }
            }
        }

        public bool Visible
        {
            get
            {
                return actionPropertiesScroll.Visible;
            }
            set
            {
                actionPropertiesScroll.Visible = value;
            }
        }

        void duration_ValueChanged(Widget source, EventArgs e)
        {
            if (action != null)
            {
                action.Duration = duration.FloatValue;
            }
        }

        void startTime_ValueChanged(Widget source, EventArgs e)
        {
            if (action != null)
            {
                action.StartTime = startTime.FloatValue;
                action._sortAction();
            }
        }
    }
}
