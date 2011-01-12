using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TimelineActionData : TimelineData
    {
        public delegate void DurationChangedEvent(float duration);

        public event DurationChangedEvent DurationChanged;

        private TimelineAction action;

        public TimelineActionData(TimelineAction action)
        {
            this.action = action;
        }

        public override void editingStarted()
        {
            action.editing();
        }

        public override void editingCompleted()
        {
            action.editingCompleted();
        }

        public TimelineAction Action
        {
            get
            {
                return action;
            }
        }

        public override float _StartTime
        {
            get
            {
                return action.StartTime;
            }
            set
            {
                action.StartTime = value;
                action._sortAction();
            }
        }

        public override float _Duration
        {
            get
            {
                return action.Duration;
            }
            set
            {
                action.Duration = value;
                if (DurationChanged != null)
                {
                    DurationChanged.Invoke(value);
                }
            }
        }

        public override String Track
        {
            get
            {
                return action.TypeName;
            }
        }
    }
}
