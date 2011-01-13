using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Saving;

namespace Medical.GUI
{
    public class PropTimeline : Dialog
    {
        public event EventHandler UpdatePropPreview
        {
            add
            {
                actionFactory.MoveProperties.UpdatePropPreview += value;
            }
            remove
            {
                actionFactory.MoveProperties.UpdatePropPreview -= value;
            }
        }

        private TimelineDataProperties actionProperties;
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private ShowPropSubActionFactory actionFactory;
        private ShowPropAction propData;
        private Dictionary<ShowPropSubAction, PropTimelineData> actionDataBindings = new Dictionary<ShowPropSubAction, PropTimelineData>();
        private bool usingTools = false;

        private ShowMenuButton editMenuButton;
        private PopupMenu editMenu;
        private ShowPropSubAction copySourceAction;
        private CopySaver copySaver = new CopySaver();

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

            //Edit button
            Button editButton = window.findWidget("EditButton") as Button;
            editMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            editMenu.Visible = false;
            MenuItem copy = editMenu.addItem("Copy");
            copy.MouseButtonClick += new MyGUIEvent(copy_MouseButtonClick);
            MenuItem paste = editMenu.addItem("Paste");
            paste.MouseButtonClick += new MyGUIEvent(paste_MouseButtonClick);
            editMenuButton = new ShowMenuButton(editButton, editMenu);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);

            Button removeAction = window.findWidget("RemoveAction") as Button;
            removeAction.MouseButtonClick += new MyGUIEvent(removeAction_MouseButtonClick);
        }

        public void setPropData(ShowPropAction showProp)
        {
            usingTools = false;
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
                bool wasUsingTools = usingTools;
                usingTools = timelineView.CurrentData.Track == "Move";
                if (!usingTools && wasUsingTools)
                {
                    actionFactory.MoveProperties.fireUpdatePropPreview();
                }
            }
        }

        #region Edit Menu

        void paste_MouseButtonClick(Widget source, EventArgs e)
        {
            if (copySourceAction != null)
            {
                ShowPropSubAction copiedAction = copySaver.copy<ShowPropSubAction>(copySourceAction);
                copiedAction.StartTime = timelineView.MarkerTime;
                propData.addSubAction(copiedAction);
                addSubActionData(copiedAction);
                timelineView.CurrentData = actionDataBindings[copiedAction];
            }
            editMenu.setVisibleSmooth(false);
        }

        void copy_MouseButtonClick(Widget source, EventArgs e)
        {
            PropTimelineData currentData = (PropTimelineData)timelineView.CurrentData;
            if (currentData != null)
            {
                copySourceAction = copySaver.copy<ShowPropSubAction>(currentData.Action);
            }
        }

        #endregion Edit Menu
    }
}
