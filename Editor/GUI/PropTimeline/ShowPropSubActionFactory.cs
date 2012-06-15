using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowPropSubActionFactory : IDisposable
    {
        Dictionary<String, ShowPropSubActionFactoryData> trackInfo = new Dictionary<string, ShowPropSubActionFactoryData>();
        private MovePropProperties movePropProperties;
        private PoseableHandProperties leftPoseableHandProperties;
        private PoseableHandProperties rightPoseableHandProperties;

        private Dictionary<ShowPropSubAction, PropTimelineData> actionDataBindings = new Dictionary<ShowPropSubAction, PropTimelineData>();

        public ShowPropSubActionFactory(Widget parentWidget, PropEditController propEditController)
        {
            movePropProperties = new MovePropProperties(parentWidget, propEditController);
            leftPoseableHandProperties = new PoseableHandProperties(parentWidget, "Medical.GUI.PropTimeline.SubActionProperties.PoseableLeftHandProperties.layout");
            rightPoseableHandProperties = new PoseableHandProperties(parentWidget, "Medical.GUI.PropTimeline.SubActionProperties.PoseableRightHandProperties.layout");

            //Arrow
            ShowPropSubActionFactoryData arrowData = new ShowPropSubActionFactoryData();
            arrowData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            arrowData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            arrowData.addTrack(typeof(ChangeArrowColorAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), null);
            arrowData.addTrack(typeof(ChangeArrowShapeAction), new Color(128 / 255f, 0 / 128f, 255 / 255f), null);
            trackInfo.Add(Arrow.DefinitionName, arrowData);

            //Doppler
            ShowPropSubActionFactoryData dopplerData = new ShowPropSubActionFactoryData();
            dopplerData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            dopplerData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            trackInfo.Add(Doppler.DefinitionName, dopplerData);

            //PointingHandLeft
            ShowPropSubActionFactoryData pointingHandLeftData = new ShowPropSubActionFactoryData();
            pointingHandLeftData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            pointingHandLeftData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            pointingHandLeftData.addTrack(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), null);
            trackInfo.Add(PointingHand.LeftHandName, pointingHandLeftData);

            //PointingHandRight
            ShowPropSubActionFactoryData pointingRightHandData = new ShowPropSubActionFactoryData();
            pointingRightHandData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            pointingRightHandData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            pointingRightHandData.addTrack(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), null);
            trackInfo.Add(PointingHand.RightHandName, pointingRightHandData);

            //Ruler
            ShowPropSubActionFactoryData rulerData = new ShowPropSubActionFactoryData();
            rulerData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            rulerData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            trackInfo.Add(Ruler.DefinitionName, rulerData);

            //Syringe
            ShowPropSubActionFactoryData syringeData = new ShowPropSubActionFactoryData();
            syringeData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            syringeData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            syringeData.addTrack(typeof(PushPlungerAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), null);
            trackInfo.Add(Syringe.DefinitionName, syringeData);

            //Circular Highlight
            ShowPropSubActionFactoryData circularHighlightData = new ShowPropSubActionFactoryData();
            circularHighlightData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            circularHighlightData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            circularHighlightData.addTrack(typeof(ChangeCircularHighlightSettings), new Color(128 / 255f, 0 / 255f, 255 / 255f), null);
            trackInfo.Add(CircularHighlight.DefinitionName, circularHighlightData);

            //Poseable Hand Left
            ShowPropSubActionFactoryData poseableHandLeftData = new ShowPropSubActionFactoryData();
            poseableHandLeftData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            poseableHandLeftData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            poseableHandLeftData.addTrack(typeof(ChangeHandPosition), new Color(128 / 255f, 0 / 255f, 255 / 255f), leftPoseableHandProperties);
            poseableHandLeftData.addTrack(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 128f, 255 / 255f), null);
            trackInfo.Add(PoseableHand.LeftDefinitionName, poseableHandLeftData);

            //Poseable Hand Right
            ShowPropSubActionFactoryData poseableHandRightData = new ShowPropSubActionFactoryData();
            poseableHandRightData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            poseableHandRightData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            poseableHandRightData.addTrack(typeof(ChangeHandPosition), new Color(128 / 255f, 0 / 255f, 255 / 255f), rightPoseableHandProperties);
            poseableHandRightData.addTrack(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 128f, 255 / 255f), null);
            trackInfo.Add(PoseableHand.RightDefinitionName, poseableHandRightData);

            //Bite Stick
            ShowPropSubActionFactoryData biteStickData = new ShowPropSubActionFactoryData();
            biteStickData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            biteStickData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            trackInfo.Add(BiteStick.DefinitionName, biteStickData);

            //Range of MotionScale
            ShowPropSubActionFactoryData rangeOfMotionScale = new ShowPropSubActionFactoryData();
            rangeOfMotionScale.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            rangeOfMotionScale.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            trackInfo.Add(RangeOfMotionScale.DefinitionName, rangeOfMotionScale);

            //Pen
            ShowPropSubActionFactoryData penData = new ShowPropSubActionFactoryData();
            penData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            penData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            penData.addTrack(typeof(ClickPenAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), null);
            trackInfo.Add(Pen.DefinitionName, penData);

            //Caliper
            ShowPropSubActionFactoryData caliperData = new ShowPropSubActionFactoryData();
            caliperData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            caliperData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            caliperData.addTrack(typeof(SetCaliperMeasurement), new Color(128 / 255f, 0 / 255f, 255 / 255f), null);
            trackInfo.Add(Caliper.DefinitionName, caliperData);

            //Plane
            ShowPropSubActionFactoryData planeData = new ShowPropSubActionFactoryData();
            planeData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            planeData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), null);
            planeData.addTrack(typeof(ChangePlaneSettings), new Color(128 / 255f, 0 / 255f, 255 / 255f), null);
            trackInfo.Add(Plane.DefinitionName, planeData);
        }

        public void Dispose()
        {
            movePropProperties.Dispose();
            leftPoseableHandProperties.Dispose();
            rightPoseableHandProperties.Dispose();
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

        public PropTimelineData createData(ShowPropSubAction subAction)
        {
            PropTimelineData timelineData = new PropTimelineData(subAction);
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
