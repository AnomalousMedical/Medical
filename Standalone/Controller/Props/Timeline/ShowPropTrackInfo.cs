using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical
{
    sealed class ShowPropTrackInfo
    {
        private List<ShowPropSubActionPrototype> trackData = new List<ShowPropSubActionPrototype>();

        public ShowPropTrackInfo()
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

        public IEnumerable<ShowPropSubActionPrototype> Tracks
        {
            get
            {
                return trackData;
            }
        }
    }
}
