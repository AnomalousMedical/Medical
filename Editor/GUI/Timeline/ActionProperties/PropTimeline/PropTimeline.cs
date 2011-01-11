using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PropTimeline : Dialog
    {
        private TimelineDataProperties actionProperties;
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;

        public PropTimeline()
            :base("Medical.GUI.Timeline.ActionProperties.PropTimeline.PropTimeline.layout")
        {
            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.Duration = 5.0f;

            //Properties
            ScrollView timelinePropertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;
            actionProperties.addPanel("Muscle Position", new MovementKeyframeProperties(timelinePropertiesScrollView));

            //Timeline filter
            ScrollView timelineFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);

            //Add tracks to timeline.
        }

        void trackFilter_AddTrackItem(string name)
        {
            //if (movementSequenceController.CurrentSequence == null)
            //{
            //    createNewSequence();
            //}
            //MovementSequenceState state = new MovementSequenceState();
            //state.captureState();
            //state.StartTime = timelineView.MarkerTime;
            //movementSequenceController.CurrentSequence.addState(state);
            //timelineView.addData(new MovementKeyframeData(state, movementSequenceController.CurrentSequence));
        }
    }
}
