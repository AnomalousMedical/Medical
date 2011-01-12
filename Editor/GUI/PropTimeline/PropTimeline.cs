using System;
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
        private ShowPropSubActionFactory actionFactory;
        private ShowPropAction propData;
        private Dictionary<ShowPropSubAction, PropTimelineData> actionDataBindings = new Dictionary<ShowPropSubAction, PropTimelineData>();
        private bool usingTools = false;

        public PropTimeline()
            :base("Medical.GUI.PropTimeline.PropTimeline.layout")
        {
            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.Duration = 5.0f;
            timelineView.ActiveDataChanged += new EventHandler(timelineView_ActiveDataChanged);

            //Properties
            ScrollView timelinePropertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;
            actionFactory = new ShowPropSubActionFactory(timelinePropertiesScrollView);

            //Timeline filter
            ScrollView timelineFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);

            Button removeAction = window.findWidget("RemoveAction") as Button;
            removeAction.MouseButtonClick += new MyGUIEvent(removeAction_MouseButtonClick);
        }

        public void setPropData(ShowPropAction showProp)
        {
            timelineView.clearTracks();
            actionDataBindings.Clear();
            actionProperties.clearPanels();
            if (showProp != null)
            {
                actionFactory.addTracksForAction(showProp, timelineView, actionProperties);
                foreach (ShowPropSubAction action in showProp.SubActions)
                {
                    addSubActionData(action);
                }
                timelineView.Duration = showProp.Duration;
            }
            else
            {
                timelineView.Duration = 0.0f;
            }
            this.propData = showProp;
        }

        public float Duration
        {
            get
            {
                return timelineView.Duration;
            }
            set
            {
                timelineView.Duration = value;
            }
        }

        /// <summary>
        /// This will be true if the timeline is needing to use the move tool.
        /// </summary>
        public bool UsingTools
        {
            get
            {
                return usingTools;
            }
        }

        public Vector3 Translation
        {
            get
            {
                return actionFactory.MoveProperties.Translation;
            }
            set
            {
                actionFactory.MoveProperties.Translation = value;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return actionFactory.MoveProperties.Rotation;
            }
            set
            {
                actionFactory.MoveProperties.Rotation = value;
            }
        }

        void trackFilter_AddTrackItem(string name)
        {
            ShowPropSubAction subAction = actionFactory.createSubAction(propData, name);
            subAction.StartTime = timelineView.MarkerTime;
            propData.addSubAction(subAction);
            addSubActionData(subAction);
        }

        private void addSubActionData(ShowPropSubAction subAction)
        {
            PropTimelineData timelineData = new PropTimelineData(subAction);
            timelineView.addData(timelineData);
            actionDataBindings.Add(subAction, timelineData);
        }

        void removeAction_MouseButtonClick(Widget source, EventArgs e)
        {
            PropTimelineData propTlData = (PropTimelineData)timelineView.CurrentData;
            propData.removeSubAction(propTlData.Action);
            timelineView.removeData(propTlData);
        }

        void timelineView_ActiveDataChanged(object sender, EventArgs e)
        {
            if (timelineView.CurrentData != null)
            {
                usingTools = timelineView.CurrentData.Track == "Move";
            }
        }
    }
}
