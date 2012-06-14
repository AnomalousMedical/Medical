using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class TimelineActionDataManager : IDisposable
    {
        private Dictionary<TimelineAction, TimelineActionData> actionDataBindings = new Dictionary<TimelineAction, TimelineActionData>();

        public TimelineActionDataManager()
        {

        }

        public void Dispose()
        {
            clearData();
        }

        public TimelineActionData this[TimelineAction action]
        {
            get
            {
                return actionDataBindings[action];
            }
        }

        public TimelineActionData createData(TimelineAction action)
        {
            TimelineActionData actionData = new TimelineActionData(action);
            actionDataBindings.Add(action, actionData);
            return actionData;
        }

        public void destroyData(TimelineAction action)
        {
            actionDataBindings[action].Dispose();
            actionDataBindings.Remove(action);
        }

        public void clearData()
        {
            foreach (TimelineActionData data in actionDataBindings.Values)
            {
                data.Dispose();
            }
            actionDataBindings.Clear();
        }
    }
}
