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
        private TimelineAction copySourceAction;
        private CopySaver copySaver = new CopySaver();
        private TimelineActionFactory actionFactory;
        private TimelinePropertiesController timelinePropertiesController;

        private Button playButton;
        private Button playFullButton;
        private Button rewindButton;
        private Button fastForwardButton;

        private const int START_COLUMN_WIDTH = 100;

        public TimelineProperties(TimelineController timelineController, EditorPlugin editorPlugin, GUIManager guiManager, TimelinePropertiesController timelinePropertiesController, TimelineFileBrowserDialog fileBrowserDialog)
            :base("Medical.GUI.Timeline.TimelineProperties.layout")
        {
            this.timelinePropertiesController = timelinePropertiesController;

            this.timelineController = timelineController;
            timelineController.TimelinePlaybackStarted += new EventHandler(timelineController_TimelinePlaybackStarted);
            timelineController.TimelinePlaybackStopped += new EventHandler(timelineController_TimelinePlaybackStopped);
            timelineController.TimeTicked += new TimeTickEvent(timelineController_TimeTicked);
            timelineController.ResourceLocationChanged += new EventHandler(timelineController_ResourceLocationChanged);

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

            setEnabled(false);
        }

        public override void Dispose()
        {
            actionFactory.Dispose();
            timelineController.FileBrowser = null;
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
            if (timelineController.ResourceProvider != null && timelinePropertiesController.CurrentTimelineFile != null)
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
            if (copySourceAction != null)
            {
                TimelineAction copiedAction = copySaver.copy<TimelineAction>(copySourceAction);
                copiedAction.StartTime = timelineView.MarkerTime;
                timelinePropertiesController.CurrentTimeline.addAction(copiedAction);
                timelineView.CurrentData = actionDataBindings[copiedAction];
            }
        }

        public void copy()
        {
            TimelineActionData currentData = timelineView.CurrentData as TimelineActionData;
            if (currentData != null)
            {
                copySourceAction = copySaver.copy<TimelineAction>(currentData.Action);
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
                timelineView.MarkerTime = value;
            }
        }

        public bool KeyFocusWidget
        {
            get
            {
                return InputManager.Instance.getKeyFocusWidget() == window;
            }
        }

        void removeActionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            deleteCurrentAction();
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

        void timelineController_ResourceLocationChanged(object sender, EventArgs e)
        {
            setEnabled(timelineController.ResourceProvider != null);
            updateWindowCaption();
        }

        private void setEnabled(bool enabled)
        {
            actionFilter.Enabled = enabled;
            playButton.Enabled = enabled;
            playFullButton.Enabled = enabled;
            timelineView.Enabled = enabled;
            fastForwardButton.Enabled = enabled;
            rewindButton.Enabled = enabled;
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
            timelinePropertiesController.playPreview(timelineView.MarkerTime);
        }

        void playFullButton_MouseButtonClick(Widget source, EventArgs e)
        {
            timelinePropertiesController.playFull();
        }

        void timelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            playButton.Caption = "Play";
            playButton.StaticImage.setItemResource("Timeline/PlayIcon");
            rewindButton.Enabled = true;
            fastForwardButton.Enabled = true;
        }

        void timelineController_TimelinePlaybackStarted(object sender, EventArgs e)
        {
            playButton.Caption = "Stop";
            playButton.StaticImage.setItemResource("Timeline/StopIcon");
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

        private void deleteCurrentAction()
        {
            TimelineActionData data = (TimelineActionData)timelineView.CurrentData;
            if (data != null)
            {
                stopTimelineIfPlaying();
                timelinePropertiesController.CurrentTimeline.removeAction(data.Action);
            }
        }

        void window_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            switch (ke.Key)
            {
                case KeyboardButtonCode.KC_DELETE:
                    deleteCurrentAction();
                    break;
            }
        }
    }
}
