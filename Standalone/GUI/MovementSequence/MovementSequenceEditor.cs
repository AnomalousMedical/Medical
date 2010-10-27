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
            MenuItem newTimeline = fileMenuCtrl.addItem("New");
            
            MenuItem openTimeline = fileMenuCtrl.addItem("Open");
            
            MenuItem saveTimeline = fileMenuCtrl.addItem("Save");
            
            MenuItem saveTimelineAs = fileMenuCtrl.addItem("Save As");
            

            //Remove action button
            Button removeActionButton = window.findWidget("RemoveAction") as Button;
            

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);

            //Properties
            ScrollView timelinePropertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;

            //Timeline filter
            ScrollView timelineFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            

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
                foreach(MovementSequenceState state in movementSequence.States)
                {
                    timelineView.addData(new MovementKeyframeData(state, movementSequence));
                }
            }
        }

        #endregion
    }
}
