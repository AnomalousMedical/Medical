using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowPropSubActionFactory
    {
        Dictionary<String, ShowPropSubActionFactoryData> trackInfo = new Dictionary<string, ShowPropSubActionFactoryData>();

        public ShowPropSubActionFactory(Widget parentWidget)
        {
            ShowPropSubActionFactoryData syringeData = new ShowPropSubActionFactoryData();
            syringeData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), null);
            syringeData.addTrack(typeof(PushPlungerAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), new PushPlungerProperties(parentWidget));
            trackInfo.Add("Syringe", syringeData);
        }

        public void addTracksForAction(ShowPropAction showProp, TimelineView timelineView, TimelineDataProperties actionProperties)
        {
            ShowPropSubActionFactoryData track;
            if(trackInfo.TryGetValue(showProp.PropType, out track))
            {
                track.addTracksToTimeline(timelineView, actionProperties);
            }
        }

        public ShowPropSubAction createSubAction(ShowPropAction showProp, String name)
        {
            return trackInfo[showProp.PropType].createSubAction(name);
        }
    }

    class ShowPropSubActionFactoryData : IDisposable
    {
        private List<TrackData> trackData = new List<TrackData>();

        public ShowPropSubActionFactoryData()
        {

        }

        public void Dispose()
        {
            foreach (TrackData data in trackData)
            {
                data.Dispose();
            }
        }

        public void addTrack(Type type, Color color, TimelineDataPanel dataPanel)
        {
            TrackData data = new TrackData(type, color, dataPanel, getTypeName(type));
            trackData.Add(data);
        }

        public void addTracksToTimeline(TimelineView timeline, TimelineDataProperties actionProperties)
        {
            foreach (TrackData data in trackData)
            {
                timeline.addTrack(data.TypeName, data.Color);
                if (data.Panel != null)
                {
                    actionProperties.addPanel(data.TypeName, data.Panel);
                }
            }
        }

        public ShowPropSubAction createSubAction(string name)
        {
            foreach (TrackData data in trackData)
            {
                if (data.TypeName == name)
                {
                    return (ShowPropSubAction)Activator.CreateInstance(data.Type);
                }
            }
            return null;
        }

        private String getTypeName(Type type)
        {
            try
            {
                TimelineActionProperties properties = (TimelineActionProperties)(type.GetCustomAttributes(typeof(TimelineActionProperties), false)[0]);
                return properties.TypeName;
            }
            catch (Exception)
            {
                throw new Exception("All ShowPropSubAction added to the factory must have a TimelineActionProperties attribute.");
            }
        }

        class TrackData : IDisposable
        {
            public TrackData(Type type, Color color, TimelineDataPanel dataPanel, String typeName)
            {
                this.Color = color;
                this.Type = type;
                this.Panel = dataPanel;
                this.TypeName = typeName;
            }

            public void Dispose()
            {
                if (Panel != null)
                {
                    Panel.Dispose();
                }
            }

            public Color Color { get; private set; }

            public Type Type { get; private set; }

            public String TypeName { get; private set; }

            public TimelineDataPanel Panel { get; private set; }
        }
    }
}
