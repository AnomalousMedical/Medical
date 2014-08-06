using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    /// <summary>
    /// A collection of track prototypes for a given prop type. Determines what appears on a given prop's prop
    /// timeline.
    /// </summary>
    public sealed class ShowPropTrackInfo : Saveable
    {
        private List<ShowPropSubActionPrototype> trackData = new List<ShowPropSubActionPrototype>();

        public ShowPropTrackInfo()
        {

        }

        public void addTrack(ShowPropSubActionPrototype prototype)
        {
            trackData.Add(prototype);
        }

        public void removeTrack(ShowPropSubActionPrototype prototype)
        {
            trackData.Remove(prototype);
        }

        public IEnumerable<ShowPropSubActionPrototype> Tracks
        {
            get
            {
                return trackData;
            }
        }

        private ShowPropTrackInfo(LoadInfo info)
        {
            info.RebuildList("Track", trackData);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList("Track", trackData);
        }

        private EditInterface editInterface;
        public EditInterface EditInterface
        {
            get
            {
                if(editInterface == null)
                {
                    editInterface = new EditInterface("Timeline Tracks");
                    foreach(var track in trackData)
                    {
                        editInterface.addSubInterface(track.EditInterface);
                    }
                }
                return editInterface;
            }
        }
    }
}
