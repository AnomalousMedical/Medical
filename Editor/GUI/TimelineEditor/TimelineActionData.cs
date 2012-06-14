using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TimelineActionData : TimelineData, IDisposable
    {
        private TimelineAction action;

        public TimelineActionData(TimelineAction action)
        {
            this.action = action;
            action.DurationChanged += action_DurationChanged;
            action.StartTimeChanged += action_StartTimeChanged; 
        }

        public virtual void Dispose()
        {
            action.DurationChanged -= action_DurationChanged;
            action.StartTimeChanged -= action_StartTimeChanged; 
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
            }
        }

        public override String Track
        {
            get
            {
                return action.TypeName;
            }
        }

        void action_StartTimeChanged(TimelineAction obj)
        {
            StartTime = obj.StartTime;
        }

        void action_DurationChanged(TimelineAction obj)
        {
            Duration = obj.Duration;
        }
    }
}
