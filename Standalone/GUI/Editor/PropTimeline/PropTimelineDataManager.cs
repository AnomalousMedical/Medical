using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    /// <summary>
    /// This class manages the PropTimelineData for the prop timeline since those object have a
    /// dispose lifecycle.
    /// </summary>
    class PropTimelineDataManager : IDisposable
    {
        private Dictionary<ShowPropSubAction, PropTimelineData> actionDataBindings = new Dictionary<ShowPropSubAction, PropTimelineData>();

        public PropTimelineDataManager()
        {

        }

        public void Dispose()
        {
            clearData();
        }

        public PropTimelineData createData(ShowPropAction showProp, ShowPropSubAction subAction, PropEditController propEditController)
        {
            PropTimelineData timelineData = new PropTimelineData(subAction, propEditController);
            actionDataBindings.Add(subAction, timelineData);
            return timelineData;
        }

        public void destroyData(ShowPropSubAction subAction)
        {
            actionDataBindings[subAction].Dispose();
            actionDataBindings.Remove(subAction);
        }

        public void clearData()
        {
            foreach (PropTimelineData data in actionDataBindings.Values)
            {
                data.Dispose();
            }
            actionDataBindings.Clear();
        }

        public PropTimelineData this[ShowPropSubAction subAction]
        {
            get
            {
                return actionDataBindings[subAction];
            }
        }
    }
}
