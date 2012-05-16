using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Engine;
using Logging;
using Engine.Saving;
using Engine.Saving.XMLSaver;
using System.Xml;
using Engine.Platform;

namespace Medical.GUI
{
    class TimelineProperties : MDIDialog
    {
        private TimelineController timelineController;
        private TimelineDataProperties dataProperties;
        private TrackFilter actionFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Dictionary<TimelineAction, TimelineActionData> actionDataBindings = new Dictionary<TimelineAction, TimelineActionData>();
        private TimelineActionFactory actionFactory;
        private TimelinePropertiesController timelinePropertiesController;
        private SaveableClipboard clipboard;
        private TimelineData editingStoppedLastData;

        public event EventDelegate<TimelineProperties, float> MarkerMoved;

        private Button playButton;
        private Button playFullButton;
        private Button rewindButton;
        private Button fastForwardButton;

        private const int START_COLUMN_WIDTH = 100;

        public TimelineProperties(TimelineController timelineController, EditorPlugin editorPlugin, GUIManager guiManager, TimelinePropertiesController timelinePropertiesController, SaveableClipboard clipboard)
            :base("Medical.GUI.Timeline.TimelineProperties.layout")
        {
            this.clipboard = clipboard;
            this.timelinePropertiesController = timelinePropertiesController;

            this.timelineController = timelineController;
            timelineController.TimelinePlaybackStarted += new EventHandler(timelineController_TimelinePlaybackStarted);
            timelineController.TimelinePlaybackStopped += new EventHandler(timelineController_TimelinePlaybackStopped);
            timelineController.TimeTicked += new TimeTickEvent(timelineController_TimeTicked);

            window.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);

            //Remove action button
            Button removeActionButton = window.findWidget("RemoveAction") as Button;
            removeActionButton.MouseButtonClick += new MyGUIEvent(removeActionButton_MouseButtonClick);

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            playFullButton = window.findWidget("PlayFull") as Button;
            playFullButton.MouseButtonClick += new MyGUIEvent(playFullButton_MouseButtonClick);

            fastForwardButton = window.findWidget("FastForward") as Button;
            fastForwardButton.MouseButtonClick += new MyGUIEvent(fastForwardButton_MouseButtonClick);

            rewindButton = window.findWidget("Rewind") as Button;
            rewindButton.MouseButtonClick += new MyGUIEvent(rewindButton_MouseButtonClick);

            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.KeyReleased += new EventHandler<KeyEventArgs>(timelineView_KeyReleased);
            timelineView.ActiveDataChanging += new EventHandler<CancelEventArgs>(timelineView_ActiveDataChanging);
            timelineView.MarkerMoved += new EventDelegate<TimelineView, float>(timelineView_MarkerMoved);

            //Properties
            ScrollView propertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            dataProperties = new TimelineDataProperties(propertiesScrollView, timelineView);
            dataProperties.Visible = false;

            //Track filter
            ScrollView trackFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            actionFilter = new TrackFilter(trackFilterScrollView, timelineView);
            actionFilter.AddTrackItem += new AddTrackItemCallback(actionFilter_AddTrackItem);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);

            //Add tracks to timeline.
            actionFactory = new TimelineActionFactory(propertiesScrollView, editorPlugin);
            foreach (TimelineActionFactoryData actionProp in actionFactory.ActionProperties)
            {
                timelineView.addTrack(actionProp.TypeName, actionProp.Color);

                if (actionProp.Panel != null)
                {
                    dataProperties.addPanel(actionProp.TypeName, actionProp.Panel);
                }
            }

