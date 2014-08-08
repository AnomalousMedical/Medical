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
            onTrackAdded(prototype);
        }

        public void removeTrack(ShowPropSubActionPrototype prototype)
        {
            trackData.Remove(prototype);
            onTrackRemoved(prototype);
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
                    editInterface.addCommand(new EditInterfaceCommand("Add Track", addTrack));
                    var trackPrototypeManager = editInterface.createEditInterfaceManager<ShowPropSubActionPrototype>();
                    trackPrototypeManager.addCommand(new EditInterfaceCommand("Remove", removeTrack));
                    foreach(var track in trackData)
                    {
                        onTrackAdded(track);
                    }
                }
                return editInterface;
            }
        }

        private void addTrack(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.getInputString("Enter a name for the track.", delegate(String trackName, ref String trackErrorPrompt)
            {
                if (String.IsNullOrEmpty(trackName))
                {
                    trackErrorPrompt = "You must enter a name for the track.";
                    return false;
                }

                TypeBrowser browser = new TypeBrowser("Track Types", "Choose a track type", typeof(ShowPropSubAction));
                callback.showBrowser(browser, delegate(Type resultType, ref String typeBrowseErrorPrompt)
                {
                    addTrack(new ShowPropSubActionPrototype(resultType, trackName));
                    return true;
                });

                return true;
            });
        }

        private void removeTrack(EditUICallback callback, EditInterfaceCommand caller)
        {
            removeTrack(editInterface.resolveSourceObject<ShowPropSubActionPrototype>(callback.getSelectedEditInterface()));
        }

        private void onTrackAdded(ShowPropSubActionPrototype prototype)
        {
            if (editInterface != null)
            {
                editInterface.addSubInterface(prototype, prototype.EditInterface);
            }
        }

        private void onTrackRemoved(ShowPropSubActionPrototype prototype)
        {
            if(editInterface != null)
            {
                editInterface.removeSubInterface(prototype);
            }
        }
    }
}
