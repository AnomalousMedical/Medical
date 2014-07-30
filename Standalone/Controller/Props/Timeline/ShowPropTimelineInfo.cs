using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical
{
    sealed class ShowPropTimelineInfo
    {
        private List<ShowPropSubActionPrototype> trackData = new List<ShowPropSubActionPrototype>();

        public ShowPropTimelineInfo()
        {

        }

        public void addTrack(ShowPropSubActionPrototype prototype)
        {
            trackData.Add(prototype);
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

        public PropTimelineData createData(ShowPropSubAction subAction, PropEditController propEditController)
        {
            foreach (ShowPropSubActionPrototype data in trackData)
            {
                if (data.TypeName == subAction.TypeName)
                {
                    return data.createData(subAction, propEditController);
                }
            }
            return null;
        }

        public IEnumerable<ShowPropSubActionPrototype> Tracks
        {
            get
            {
                return trackData;
            }
        }
    }
}