            Enabled = false;
        }

        public override void Dispose()
        {
            actionFactory.Dispose();
            actionFilter.Dispose();
            timelineView.Dispose();
            base.Dispose();
        }

        public void setCurrentTimeline(Timeline timeline)
        {
            timelineView.removeAllData();
            foreach (TimelineAction action in timeline.Actions)
            {
                addActionToTimeline(action);
            }
            updateWindowCaption();
        }

        public void updateWindowCaption()
        {
            if (timelinePropertiesController.ResourceProvider != null && timelinePropertiesController.CurrentTimelineFile != null)
            {
                window.Caption = String.Format("Timeline - {0}", Path.GetFileName(timelinePropertiesController.CurrentTimelineFile));
            }
            else
            {
                window.Caption = "Timeline";
            }
        }

        public void paste()
        {
            TimelineActionClipboardContainer clipContainer = clipboard.createCopy<TimelineActionClipboardContainer>();
            if (clipContainer != null)
            {
                clipContainer.addActionsToTimeline(timelinePropertiesController.CurrentTimeline, timelineView.MarkerTime);
            }
        }

        public void copy()
        {
            TimelineActionClipboardContainer clipContainer = new TimelineActionClipboardContainer();
            clipContainer.addActions(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
        }

        public void cut()
        {
            copy();
            deleteSelectedActions();
        }

        public void selectAll()
        {
            timelineView.selectAll();
        }

        public void stopEditing()
        {
            editingStoppedLastData = timelineView.CurrentData;
            timelineView.CurrentData = null;
        }

        public float MarkerTime
        {
            get
            {
                return timelineView.MarkerTime;
            }
            set
            {
                timelineView.MarkerTime = value;
            }
        }

        void removeActionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            TimelineActionData data = (TimelineActionData)timelineView.CurrentData;
            if (data != null)
            {
                stopTimelineIfPlaying();
                timelinePropertiesController.CurrentTimeline.removeAction(data.Action);
            }
        }

        void actionFilter_AddTrackItem(string name)
        {
            TimelineAction action = actionFactory.createAction(name);
            action.StartTime = timelineView.MarkerTime;
            timelinePropertiesController.CurrentTimeline.addAction(action);
            action.capture();
            timelineView.CurrentData = actionDataBindings[action];
        }

        public void addActionToTimeline(TimelineAction action)
        {
            TimelineActionData data = new TimelineActionData(action);
            actionDataBindings.Add(action, data);
            timelineView.addData(data);
        }

        public void removeActionFromTimeline(TimelineAction action)
        {
            timelineView.removeData(actionDataBindings[action]);
        }

        public bool Enabled
        {
            get
            {
                return actionFilter.Enabled;
            }
            set
            {
                actionFilter.Enabled = value;
                playButton.Enabled = value;
                playFullButton.Enabled = value;
                timelineView.Enabled = value;
                fastForwardButton.Enabled = value;
                rewindButton.Enabled = value;
                updateWindowCaption();
            }
        }

        void rewindButton_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            timelineView.MarkerTime = 0.0f;
        }

        void fastForwardButton_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            timelineView.MarkerTime += 10.0f;
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            timelinePropertiesController.togglePlayPreview(timelineView.MarkerTime);
        }

        void playFullButton_MouseButtonClick(Widget source, EventArgs e)
        {
            timelinePropertiesController.togglePlayFull();
        }

        void timelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            playButton.Caption = "Play";
            playButton.ImageBox.setItemResource("Timeline/PlayIcon");
            rewindButton.Enabled = true;
            fastForwardButton.Enabled = true;
            timelineView.CurrentData = editingStoppedLastData;
        }

        void timelineController_TimelinePlaybackStarted(object sender, EventArgs e)
        {
            playButton.Caption = "Stop";
            playButton.ImageBox.setItemResource("Timeline/StopIcon");
            rewindButton.Enabled = false;
            fastForwardButton.Enabled = false;
            timelineView.CurrentData = null;
        }

        void timelineController_TimeTicked(float currentTime)
        {
            timelineView.MarkerTime = currentTime;
        }

        private void stopTimelineIfPlaying()
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback(false);
            }
        }

        private void deleteSelectedActions()
        {
            stopTimelineIfPlaying();
            foreach (TimelineActionData data in timelineView.SelectedData)
            {
                timelinePropertiesController.CurrentTimeline.removeAction(data.Action);
            }
        }

        void window_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            processFormKeys(ke);
        }

        void timelineView_KeyReleased(object sender, KeyEventArgs e)
        {
            processFormKeys(e);
        }

        void processFormKeys(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case KeyboardButtonCode.KC_DELETE:
                    deleteSelectedActions();
                    break;
                case KeyboardButtonCode.KC_SPACE:
                    timelinePropertiesController.togglePlayPreview(timelineView.MarkerTime);
                    break;
            }
        }

        void timelineView_ActiveDataChanging(object sender, CancelEventArgs e)
        {
            e.Cancel = timelineController.Playing;
        }

        void timelineView_MarkerMoved(TimelineView source, float arg)
        {
            if (MarkerMoved != null)
            {
                MarkerMoved.Invoke(this, arg);
            }
        }
    }
}
