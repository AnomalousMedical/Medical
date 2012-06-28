using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller;
using Medical.Muscles;
using Engine.Platform;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI
{
    public class MovementSequenceEditor : LayoutComponent, EditMenuProvider
    {
        private TimelineDataProperties actionProperties;
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Button playButton;
        private NumericEdit durationEdit;
        private bool allowSynchronization = true;
        private SaveableClipboard clipboard;
        private EditorController editorController;

        private MovementSequenceController movementSequenceController;
        private MovementSequence movementSequence;

        public MovementSequenceEditor(MovementSequenceController movementSequenceController, SaveableClipboard clipboard, EditorController editorController, MyGUIViewHost viewHost, MovementSequenceEditorView view)
            : base("Medical.GUI.MovementSequence.MovementSequenceEditor.layout", viewHost)
        {
            this.clipboard = clipboard;
            this.editorController = editorController;

            widget.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);
            widget.RootKeyChangeFocus += new MyGUIEvent(widget_RootKeyChangeFocus);

            this.movementSequenceController = movementSequenceController;
            movementSequenceController.PlaybackStarted += movementSequenceController_PlaybackStarted;
            movementSequenceController.PlaybackStopped += movementSequenceController_PlaybackStopped;
            movementSequenceController.PlaybackUpdate +=  movementSequenceController_PlaybackUpdate;
            if (view.ListenForSequenceChanges)
            {
                movementSequenceController.CurrentSequenceChanged += movementSequenceController_CurrentSequenceChanged;
            }

            //Remove button
            Button removeButton = widget.findWidget("RemoveAction") as Button;
            removeButton.MouseButtonClick += new MyGUIEvent(removeButton_MouseButtonClick);
            
            //Duration Edit
            durationEdit = new NumericEdit(widget.findWidget("SequenceDuration") as EditBox);
            durationEdit.AllowFloat = true;
            durationEdit.ValueChanged += new MyGUIEvent(durationEdit_ValueChanged);
            durationEdit.MinValue = 0.0f;
            durationEdit.MaxValue = 600;

            //Play Button
            playButton = widget.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            //Timeline view
            ScrollView timelineViewScrollView = widget.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.DurationChanged += new EventHandler(timelineView_DurationChanged);
            timelineView.Duration = 5.0f;
            timelineView.KeyReleased += new EventHandler<KeyEventArgs>(timelineView_KeyReleased);

            //Properties
            ScrollView timelinePropertiesScrollView = widget.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;
            actionProperties.addPanel("Muscle Position", new MovementKeyframeProperties(timelinePropertiesScrollView));

            //Timeline filter
            ScrollView timelineFilterScrollView = widget.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            numberLine = new NumberLine(widget.findWidget("NumberLine") as ScrollView, timelineView);

            //Add tracks to timeline.
            timelineView.addTrack("Muscle Position", Color.Red);

            CurrentSequence = view.Sequence;

            ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
        }

        public override void Dispose()
        {
            movementSequenceController.PlaybackStarted -= movementSequenceController_PlaybackStarted;
            movementSequenceController.PlaybackStopped -= movementSequenceController_PlaybackStopped;
            movementSequenceController.PlaybackUpdate -= movementSequenceController_PlaybackUpdate;
            movementSequenceController.CurrentSequenceChanged -= movementSequenceController_CurrentSequenceChanged;
            base.Dispose();
        }

        public void reverseSides()
        {
            if (movementSequence != null)
            {
                movementSequence.reverseSides();
            }
        }

        public void cut()
        {
            MovementSequenceClipboardContainer clipContainer = new MovementSequenceClipboardContainer();
            clipContainer.addKeyFrames(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
            deleteSelectedActions();
        }

        public void copy()
        {
            MovementSequenceClipboardContainer clipContainer = new MovementSequenceClipboardContainer();
            clipContainer.addKeyFrames(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
        }

        public void paste()
        {
            MovementSequenceClipboardContainer clipContainer = clipboard.createCopy<MovementSequenceClipboardContainer>();
            if (clipContainer != null)
            {
                clipContainer.addKeyFramesToSequence(movementSequence, this, timelineView.MarkerTime, timelineView.Duration);
            }
        }

        public void selectAll()
        {
            timelineView.selectAll();
        }

        public MovementSequence CurrentSequence
        {
            get
            {
                return movementSequence;
            }
            set
            {
                movementSequence = value;
                timelineView.removeAllData();
                if (movementSequence != null)
                {
                    timelineView.Duration = movementSequence.Duration;
                    foreach (MovementSequenceState state in movementSequence.States)
                    {
                        timelineView.addData(new MovementKeyframeData(state, movementSequence));
                    }
                }
                else
                {
                    timelineView.Duration = 5.0f;
                }
            }
        }

        internal void addStateToTimeline(MovementSequenceState state)
        {
            timelineView.addData(new MovementKeyframeData(state, movementSequence));
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (movementSequenceController.Playing)
            {
                movementSequenceController.stopPlayback();
            }
            else
            {
                movementSequenceController.CurrentSequence = movementSequence;
                movementSequenceController.playCurrentSequence();
            }
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MovementKeyframeData data = timelineView.CurrentData as MovementKeyframeData;
            timelineView.removeData(data);
            movementSequence.deleteState(data.KeyFrame);
        }

        private void deleteSelectedActions()
        {
            foreach (MovementKeyframeData data in timelineView.SelectedData)
            {
                timelineView.removeData(data);
                movementSequence.deleteState(data.KeyFrame);
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
                    deleteSelectedActions();
                    break;
            }
        }

        void movementSequenceController_PlaybackStopped(MovementSequenceController controller)
        {
            playButton.Caption = "Play";
            playButton.ImageBox.setItemResource("Timeline/PlayIcon");
        }

        void movementSequenceController_PlaybackStarted(MovementSequenceController controller)
        {
            playButton.Caption = "Stop";
            playButton.ImageBox.setItemResource("Timeline/StopIcon");
        }

        void movementSequenceController_PlaybackUpdate(MovementSequenceController controller)
        {
            timelineView.MarkerTime = controller.CurrentTime;
        }

        void trackFilter_AddTrackItem(string name)
        {
            MovementSequenceState state = new MovementSequenceState();
            state.captureState();
            state.StartTime = timelineView.MarkerTime;
            movementSequence.addState(state);
            addStateToTimeline(state);
        }

        private void synchronizeDuration(object sender, float duration)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (sender != durationEdit)
                {
                    durationEdit.FloatValue = duration;
                }
                if (sender != timelineView)
                {
                    timelineView.Duration = duration;
                }
                if (sender != movementSequence && movementSequence != null)
                {
                    movementSequence.Duration = duration;
                }
                allowSynchronization = true;
            }
        }

        void timelineView_DurationChanged(object sender, EventArgs e)
        {
            synchronizeDuration(sender, timelineView.Duration);
        }

        void durationEdit_ValueChanged(Widget source, EventArgs e)
        {
            synchronizeDuration(durationEdit, durationEdit.FloatValue);
        }

        void movementSequenceController_CurrentSequenceChanged(MovementSequenceController controller)
        {
            CurrentSequence = controller.CurrentSequence;
        }

        void widget_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs rfea = (RootFocusEventArgs)e;
            if (rfea.Focus)
            {
                ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
            }
        }
    }
}
