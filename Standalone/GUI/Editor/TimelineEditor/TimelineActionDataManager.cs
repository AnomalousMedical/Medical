using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class TimelineActionDataManager : IDisposable
    {
        private Dictionary<TimelineAction, TimelineActionData> actionDataBindings = new Dictionary<TimelineAction, TimelineActionData>();
        private Dictionary<Type, TimelineActionPrototype> actionPrototypesType = new Dictionary<Type, TimelineActionPrototype>();
        private Dictionary<String, TimelineActionPrototype> actionPrototypesName = new Dictionary<String, TimelineActionPrototype>();

        public TimelineActionDataManager()
        {

        }

        public void Dispose()
        {
            actionPrototypesName.Clear();
            actionPrototypesType.Clear();
            clearData();
        }

        public void addPrototype(TimelineActionPrototype prototype)
        {
            actionPrototypesType.Add(prototype.Type, prototype);
            actionPrototypesName.Add(prototype.TypeName, prototype);
        }

        public TimelineAction createAction(String typeName)
        {
            return actionPrototypesName[typeName].createInstance();
        }

        public IEnumerable<TimelineActionPrototype> Prototypes
        {
            get
            {
                return actionPrototypesName.Values;
            }
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
            TimelineActionData actionData = actionPrototypesType[action.GetType()].createData(action);
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
