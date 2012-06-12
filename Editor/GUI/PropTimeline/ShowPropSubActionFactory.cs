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
        private EditInterfaceProperties editInterfaceProperties;
        private PushPlungerProperties pushPlungerProperties;
        private PoseableHandProperties leftPoseableHandProperties;
        private PoseableHandProperties rightPoseableHandProperties;

        public ShowPropSubActionFactory(Widget parentWidget, PropEditController propEditController)
        {
            movePropProperties = new MovePropProperties(parentWidget, propEditController);
            editInterfaceProperties = new EditInterfaceProperties(parentWidget);
            pushPlungerProperties = new PushPlungerProperties(parentWidget);
            leftPoseableHandProperties = new PoseableHandProperties(parentWidget, "Medical.GUI.PropTimeline.SubActionProperties.PoseableLeftHandProperties.layout");
            rightPoseableHandProperties = new PoseableHandProperties(parentWidget, "Medical.GUI.PropTimeline.SubActionProperties.PoseableRightHandProperties.layout");

            //Arrow
            ShowPropSubActionFactoryData arrowData = new ShowPropSubActionFactoryData();
            arrowData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            arrowData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            arrowData.addTrack(typeof(ChangeArrowColorAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), editInterfaceProperties);
            arrowData.addTrack(typeof(ChangeArrowShapeAction), new Color(128 / 255f, 0 / 128f, 255 / 255f), editInterfaceProperties);
            trackInfo.Add(Arrow.DefinitionName, arrowData);

            //Doppler
            ShowPropSubActionFactoryData dopplerData = new ShowPropSubActionFactoryData();
            dopplerData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            dopplerData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            trackInfo.Add(Doppler.DefinitionName, dopplerData);

            //PointingHandLeft
            ShowPropSubActionFactoryData pointingHandLeftData = new ShowPropSubActionFactoryData();
            pointingHandLeftData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            pointingHandLeftData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            pointingHandLeftData.addTrack(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), editInterfaceProperties);
            trackInfo.Add(PointingHand.LeftHandName, pointingHandLeftData);

            //PointingHandRight
            ShowPropSubActionFactoryData pointingRightHandData = new ShowPropSubActionFactoryData();
            pointingRightHandData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            pointingRightHandData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            pointingRightHandData.addTrack(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), editInterfaceProperties);
            trackInfo.Add(PointingHand.RightHandName, pointingRightHandData);

            //Ruler
            ShowPropSubActionFactoryData rulerData = new ShowPropSubActionFactoryData();
            rulerData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            rulerData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            trackInfo.Add(Ruler.DefinitionName, rulerData);

            //Syringe
            ShowPropSubActionFactoryData syringeData = new ShowPropSubActionFactoryData();
            syringeData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            syringeData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            syringeData.addTrack(typeof(PushPlungerAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), pushPlungerProperties);
            trackInfo.Add(Syringe.DefinitionName, syringeData);

            //Circular Highlight
            ShowPropSubActionFactoryData circularHighlightData = new ShowPropSubActionFactoryData();
            circularHighlightData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            circularHighlightData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            circularHighlightData.addTrack(typeof(ChangeCircularHighlightSettings), new Color(128 / 255f, 0 / 255f, 255 / 255f), editInterfaceProperties);
            trackInfo.Add(CircularHighlight.DefinitionName, circularHighlightData);

            //Poseable Hand Left
            ShowPropSubActionFactoryData poseableHandLeftData = new ShowPropSubActionFactoryData();
            poseableHandLeftData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            poseableHandLeftData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            poseableHandLeftData.addTrack(typeof(ChangeHandPosition), new Color(128 / 255f, 0 / 255f, 255 / 255f), leftPoseableHandProperties);
            poseableHandLeftData.addTrack(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 128f, 255 / 255f), editInterfaceProperties);
            trackInfo.Add(PoseableHand.LeftDefinitionName, poseableHandLeftData);

            //Poseable Hand Right
            ShowPropSubActionFactoryData poseableHandRightData = new ShowPropSubActionFactoryData();
            poseableHandRightData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            poseableHandRightData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            poseableHandRightData.addTrack(typeof(ChangeHandPosition), new Color(128 / 255f, 0 / 255f, 255 / 255f), rightPoseableHandProperties);
            poseableHandRightData.addTrack(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 128f, 255 / 255f), editInterfaceProperties);
            trackInfo.Add(PoseableHand.RightDefinitionName, poseableHandRightData);

            //Bite Stick
            ShowPropSubActionFactoryData biteStickData = new ShowPropSubActionFactoryData();
            biteStickData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            biteStickData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            trackInfo.Add(BiteStick.DefinitionName, biteStickData);

            //Range of MotionScale
            ShowPropSubActionFactoryData rangeOfMotionScale = new ShowPropSubActionFactoryData();
            rangeOfMotionScale.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            rangeOfMotionScale.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            trackInfo.Add(RangeOfMotionScale.DefinitionName, rangeOfMotionScale);

            //Pen
            ShowPropSubActionFactoryData penData = new ShowPropSubActionFactoryData();
            penData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            penData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            penData.addTrack(typeof(ClickPenAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), editInterfaceProperties);
            trackInfo.Add(Pen.DefinitionName, penData);

            //Caliper
            ShowPropSubActionFactoryData caliperData = new ShowPropSubActionFactoryData();
            caliperData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            caliperData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            caliperData.addTrack(typeof(SetCaliperMeasurement), new Color(128 / 255f, 0 / 255f, 255 / 255f), editInterfaceProperties);
            trackInfo.Add(Caliper.DefinitionName, caliperData);

            //Plane
            ShowPropSubActionFactoryData planeData = new ShowPropSubActionFactoryData();
            planeData.addTrack(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), movePropProperties);
            planeData.addTrack(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), editInterfaceProperties);
            planeData.addTrack(typeof(ChangePlaneSettings), new Color(128 / 255f, 0 / 255f, 255 / 255f), editInterfaceProperties);
            trackInfo.Add(Plane.DefinitionName, planeData);
        }

        public void Dispose()
        {
            movePropProperties.Dispose();
            editInterfaceProperties.Dispose();
            pushPlungerProperties.Dispose();
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

        public MovePropProperties MoveProperties
        {
            get
            {
                return movePropProperties;
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
