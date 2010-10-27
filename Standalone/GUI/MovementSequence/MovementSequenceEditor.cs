using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller;
using Medical.Muscles;

namespace Medical.GUI
{
    class MovementSequenceEditor : Dialog
    {
        private MenuCtrl fileMenuCtrl;
        private TimelineDataProperties actionProperties;
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Button playButton;
        private NumericEdit durationEdit;
        private bool allowSynchronization = true;

        private MovementSequenceController movementSequenceController;

        public MovementSequenceEditor(MovementSequenceController movementSequenceController)
            : base("Medical.GUI.MovementSequence.MovementSequenceEditor.layout")
        {
            this.movementSequenceController = movementSequenceController;
            movementSequenceController.CurrentSequenceChanged += new MovementSequenceEvent(movementSequenceController_CurrentSequenceChanged);
            movementSequenceController.PlaybackStarted += new MovementSequenceEvent(movementSequenceController_PlaybackStarted);
            movementSequenceController.PlaybackStopped += new MovementSequenceEvent(movementSequenceController_PlaybackStopped);
            movementSequenceController.PlaybackUpdate += new MovementSequenceEvent(movementSequenceController_PlaybackUpdate);

            //Menu
            MenuBar menuBar = window.findWidget("MenuBar") as MenuBar;
            MenuItem fileMenu = menuBar.addItem("File", MenuItemType.Popup);
            fileMenuCtrl = fileMenu.createItemChild();
            MenuItem newSequence = fileMenuCtrl.addItem("New");

            MenuItem openSequence = fileMenuCtrl.addItem("Open");
            
            MenuItem saveSequence = fileMenuCtrl.addItem("Save");
            
            MenuItem saveSequenceAs = fileMenuCtrl.addItem("Save As");
            

            //Remove action button
            Button removeActionButton = window.findWidget("RemoveAction") as Button;
            
            //Duration Edit
            durationEdit = new NumericEdit(window.findWidget("SequenceDuration") as Edit);
            durationEdit.AllowFloat = true;
            durationEdit.ValueChanged += new MyGUIEvent(durationEdit_ValueChanged);

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.DurationChanged += new EventHandler(timelineView_DurationChanged);
            timelineView.Duration = 5.0f;

            //Properties
            ScrollView timelinePropertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;

            //Timeline filter
            ScrollView timelineFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);

            //Add tracks to timeline.
            timelineView.addTrack("Muscle Position", Color.Red);
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (movementSequenceController.Playing)
            {
                movementSequenceController.stopPlayback();
            }
            else
            {
                movementSequenceController.playCurrentSequence();
            }
        }

        void createNewSequence()
        {
            MovementSequence movementSequence = new MovementSequence();
            movementSequence.Duration = 5.0f;
            movementSequenceController.CurrentSequence = movementSequence;
        }

        #region MovementSequenceController Callbacks

        void movementSequenceController_PlaybackStopped(MovementSequenceController controller)
        {
            playButton.Caption = "Play";
        }

        void movementSequenceController_PlaybackStarted(MovementSequenceController controller)
        {
            playButton.Caption = "Stop";
        }

        void movementSequenceController_PlaybackUpdate(MovementSequenceController controller)
        {
            timelineView.MarkerTime = controller.CurrentTime;
        }

        void movementSequenceController_CurrentSequenceChanged(MovementSequenceController controller)
        {
            timelineView.removeAllData();
            MovementSequence movementSequence = controller.CurrentSequence;
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

        #endregion

        #region Timeline Callbacks

        void trackFilter_AddTrackItem(string name)
        {
            if (movementSequenceController.CurrentSequence == null)
            {
                createNewSequence();
            }
            MovementSequenceState state = new MovementSequenceState();
            state.captureState();
            state.StartTime = timelineView.MarkerTime;
            movementSequenceController.CurrentSequence.addState(state);
            timelineView.addData(new MovementKeyframeData(state, movementSequenceController.CurrentSequence));
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
                if (sender != movementSequenceController.CurrentSequence && movementSequenceController.CurrentSequence != null)
                {
                    movementSequenceController.CurrentSequence.Duration = duration;
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

        #endregion
    }
}
