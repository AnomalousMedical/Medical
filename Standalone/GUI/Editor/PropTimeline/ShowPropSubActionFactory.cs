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
        Dictionary<String, ShowPropTrackInfo> trackInfo = new Dictionary<string, ShowPropTrackInfo>();

        private Dictionary<ShowPropSubAction, PropTimelineData> actionDataBindings = new Dictionary<ShowPropSubAction, PropTimelineData>();

        public ShowPropSubActionFactory()
        {
            ShowPropSubActionPrototype movePropPrototype = new ShowPropSubActionPrototype(typeof(MovePropAction), "Move");
            ShowPropSubActionPrototype transparencyPrototype = new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), "Set Transparency");

            //Arrow
            ShowPropTrackInfo arrowData = new ShowPropTrackInfo();
            arrowData.addTrack(movePropPrototype);
            arrowData.addTrack(transparencyPrototype);
            arrowData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeArrowColorAction), "Change Color"));
            arrowData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeArrowShapeAction), "Change Arrow Shape"));
            trackInfo.Add(Arrow.DefinitionName, arrowData);

            //Doppler
            ShowPropTrackInfo dopplerData = new ShowPropTrackInfo();
            dopplerData.addTrack(movePropPrototype);
            dopplerData.addTrack(transparencyPrototype);
            trackInfo.Add(Doppler.DefinitionName, dopplerData);

            //PointingHandLeft
            ShowPropTrackInfo pointingHandLeftData = new ShowPropTrackInfo();
            pointingHandLeftData.addTrack(movePropPrototype);
            pointingHandLeftData.addTrack(transparencyPrototype);
            pointingHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), "Attach To Object"));
            trackInfo.Add(PointingHand.LeftHandName, pointingHandLeftData);

            //PointingHandRight
            ShowPropTrackInfo pointingRightHandData = new ShowPropTrackInfo();
            pointingRightHandData.addTrack(movePropPrototype);
            pointingRightHandData.addTrack(transparencyPrototype);
            pointingRightHandData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), "Attach To Object"));
            trackInfo.Add(PointingHand.RightHandName, pointingRightHandData);

            //Ruler
            ShowPropTrackInfo rulerData = new ShowPropTrackInfo();
            rulerData.addTrack(movePropPrototype);
            rulerData.addTrack(transparencyPrototype);
            trackInfo.Add(Ruler.DefinitionName, rulerData);

            //Syringe
            ShowPropTrackInfo syringeData = new ShowPropTrackInfo();
            syringeData.addTrack(movePropPrototype);
            syringeData.addTrack(transparencyPrototype);
            syringeData.addTrack(new ShowPropSubActionPrototype(typeof(PushPlungerAction), "Push Plunger"));
            trackInfo.Add(Syringe.DefinitionName, syringeData);

            //Circular Highlight
            ShowPropTrackInfo circularHighlightData = new ShowPropTrackInfo();
            circularHighlightData.addTrack(movePropPrototype);
            circularHighlightData.addTrack(transparencyPrototype);
            circularHighlightData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeCircularHighlightSettings), "Settings"));
            trackInfo.Add(CircularHighlight.DefinitionName, circularHighlightData);

            //Poseable Hand Left
            ShowPropTrackInfo poseableHandLeftData = new ShowPropTrackInfo();
            poseableHandLeftData.addTrack(movePropPrototype);
            poseableHandLeftData.addTrack(transparencyPrototype);
            poseableHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeHandPosition), "Hand Position"));
            poseableHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), "Attach To Object"));
            trackInfo.Add(PoseableHand.LeftDefinitionName, poseableHandLeftData);

            //Poseable Hand Right
            ShowPropTrackInfo poseableHandRightData = new ShowPropTrackInfo();
            poseableHandRightData.addTrack(movePropPrototype);
            poseableHandRightData.addTrack(transparencyPrototype);
            poseableHandRightData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeHandPosition), "Hand Position"));
            poseableHandRightData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), "Attach To Object"));
            trackInfo.Add(PoseableHand.RightDefinitionName, poseableHandRightData);

            //Bite Stick
            ShowPropTrackInfo biteStickData = new ShowPropTrackInfo();
            biteStickData.addTrack(movePropPrototype);
            biteStickData.addTrack(transparencyPrototype);
            trackInfo.Add(BiteStick.DefinitionName, biteStickData);

            //Range of MotionScale
            ShowPropTrackInfo rangeOfMotionScale = new ShowPropTrackInfo();
            rangeOfMotionScale.addTrack(movePropPrototype);
            rangeOfMotionScale.addTrack(transparencyPrototype);
            trackInfo.Add(RangeOfMotionScale.DefinitionName, rangeOfMotionScale);

            //Pen
            ShowPropTrackInfo penData = new ShowPropTrackInfo();
            penData.addTrack(movePropPrototype);
            penData.addTrack(transparencyPrototype);
            penData.addTrack(new ShowPropSubActionPrototype(typeof(ClickPenAction), "Click Pen"));
            trackInfo.Add(Pen.DefinitionName, penData);

            //Caliper
            ShowPropTrackInfo caliperData = new ShowPropTrackInfo();
            caliperData.addTrack(movePropPrototype);
            caliperData.addTrack(transparencyPrototype);
            caliperData.addTrack(new ShowPropSubActionPrototype(typeof(SetCaliperMeasurement), "Set Measurement"));
            trackInfo.Add(Caliper.DefinitionName, caliperData);

            //Plane
            ShowPropTrackInfo planeData = new ShowPropTrackInfo();
            planeData.addTrack(movePropPrototype);
            planeData.addTrack(transparencyPrototype);
            planeData.addTrack(new ShowPropSubActionPrototype(typeof(ChangePlaneSettings), "Settings"));
            trackInfo.Add(Plane.DefinitionName, planeData);

            //Line
            ShowPropTrackInfo lineData = new ShowPropTrackInfo();
            lineData.addTrack(movePropPrototype);
            lineData.addTrack(transparencyPrototype);
            lineData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeLinePropSettings), "Settings"));
            trackInfo.Add(LineProp.DefinitionName, lineData);
        }

        public void Dispose()
        {
            clearData();
        }

        public void addTracksForAction(ShowPropAction showProp, TimelineView timelineView)
        {
            ShowPropTrackInfo propTrackInfo;
            if(trackInfo.TryGetValue(showProp.PropType, out propTrackInfo))
            {
                foreach (ShowPropSubActionPrototype data in propTrackInfo.Tracks)
                {
                    timelineView.addTrack(data.TypeName);
                }
            }
        }

        public ShowPropSubAction createSubAction(ShowPropAction showProp, String name)
        {
            return trackInfo[showProp.PropType].createSubAction(name);
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
