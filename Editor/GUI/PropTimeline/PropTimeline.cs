﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class PropTimeline : Dialog
    {
        private TimelineDataProperties actionProperties;
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private ShowPropSubActionFactory actionFactory = new ShowPropSubActionFactory();
        private ShowPropAction propData;
        private Dictionary<ShowPropSubAction, PropTimelineData> actionDataBindings = new Dictionary<ShowPropSubAction, PropTimelineData>();

        public PropTimeline()
            :base("Medical.GUI.PropTimeline.PropTimeline.layout")
        {
            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.Duration = 5.0f;

            //Properties
            ScrollView timelinePropertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;
            //actionProperties.addPanel("Muscle Position", new MovementKeyframeProperties(timelinePropertiesScrollView));

            //Timeline filter
            ScrollView timelineFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);
        }

        public void setPropData(ShowPropAction showProp)
        {
            timelineView.clearTracks();
            actionDataBindings.Clear();
            if (showProp != null)
            {
                actionFactory.addTracksForAction(showProp, timelineView);
                foreach (ShowPropSubAction action in showProp.SubActions)
                {
                    addSubActionData(action);
                }
            }
            this.propData = showProp;
        }

        void trackFilter_AddTrackItem(string name)
        {
            ShowPropSubAction subAction = actionFactory.createSubAction(propData, name);
            propData.addSubAction(subAction);
            addSubActionData(subAction);
        }

        private void addSubActionData(ShowPropSubAction subAction)
        {
            PropTimelineData timelineData = new PropTimelineData(subAction);
            timelineView.addData(timelineData);
            actionDataBindings.Add(subAction, timelineData);
        }
    }
}
