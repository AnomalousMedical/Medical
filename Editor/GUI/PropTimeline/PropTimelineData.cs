using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PropTimelineData : TimelineData, IDisposable
    {
        private ShowPropSubAction action;

        public PropTimelineData(ShowPropSubAction action)
        {
            this.action = action;
            action.StartTimeChanged += action_StartTimeChanged;
            action.DurationChanged += action_DurationChanged;
        }

        public void Dispose()
        {
            action.StartTimeChanged -= action_StartTimeChanged;
            action.DurationChanged -= action_DurationChanged;
        }

        public override void editingStarted()
        {
            action.editing();
        }

        public override void editingCompleted()
        {
            action.editingCompleted();
        }

        public ShowPropSubAction Action
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

        void action_DurationChanged(ShowPropSubAction obj)
        {
            Duration = obj.Duration;
        }

        void action_StartTimeChanged(ShowPropSubAction obj)
        {
            StartTime = obj.StartTime;
        }
    }
}
