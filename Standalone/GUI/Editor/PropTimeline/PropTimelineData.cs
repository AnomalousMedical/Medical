using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    sealed class PropTimelineData : TimelineData, IDisposable
    {
        private ShowPropSubAction action;
        private PropEditController propEditController;

        public PropTimelineData(ShowPropSubAction action, PropEditController propEditController)
        {
            this.action = action;
            this.propEditController = propEditController;
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
            action.editing(propEditController);
        }

        public override void editingCompleted()
        {
            action.editingCompleted(propEditController);
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
