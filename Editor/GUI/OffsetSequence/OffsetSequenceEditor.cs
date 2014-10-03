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
    public class OffsetSequenceEditor : LayoutComponent, EditMenuProvider
    {
        private TimelineDataProperties actionProperties;
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Button playButton;
        private NumericEdit durationEdit;
        private bool allowSynchronization = true;
        private SaveableClipboard clipboard;

        private OffsetModifierSequence offsetSequence;

        public OffsetSequenceEditor(SaveableClipboard clipboard, MyGUIViewHost viewHost, OffsetSequenceEditorView view)
            : base("Medical.GUI.OffsetSequence.OffsetSequenceEditor.layout", viewHost)
        {
            this.clipboard = clipboard;

            widget.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);
            widget.RootKeyChangeFocus += new MyGUIEvent(widget_RootKeyChangeFocus);

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
            timelineView.Duration = 1.0f;
            timelineView.KeyReleased += new EventHandler<KeyEventArgs>(timelineView_KeyReleased);

            //Properties
            ScrollView timelinePropertiesScrollView = widget.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;
            actionProperties.addPanel("Offset Position", new OffsetKeyframeProperties(timelinePropertiesScrollView));

            //Timeline filter
            ScrollView timelineFilterScrollView = widget.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            numberLine = new NumberLine(widget.findWidget("NumberLine") as ScrollView, timelineView);

            //Add tracks to timeline.
            timelineView.addTrack("Offset Position");

            CurrentSequence = view.Sequence;

            ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void reverseSides()
        {
            if (offsetSequence != null)
            {
                //offsetSequence.reverseSides();
            }
        }

        public void cut()
        {
            OffsetSequenceClipboardContainer clipContainer = new OffsetSequenceClipboardContainer();
            clipContainer.addKeyFrames(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
            deleteSelectedActions();
        }

        public void copy()
        {
            OffsetSequenceClipboardContainer clipContainer = new OffsetSequenceClipboardContainer();
            clipContainer.addKeyFrames(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
        }

        public void paste()
        {
            OffsetSequenceClipboardContainer clipContainer = clipboard.createCopy<OffsetSequenceClipboardContainer>();
            if (clipContainer != null)
            {
                clipContainer.addKeyFramesToSequence(offsetSequence, this, timelineView.MarkerTime, timelineView.Duration);
            }
        }

        public void selectAll()
        {
            timelineView.selectAll();
        }

        public OffsetModifierSequence CurrentSequence
        {
            get
            {
                return offsetSequence;
            }
            set
            {
                offsetSequence = value;
                timelineView.removeAllData();
                if (offsetSequence != null)
                {
                    timelineView.Duration = 1.0f;
                    foreach (var state in offsetSequence.Keyframes)
                    {
                        timelineView.addData(new OffsetKeyframeData(state, offsetSequence));
                    }
                }
                else
                {
                    timelineView.Duration = 5.0f;
                }
            }
        }

        internal void addStateToTimeline(OffsetModifierKeyframe state)
        {
            timelineView.addData(new OffsetKeyframeData(state, offsetSequence));
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //if (movementSequenceController.Playing)
            //{
            //    movementSequenceController.stopPlayback();
            //}
            //else
            //{
            //    movementSequenceController.CurrentSequence = offsetSequence;
            //    movementSequenceController.playCurrentSequence();
            //}
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (offsetSequence != null)
            {
                OffsetKeyframeData data = timelineView.CurrentData as OffsetKeyframeData;
                timelineView.removeData(data);
                offsetSequence.removeFrame(data.KeyFrame);
            }
        }

        private void deleteSelectedActions()
        {
            foreach (OffsetKeyframeData data in timelineView.SelectedData)
            {
                timelineView.removeData(data);
                offsetSequence.removeFrame(data.KeyFrame);
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

        //void movementSequenceController_PlaybackStopped(MovementSequenceController controller)
        //{
        //    playButton.Caption = "Play";
        //    playButton.ImageBox.setItemResource("Timeline/PlayIcon");
        //}

        //void movementSequenceController_PlaybackStarted(MovementSequenceController controller)
        //{
        //    playButton.Caption = "Stop";
        //    playButton.ImageBox.setItemResource("Timeline/StopIcon");
        //}

        //void movementSequenceController_PlaybackUpdate(MovementSequenceController controller)
        //{
        //    timelineView.MarkerTime = controller.CurrentTime;
        //}

        void trackFilter_AddTrackItem(string name, Object trackUserObject)
        {
            if (offsetSequence != null)
            {
                OffsetModifierKeyframe keyframe = new OffsetModifierKeyframe();
                //state.captureState();
                keyframe.BlendAmount = timelineView.MarkerTime;
                offsetSequence.addKeyframe(keyframe);
                offsetSequence.sort();
                addStateToTimeline(keyframe);
            }
        }

        private void synchronizeDuration(object sender, float duration)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                //if (sender != durationEdit)
                //{
                //    durationEdit.FloatValue = duration;
                //}
                //if (sender != timelineView)
                //{
                //    timelineView.Duration = duration;
                //}
                //if (sender != offsetSequence && offsetSequence != null)
                //{
                //    offsetSequence.Duration = duration;
                //}
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
