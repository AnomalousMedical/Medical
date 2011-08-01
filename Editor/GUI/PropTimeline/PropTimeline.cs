using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Saving;
using Engine.Platform;

namespace Medical.GUI
{
    public class PropTimeline : MDIDialog
    {
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
        private PropTimelineClipboard propClipboard = new PropTimelineClipboard();

        public PropTimeline()
            :base("Medical.GUI.PropTimeline.PropTimeline.layout")
        {
            window.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);

            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.Duration = 5.0f;
            timelineView.ActiveDataChanged += new EventHandler(timelineView_ActiveDataChanged);
            timelineView.KeyReleased += new EventHandler<KeyEventArgs>(timelineView_KeyReleased);

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
            MenuItem cut = editMenu.addItem("Cut");
            cut.MouseButtonClick += new MyGUIEvent(cut_MouseButtonClick);
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
            if (propData != null)
            {
                propData.Updated -= propData_Updated;
            }
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
            if (propData != null)
            {
                propData.Updated += propData_Updated;
            }
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

        void propData_Updated(float time)
        {
            timelineView.MarkerTime = time;
        }

        void trackFilter_AddTrackItem(string name)
        {
            ShowPropSubAction subAction = actionFactory.createSubAction(propData, name);
            subAction.StartTime = timelineView.MarkerTime;
            if (subAction is MovePropAction)
            {
                //This is a bit hacky, but if the action is a MovePropAction grab the 
                //current location and set that for the move position. 
                //This makes editing easier.
                MovePropAction moveProp = (MovePropAction)subAction;
                if (usingTools)
                {
                    moveProp.Translation = Translation;
                    moveProp.Rotation = Rotation;
                }
                else
                {
                    moveProp.Translation = propData.Translation;
                    moveProp.Rotation = propData.Rotation;
                }
            }
            propData.addSubAction(subAction);
            addSubActionData(subAction);
        }

        internal void addSubActionData(ShowPropSubAction subAction)
        {
            PropTimelineData timelineData = new PropTimelineData(subAction);
            timelineView.addData(timelineData);
            actionDataBindings.Add(subAction, timelineData);
        }

        void removeAction_MouseButtonClick(Widget source, EventArgs e)
        {
            removeCurrentData();
        }

        private void removeCurrentData()
        {
            PropTimelineData propTlData = (PropTimelineData)timelineView.CurrentData;
            propData.removeSubAction(propTlData.Action);
            timelineView.removeData(propTlData);
        }

        private void removeSelectedData()
        {
            foreach (PropTimelineData propTlData in timelineView.SelectedData)
            {
                propData.removeSubAction(propTlData.Action);
                timelineView.removeData(propTlData);
            }
        }

        void timelineView_ActiveDataChanged(object sender, EventArgs e)
        {
            if (timelineView.CurrentData != null)
            {
                bool wasUsingTools = usingTools;
                usingTools = timelineView.CurrentData.Track == "Move";
                if (!usingTools && wasUsingTools)
                {
                    propData._movePreviewProp(propData.Translation, propData.Rotation);
                }
            }
        }

        void window_KeyButtonReleased(Widget source, EventArgs e)
        {
            processKeys((KeyEventArgs)e);
        }

        void timelineView_KeyReleased(object sender, KeyEventArgs e)
        {
            processKeys(e);
        }

        private void processKeys(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case KeyboardButtonCode.KC_DELETE:
                    removeSelectedData();
                    break;
            }
        }

        #region Edit Menu

        void cut_MouseButtonClick(Widget source, EventArgs e)
        {
            propClipboard.copy(timelineView.SelectedData);
            removeSelectedData();
            editMenu.setVisibleSmooth(false);
        }

        void paste_MouseButtonClick(Widget source, EventArgs e)
        {
            propClipboard.paste(propData, this, timelineView.MarkerTime, timelineView.Duration);
            editMenu.setVisibleSmooth(false);
        }

        void copy_MouseButtonClick(Widget source, EventArgs e)
        {
            propClipboard.copy(timelineView.SelectedData);
            editMenu.setVisibleSmooth(false);
        }

        #endregion Edit Menu
    }
}
