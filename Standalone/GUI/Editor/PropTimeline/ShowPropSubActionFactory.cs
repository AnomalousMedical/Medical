﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowPropSubActionFactory : IDisposable
    {
        Dictionary<String, ShowPropTimelineInfo> trackInfo = new Dictionary<string, ShowPropTimelineInfo>();

        private Dictionary<ShowPropSubAction, PropTimelineData> actionDataBindings = new Dictionary<ShowPropSubAction, PropTimelineData>();

        public ShowPropSubActionFactory(PropEditController propEditController)
        {
            MovePropPrototype movePropPrototype = new MovePropPrototype(typeof(MovePropAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), "Move", propEditController);
            ShowPropSubActionPrototype transparencyPrototype = new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), new Color(128f / 255f, 200f / 255f, 25f / 255f), "Set Transparency");

            //Arrow
            ShowPropTimelineInfo arrowData = new ShowPropTimelineInfo();
            arrowData.addTrack(movePropPrototype);
            arrowData.addTrack(transparencyPrototype);
            arrowData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeArrowColorAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), "Change Color"));
            arrowData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeArrowShapeAction), new Color(128 / 255f, 0 / 128f, 255 / 255f), "Change Arrow Shape"));
            trackInfo.Add(Arrow.DefinitionName, arrowData);

            //Doppler
            ShowPropTimelineInfo dopplerData = new ShowPropTimelineInfo();
            dopplerData.addTrack(movePropPrototype);
            dopplerData.addTrack(transparencyPrototype);
            trackInfo.Add(Doppler.DefinitionName, dopplerData);

            //PointingHandLeft
            ShowPropTimelineInfo pointingHandLeftData = new ShowPropTimelineInfo();
            pointingHandLeftData.addTrack(movePropPrototype);
            pointingHandLeftData.addTrack(transparencyPrototype);
            pointingHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), "Attach To Object"));
            trackInfo.Add(PointingHand.LeftHandName, pointingHandLeftData);

            //PointingHandRight
            ShowPropTimelineInfo pointingRightHandData = new ShowPropTimelineInfo();
            pointingRightHandData.addTrack(movePropPrototype);
            pointingRightHandData.addTrack(transparencyPrototype);
            pointingRightHandData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), "Attach To Object"));
            trackInfo.Add(PointingHand.RightHandName, pointingRightHandData);

            //Ruler
            ShowPropTimelineInfo rulerData = new ShowPropTimelineInfo();
            rulerData.addTrack(movePropPrototype);
            rulerData.addTrack(transparencyPrototype);
            trackInfo.Add(Ruler.DefinitionName, rulerData);

            //Syringe
            ShowPropTimelineInfo syringeData = new ShowPropTimelineInfo();
            syringeData.addTrack(movePropPrototype);
            syringeData.addTrack(transparencyPrototype);
            syringeData.addTrack(new ShowPropSubActionPrototype(typeof(PushPlungerAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), "Push Plunger"));
            trackInfo.Add(Syringe.DefinitionName, syringeData);

            //Circular Highlight
            ShowPropTimelineInfo circularHighlightData = new ShowPropTimelineInfo();
            circularHighlightData.addTrack(movePropPrototype);
            circularHighlightData.addTrack(transparencyPrototype);
            circularHighlightData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeCircularHighlightSettings), new Color(128 / 255f, 0 / 255f, 255 / 255f), "Settings"));
            trackInfo.Add(CircularHighlight.DefinitionName, circularHighlightData);

            //Poseable Hand Left
            ShowPropTimelineInfo poseableHandLeftData = new ShowPropTimelineInfo();
            poseableHandLeftData.addTrack(movePropPrototype);
            poseableHandLeftData.addTrack(transparencyPrototype);
            poseableHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeHandPosition), new Color(128 / 255f, 0 / 255f, 255 / 255f), "Hand Position"));
            poseableHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 128f, 255 / 255f), "Attach To Object"));
            trackInfo.Add(PoseableHand.LeftDefinitionName, poseableHandLeftData);

            //Poseable Hand Right
            ShowPropTimelineInfo poseableHandRightData = new ShowPropTimelineInfo();
            poseableHandRightData.addTrack(movePropPrototype);
            poseableHandRightData.addTrack(transparencyPrototype);
            poseableHandRightData.addTrack(new ShowPropSubActionPrototype(typeof(ChangeHandPosition), new Color(128 / 255f, 0 / 255f, 255 / 255f), "Hand Position"));
            poseableHandRightData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), new Color(128 / 255f, 0 / 128f, 255 / 255f), "Attach To Object"));
            trackInfo.Add(PoseableHand.RightDefinitionName, poseableHandRightData);

            //Bite Stick
            ShowPropTimelineInfo biteStickData = new ShowPropTimelineInfo();
            biteStickData.addTrack(movePropPrototype);
            biteStickData.addTrack(transparencyPrototype);
            trackInfo.Add(BiteStick.DefinitionName, biteStickData);

            //Range of MotionScale
            ShowPropTimelineInfo rangeOfMotionScale = new ShowPropTimelineInfo();
            rangeOfMotionScale.addTrack(movePropPrototype);
            rangeOfMotionScale.addTrack(transparencyPrototype);
            trackInfo.Add(RangeOfMotionScale.DefinitionName, rangeOfMotionScale);

            //Pen
            ShowPropTimelineInfo penData = new ShowPropTimelineInfo();
            penData.addTrack(movePropPrototype);
            penData.addTrack(transparencyPrototype);
            penData.addTrack(new ShowPropSubActionPrototype(typeof(ClickPenAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), "Click Pen"));
            trackInfo.Add(Pen.DefinitionName, penData);

            //Caliper
            ShowPropTimelineInfo caliperData = new ShowPropTimelineInfo();
            caliperData.addTrack(movePropPrototype);
            caliperData.addTrack(transparencyPrototype);
            caliperData.addTrack(new ShowPropSubActionPrototype(typeof(SetCaliperMeasurement), new Color(128 / 255f, 0 / 255f, 255 / 255f), "Set Measurement"));
            trackInfo.Add(Caliper.DefinitionName, caliperData);

            //Plane
            ShowPropTimelineInfo planeData = new ShowPropTimelineInfo();
            planeData.addTrack(movePropPrototype);
            planeData.addTrack(transparencyPrototype);
            planeData.addTrack(new ShowPropSubActionPrototype(typeof(ChangePlaneSettings), new Color(128 / 255f, 0 / 255f, 255 / 255f), "Settings"));
            trackInfo.Add(Plane.DefinitionName, planeData);
        }

        public void Dispose()
        {
            clearData();
        }

        public void addTracksForAction(ShowPropAction showProp, TimelineView timelineView)
        {
            ShowPropTimelineInfo track;
            if(trackInfo.TryGetValue(showProp.PropType, out track))
            {
                track.addTracksToTimeline(timelineView);
            }
        }

        public ShowPropSubAction createSubAction(ShowPropAction showProp, String name)
        {
            return trackInfo[showProp.PropType].createSubAction(name);
        }

        public PropTimelineData createData(ShowPropAction showProp, ShowPropSubAction subAction)
        {
            PropTimelineData timelineData = trackInfo[showProp.PropType].createData(subAction);
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