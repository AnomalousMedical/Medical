using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Saving;
using Engine.Platform;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI
{
    public class PropTimeline : LayoutComponent, EditMenuProvider
    {
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private ShowPropSubActionFactory actionFactory;
        private ShowPropAction propData;

        private SaveableClipboard clipboard;
        private PropEditController propEditController;
        private PropTimelineData propTimelineData = null;

        public PropTimeline(SaveableClipboard clipboard, PropEditController propEditController, MyGUIViewHost viewHost)
            :base("Medical.GUI.PropTimeline.PropTimeline.layout", viewHost)
        {
            this.clipboard = clipboard;
            this.propEditController = propEditController;
            propEditController.ShowPropActionChanged += propEditController_ShowPropActionChanged;
            propEditController.DurationChanged += propEditController_DurationChanged;
            propEditController.MarkerMoved += propEditController_MarkerMoved;

            widget.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);
            widget.RootKeyChangeFocus += new MyGUIEvent(widget_RootKeyChangeFocus);

            //Timeline view
            ScrollView timelineViewScrollView = widget.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.Duration = 5.0f;
            timelineView.KeyReleased += new EventHandler<KeyEventArgs>(timelineView_KeyReleased);
            timelineView.ActiveDataChanged += new EventHandler(timelineView_ActiveDataChanged);

            //Properties
            actionFactory = new ShowPropSubActionFactory(propEditController);

            //Timeline filter
            ScrollView timelineFilterScrollView = widget.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            numberLine = new NumberLine(widget.findWidget("NumberLine") as ScrollView, timelineView);

            setPropData(propEditController.CurrentShowPropAction);

            ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
        }

        public override void Dispose()
        {
            ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).removeMenuProvider(this);
            actionFactory.Dispose();
            propEditController.MarkerMoved -= propEditController_MarkerMoved;
            propEditController.ShowPropActionChanged -= propEditController_ShowPropActionChanged;
            propEditController.DurationChanged -= propEditController_DurationChanged;
            timelineView.Dispose();
            base.Dispose();
        }

        public void setPropData(ShowPropAction showProp)
        {
            if (propData != null)
            {
                propData.Updated -= propData_Updated;
                propData.ActionAdded -= propData_ActionAdded;
                propData.ActionRemoved -= propData_ActionRemoved;
            }
            timelineView.clearTracks();
            actionFactory.clearData();
            if (showProp != null)
            {
                actionFactory.addTracksForAction(showProp, timelineView);
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
                propData.ActionAdded += propData_ActionAdded;
                propData.ActionRemoved += propData_ActionRemoved;
            }
            MarkerTime = propEditController.MarkerPosition;
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

        void propData_Updated(float time)
        {
            timelineView.MarkerTime = time;
        }

        void propData_ActionRemoved(ShowPropAction showProp, ShowPropSubAction subAction)
        {
            timelineView.removeData(actionFactory[subAction]);
            actionFactory.destroyData(subAction);
        }

        void propData_ActionAdded(ShowPropAction showProp, ShowPropSubAction subAction)
        {
            addSubActionData(subAction);
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
                    MovePropAction currentMoveProp = (MovePropAction)((PropTimelineData)timelineView.CurrentData).Action;
                    moveProp.Translation = currentMoveProp.Translation;
                    moveProp.Rotation = currentMoveProp.Rotation;
                }
                else
                {
                    moveProp.Translation = propData.Translation;
                    moveProp.Rotation = propData.Rotation;
                }
            }
            propData.addSubAction(subAction);
        }

        private void addSubActionData(ShowPropSubAction subAction)
        {
            timelineView.addData(actionFactory.createData(subAction));
        }

        private void removeCurrentData()
        {
            PropTimelineData propTlData = (PropTimelineData)timelineView.CurrentData;
            propData.removeSubAction(propTlData.Action);
        }

        private void removeSelectedData()
        {
            foreach (PropTimelineData propTlData in timelineView.SelectedData)
            {
                propData.removeSubAction(propTlData.Action);
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

        public void cut()
        {
            PropTimelineClipboardContainer clipContainer = new PropTimelineClipboardContainer();
            clipContainer.addActions(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
            removeSelectedData();
        }

        public void paste()
        {
            PropTimelineClipboardContainer clipContainer = clipboard.createCopy<PropTimelineClipboardContainer>();
            if (clipContainer != null)
            {
                clipContainer.addActionsToTimeline(propData, this, timelineView.MarkerTime, timelineView.Duration);
            }
        }

        public void copy()
        {
            PropTimelineClipboardContainer clipContainer = new PropTimelineClipboardContainer();
            clipContainer.addActions(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
        }

        public void selectAll()
        {
            timelineView.selectAll();
        }

        void propEditController_DurationChanged(float duration)
        {
            Duration = duration;
        }

        void propEditController_ShowPropActionChanged(ShowPropAction obj)
        {
            setPropData(obj);
        }

        void propEditController_MarkerMoved(float obj)
        {
            MarkerTime = obj;
        }

        void widget_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs rfea = (RootFocusEventArgs)e;
            if (rfea.Focus)
            {
                ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
            }
        }

        void timelineView_ActiveDataChanged(object sender, EventArgs e)
        {
            if (propTimelineData != null)
            {
                propTimelineData.editingCompleted();
            }
            propTimelineData = (PropTimelineData)timelineView.CurrentData;
            if (propTimelineData != null)
            {
                propTimelineData.editingStarted();
            }

            EditInterfaceHandler editInterfaceHandler = ViewHost.Context.getModel<EditInterfaceHandler>(EditInterfaceHandler.DefaultName);
            if (editInterfaceHandler != null)
            {
                if (propTimelineData != null)
                {
                    editInterfaceHandler.changeEditInterface(propTimelineData.Action.EditInterface);
                }
                else if (propData != null)
                {
                    editInterfaceHandler.changeEditInterface(propData.getEditInterface());
                }
            }
        }
    }
}
