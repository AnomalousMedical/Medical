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
        private TimelineController timelineController;
        private TrackFilter actionFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private TimelineActionDataManager actionDataManager = new TimelineActionDataManager();
        private EditorController editorController;
        private SaveableClipboard clipboard;
        private TimelineData editingStoppedLastData;
        private Timeline currentTimeline;
        private PropEditController propEditController;
        private TimelineActionData currentActionData = null;

        private Button playButton;
        private Button rewindButton;
        private Button fastForwardButton;
        private bool blockSelectionChanges = false;

        public TimelineEditorComponent(MyGUIViewHost viewHost, TimelineController timelineController, EditorController editorController, SaveableClipboard clipboard, EditorPlugin editorPlugin)
            :base("Medical.GUI.TimelineEditor.TimelineEditorComponent.layout", viewHost)
        {
            Widget window = this.widget;
            window.RootKeyChangeFocus += new MyGUIEvent(window_RootKeyChangeFocus);

            this.clipboard = clipboard;
            this.editorController = editorController;
            this.propEditController = editorPlugin.PropEditController;

            this.timelineController = timelineController;
            timelineController.TimelinePlaybackStarted += timelineController_TimelinePlaybackStarted;
            timelineController.TimelinePlaybackStopped += timelineController_TimelinePlaybackStopped;
            timelineController.TimeTicked += timelineController_TimeTicked;

            window.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);

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

            //Track filter
            ScrollView trackFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            actionFilter = new TrackFilter(trackFilterScrollView, timelineView);
            actionFilter.AddTrackItem += new AddTrackItemCallback(actionFilter_AddTrackItem);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);

            //Add tracks to timeline.
            buildTrackActions();
            foreach (TimelineActionPrototype actionProp in actionDataManager.Prototypes)
            {
                timelineView.addTrack(actionProp.TypeName, actionProp.Color);
            }

            //Enabled = false;

            ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
        }

        public override void Dispose()
        {
            actionDataManager.Dispose();
            ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).removeMenuProvider(this);
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
                    actionDataManager.clearData();
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

        void actionFilter_AddTrackItem(string name)
        {
            TimelineAction action = actionDataManager.createAction(name);
            action.StartTime = timelineView.MarkerTime;
            currentTimeline.addAction(action);
            action.capture();
            timelineView.CurrentData = actionDataManager[action];
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

        void timelineView_ActiveDataChanged(object sender, EventArgs e)
        {
            if (currentActionData != null)
            {
                currentActionData.editingCompleted();
            }
            currentActionData = (TimelineActionData)timelineView.CurrentData;
            if (currentActionData != null)
            {
                currentActionData.editingStarted();
            }

            EditInterfaceHandler editInterfaceHandler = ViewHost.Context.getModel<EditInterfaceHandler>(EditInterfaceHandler.DefaultName);
            if (editInterfaceHandler != null)
            {
                if (currentActionData != null)
                {
                    editInterfaceHandler.changeEditInterface(currentActionData.Action.getEditInterface());
                }
                else if (currentTimeline != null)
                {
                    editInterfaceHandler.changeEditInterface(currentTimeline.getEditInterface());
                }
            }
        }

        void timelineView_MarkerMoved(TimelineView source, float arg)
        {
            propEditController.MarkerPosition = arg;
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
            timelineView.removeData(actionDataManager[action]);
            actionDataManager.destroyData(action);
        }

        void addActionToTimeline(TimelineAction action)
        {
            timelineView.addData(actionDataManager.createData(action));
        }

        void window_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs rfea = (RootFocusEventArgs)e;
            if (rfea.Focus)
            {
                ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
            }
        }

        void buildTrackActions()
        {
            actionDataManager.addPrototype(new TimelineActionPrototype("Change Medical State", typeof(ChangeMedicalStateAction), new Color(128 / 255f, 0 / 255f, 255 / 255f)));
            actionDataManager.addPrototype(new TimelineActionPrototype("Highlight Teeth", typeof(HighlightTeethAction), new Color(247 / 255f, 150 / 255f, 70 / 255f)));
            actionDataManager.addPrototype(new TimelineActionPrototype("Change Layers", typeof(LayerChangeAction), new Color(155 / 255f, 187 / 255f, 89 / 255f)));
            actionDataManager.addPrototype(new TimelineActionPrototype("Move Camera", typeof(MoveCameraAction), new Color(192 / 255f, 80 / 255f, 77 / 255f)));
            actionDataManager.addPrototype(new TimelineActionPrototype("Play Sequence", typeof(PlaySequenceAction), new Color(31 / 255f, 73 / 255f, 125 / 255f)));
            actionDataManager.addPrototype(new TimelineActionPrototype("Muscle Position", typeof(MusclePositionAction), new Color(255 / 255f, 0 / 255f, 0 / 255f)));
            actionDataManager.addPrototype(new TimelineActionPrototype("Show Image", typeof(ShowImageAction), new Color(31 / 255f, 73 / 255f, 125 / 255f)));
            actionDataManager.addPrototype(new TimelineActionPrototype("Show Text", typeof(ShowTextAction), new Color(31 / 255f, 255 / 255f, 125 / 255f)));
            actionDataManager.addPrototype(new TimelineActionPrototype("Play Sound", typeof(PlaySoundAction), new Color(0 / 255f, 0 / 255f, 0 / 255f)));
            actionDataManager.addPrototype(new ShowPropActionPrototype(new Color(128 / 255f, 0 / 255f, 255 / 255f), propEditController));
        }
    }
}
