using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Platform;
using Engine;
using Logging;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI
{
    public class TimelineEditorComponent : LayoutComponent, EditMenuProvider
    {
        private String windowTitle;
        private const String windowTitleFormat = "{0} - {1}";

        private TimelineController timelineController;
        private TrackFilter actionFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Dictionary<TimelineAction, TimelineActionData> actionDataBindings = new Dictionary<TimelineAction, TimelineActionData>();
        private EditorController editorController;
        private SaveableClipboard clipboard;
        private TimelineActionFactory actionFactory;
        private TimelineDataProperties dataProperties;
        private TimelineData editingStoppedLastData;
        private Timeline currentTimeline;
        private PropEditController propEditController;

        private Button playButton;
        private Button rewindButton;
        private Button fastForwardButton;
        private bool blockSelectionChanges = false;

        public TimelineEditorComponent(MyGUIViewHost viewHost, TimelineController timelineController, EditorController editorController, SaveableClipboard clipboard, EditorPlugin editorPlugin)
            :base("Medical.GUI.TimelineEditor.TimelineEditorComponent.layout", viewHost)
        {
            Widget window = this.widget;
            window.RootKeyChangeFocus += new MyGUIEvent(window_RootKeyChangeFocus);

            //windowTitle = window.Caption;

            this.clipboard = clipboard;
            this.editorController = editorController;
            this.propEditController = editorPlugin.PropEditController;

            this.timelineController = timelineController;
            timelineController.TimelinePlaybackStarted += timelineController_TimelinePlaybackStarted;
            timelineController.TimelinePlaybackStopped += timelineController_TimelinePlaybackStopped;
            timelineController.TimeTicked += timelineController_TimeTicked;

            window.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);

            //Remove action button
            Button removeActionButton = window.findWidget("RemoveAction") as Button;
            removeActionButton.MouseButtonClick += new MyGUIEvent(removeActionButton_MouseButtonClick);

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

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
            timelineView.ActiveDataChanged += new EventHandler(timelineView_ActiveDataChanged);

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

            //Enabled = false;

            ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
        }

        public override void Dispose()
        {
            ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).removeMenuProvider(this);
            actionFactory.Dispose();
            CurrentTimeline = null;
            timelineController.TimelinePlaybackStarted -= timelineController_TimelinePlaybackStarted;
            timelineController.TimelinePlaybackStopped -= timelineController_TimelinePlaybackStopped;
            timelineController.TimeTicked -= timelineController_TimeTicked;
            timelineView.Dispose();
            base.Dispose();
        }

        public void paste()
        {
            TimelineActionClipboardContainer clipContainer = clipboard.createCopy<TimelineActionClipboardContainer>();
            if (clipContainer != null)
            {
                clipContainer.addActionsToTimeline(currentTimeline, timelineView.MarkerTime);
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

        public void updateFileName(String currentFile)
        {
            if (currentFile == null)
            {
                //window.Caption = windowTitle;
            }
            else
            {
                //window.Caption = String.Format(windowTitleFormat, windowTitle, currentFile);
            }
        }

        public Timeline CurrentTimeline
        {
            get
            {
                return currentTimeline;
            }
            set
            {
                if (currentTimeline != value)
                {
                    if (currentTimeline != null)
                    {
                        currentTimeline.ActionAdded -= currentTimeline_ActionAdded;
                        currentTimeline.ActionRemoved -= currentTimeline_ActionRemoved;
                    }
                    timelineView.removeAllData();
                    actionDataBindings.Clear();
                    currentTimeline = value;
                    //Enabled = currentTimeline != null;
                    if (currentTimeline != null)
                    {
                        timelineController.setAsTimelineController(currentTimeline);
                        foreach (TimelineAction action in currentTimeline.Actions)
                        {
                            addActionToTimeline(action);
                        }

                        currentTimeline.ActionAdded += currentTimeline_ActionAdded;
                        currentTimeline.ActionRemoved += currentTimeline_ActionRemoved;
                    }
                }
            }
        }

        void removeActionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            TimelineActionData data = (TimelineActionData)timelineView.CurrentData;
            if (data != null)
            {
                stopTimelineIfPlaying();
                currentTimeline.removeAction(data.Action);
            }
        }

        void actionFilter_AddTrackItem(string name)
        {
            TimelineAction action = actionFactory.createAction(name);
            action.StartTime = timelineView.MarkerTime;
            currentTimeline.addAction(action);
            action.capture();
            timelineView.CurrentData = actionDataBindings[action];
        }

        void timelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            blockSelectionChanges = false;
            playButton.Caption = "Play";
            playButton.ImageBox.setItemResource("Timeline/PlayIcon");
            rewindButton.Enabled = true;
            fastForwardButton.Enabled = true;
            if (currentTimeline != null)
            {
                timelineController.setAsTimelineController(currentTimeline);
            }
            timelineView.CurrentData = editingStoppedLastData;
        }

        void timelineController_TimelinePlaybackStarted(object sender, EventArgs e)
        {
            editingStoppedLastData = timelineView.CurrentData;
            playButton.Caption = "Stop";
            playButton.ImageBox.setItemResource("Timeline/StopIcon");
            rewindButton.Enabled = false;
            fastForwardButton.Enabled = false;
            timelineView.CurrentData = null;
            blockSelectionChanges = true;
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
                currentTimeline.removeAction(data.Action);
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
            }
        }

        public void togglePlayPreview()
        {
            togglePlayPreview(timelineView.MarkerTime);
        }

        public void togglePlayPreview(float startTime)
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback(false);
            }
            else if (currentTimeline != null)
            {
                timelineController.startPlayback(currentTimeline, startTime, false);
            }
        }

        void timelineView_ActiveDataChanging(object sender, CancelEventArgs e)
        {
            e.Cancel = blockSelectionChanges;
        }

        void timelineView_MarkerMoved(TimelineView source, float arg)
        {
            propEditController.MarkerPosition = arg;
        }

        void timelineView_ActiveDataChanged(object sender, EventArgs e)
        {
            EditInterfaceHandler editInterfaceHandler = ViewHost.Context.getModel<EditInterfaceHandler>(EditInterfaceHandler.DefaultName);
            if (editInterfaceHandler != null)
            {
                TimelineActionData actionData = (TimelineActionData)timelineView.CurrentData;
                if (actionData != null)
                {
                    editInterfaceHandler.changeEditInterface(actionData.Action.getEditInterface());
                }
                else if(currentTimeline != null)
                {
                    editInterfaceHandler.changeEditInterface(currentTimeline.getEditInterface());
                }
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
            togglePlayPreview(timelineView.MarkerTime);
        }

        void currentTimeline_ActionRemoved(object sender, TimelineActionEventArgs e)
        {
            removeActionFromTimeline(e.Action);
        }

        void currentTimeline_ActionAdded(object sender, TimelineActionEventArgs e)
        {
            addActionToTimeline(e.Action);
        }

        void removeActionFromTimeline(TimelineAction action)
        {
            timelineView.removeData(actionDataBindings[action]);
            actionDataBindings.Remove(action);
        }

        void addActionToTimeline(TimelineAction action)
        {
            TimelineActionData data = new TimelineActionData(action);
            actionDataBindings.Add(action, data);
            timelineView.addData(data);
        }

        void window_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs rfea = (RootFocusEventArgs)e;
            if (rfea.Focus)
            {
                ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
            }
        }
    }
}
