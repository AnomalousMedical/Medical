using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Saving;
using Engine.Platform;
using Medical.GUI.AnomalousMvc;

namespace Medical.GUI
{
    public class PropTimeline : LayoutComponent
    {
        private TimelineDataProperties actionProperties;
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private ShowPropSubActionFactory actionFactory;
        private ShowPropAction propData;
        private Dictionary<ShowPropSubAction, PropTimelineData> actionDataBindings = new Dictionary<ShowPropSubAction, PropTimelineData>();

        private ShowMenuButton editMenuButton;
        private PopupMenu editMenu;
        private SaveableClipboard clipboard;
        private PropEditController propEditController;

        public PropTimeline(SaveableClipboard clipboard, PropEditController propEditController, MyGUIViewHost viewHost)
            :base("Medical.GUI.PropTimeline.PropTimeline.layout", viewHost)
        {
            this.clipboard = clipboard;
            this.propEditController = propEditController;
            propEditController.ShowPropActionChanged += propEditController_ShowPropActionChanged;
            propEditController.DurationChanged += propEditController_DurationChanged;

            widget.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);

            //Timeline view
            ScrollView timelineViewScrollView = widget.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.Duration = 5.0f;
            timelineView.KeyReleased += new EventHandler<KeyEventArgs>(timelineView_KeyReleased);

            //Properties
            ScrollView timelinePropertiesScrollView = widget.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;
            actionFactory = new ShowPropSubActionFactory(timelinePropertiesScrollView, propEditController);

            //Timeline filter
            ScrollView timelineFilterScrollView = widget.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            //Edit button
            Button editButton = widget.findWidget("EditButton") as Button;
            editMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            editMenu.Visible = false;
            MenuItem selectAll = editMenu.addItem("Select All");
            selectAll.MouseButtonClick += new MyGUIEvent(selectAll_MouseButtonClick);
            MenuItem cut = editMenu.addItem("Cut");
            cut.MouseButtonClick += new MyGUIEvent(cut_MouseButtonClick);
            MenuItem copy = editMenu.addItem("Copy");
            copy.MouseButtonClick += new MyGUIEvent(copy_MouseButtonClick);
            MenuItem paste = editMenu.addItem("Paste");
            paste.MouseButtonClick += new MyGUIEvent(paste_MouseButtonClick);
            editMenuButton = new ShowMenuButton(editButton, editMenu);

            numberLine = new NumberLine(widget.findWidget("NumberLine") as ScrollView, timelineView);

            Button removeAction = widget.findWidget("RemoveAction") as Button;
            removeAction.MouseButtonClick += new MyGUIEvent(removeAction_MouseButtonClick);

            setPropData(propEditController.CurrentShowPropAction);
        }

        public override void Dispose()
        {
            actionFactory.Dispose();
            propEditController.ShowPropActionChanged -= propEditController_ShowPropActionChanged;
            propEditController.DurationChanged -= propEditController_DurationChanged;
            Gui.Instance.destroyWidget(editMenu);
            timelineView.Dispose();
            base.Dispose();
        }

        public void setPropData(ShowPropAction showProp)
        {
            if (propData != null)
            {
                propData.Updated -= propData_Updated;
            }
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

        public float MarkerTime
        {
            get
            {
                return timelineView.MarkerTime;
            }
            set
            {
                if (propData != null)
                {
                    timelineView.MarkerTime = value - propData.StartTime;
                }
                else
                {
                    timelineView.MarkerTime = value;
                }
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
                if (timelineView.CurrentData != null && timelineView.CurrentData.Track == "Move")
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
            PropTimelineClipboardContainer clipContainer = new PropTimelineClipboardContainer();
            clipContainer.addActions(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
            removeSelectedData();
            editMenu.setVisibleSmooth(false);
        }

        void paste_MouseButtonClick(Widget source, EventArgs e)
        {
            PropTimelineClipboardContainer clipContainer = clipboard.createCopy<PropTimelineClipboardContainer>();
            if (clipContainer != null)
            {
                clipContainer.addActionsToTimeline(propData, this, timelineView.MarkerTime, timelineView.Duration);
            }
            editMenu.setVisibleSmooth(false);
        }

        void copy_MouseButtonClick(Widget source, EventArgs e)
        {
            PropTimelineClipboardContainer clipContainer = new PropTimelineClipboardContainer();
            clipContainer.addActions(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
            editMenu.setVisibleSmooth(false);
        }

        void selectAll_MouseButtonClick(Widget source, EventArgs e)
        {
            timelineView.selectAll();
            editMenu.setVisibleSmooth(false);
        }

        #endregion Edit Menu

        void propEditController_DurationChanged(float duration)
        {
            Duration = duration;
        }

        void propEditController_ShowPropActionChanged(ShowPropAction obj)
        {
            setPropData(obj);
        }
    }
}
