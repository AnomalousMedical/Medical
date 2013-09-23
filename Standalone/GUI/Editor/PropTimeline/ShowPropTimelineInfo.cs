using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ShowPropTimelineInfo
    {
        private List<ShowPropSubActionPrototype> trackData = new List<ShowPropSubActionPrototype>();

        public ShowPropTimelineInfo()
        {

        }

        public void addTrack(ShowPropSubActionPrototype prototype)
        {
            trackData.Add(prototype);
        }

        public void addTracksToTimeline(TimelineView timeline)
        {
            foreach (ShowPropSubActionPrototype data in trackData)
            {
                timeline.addTrack(data.TypeName);
            }
        }

        public ShowPropSubAction createSubAction(string name)
        {
            foreach (ShowPropSubActionPrototype data in trackData)
            {
                if (data.TypeName == name)
                {
                    return data.createSubAction();
                }
            }
            return null;
        }

        public PropTimelineData createData(ShowPropSubAction subAction)
        {
            foreach (ShowPropSubActionPrototype data in trackData)
            {
                if (data.TypeName == subAction.TypeName)
                {
                    return data.createData(subAction);
                }
            }
            return null;
        }
    }
}
