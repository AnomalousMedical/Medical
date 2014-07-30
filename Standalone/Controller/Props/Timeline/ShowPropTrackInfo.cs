using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical
{
    /// <summary>
    /// A collection of track prototypes for a given prop type. Determines what appears on a given prop's prop
    /// timeline.
    /// </summary>
    public sealed class ShowPropTrackInfo
    {
        private List<ShowPropSubActionPrototype> trackData = new List<ShowPropSubActionPrototype>();

        public ShowPropTrackInfo()
        {

        }

        public void addTrack(ShowPropSubActionPrototype prototype)
        {
            trackData.Add(prototype);
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
