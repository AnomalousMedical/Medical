﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Medical.GUI;

namespace Medical
{
    public class SequencePlayer : PinableMDIDialog
    {
        private MovementSequenceController sequenceController;
        private SequenceMenu sequenceMenu;
        private Button playButton;
        private Button stopButton;
        private EditBox nowPlaying;
        private MusclePositionController musclePositionController;

        public SequencePlayer(MovementSequenceController sequenceController, MusclePositionController musclePositionController)
            :base("Medical.GUI.SequencePlayer.SequencePlayer.layout")
        {
            this.sequenceController = sequenceController;
            sequenceMenu = new SequenceMenu(sequenceController);

            this.musclePositionController = musclePositionController;

            Button sequenceButton = window.findWidget("Sequence") as Button;
            sequenceButton.MouseButtonClick += new MyGUIEvent(sequenceButton_MouseButtonClick);

            playButton = window.findWidget("Play") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            stopButton = window.findWidget("Stop") as Button;
            stopButton.MouseButtonClick += new MyGUIEvent(stopButton_MouseButtonClick);

            nowPlaying = window.findWidget("NowPlaying") as EditBox;

            sequenceController.PlaybackStarted += new MovementSequenceEvent(sequenceController_PlaybackStarted);
            sequenceController.PlaybackStopped += new MovementSequenceEvent(sequenceController_PlaybackStopped);
            sequenceController.CurrentSequenceChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceChanged);
        }

        public override void Dispose()
        {
            sequenceMenu.Dispose();
            base.Dispose();
        }

        void stopButton_MouseButtonClick(Widget source, EventArgs e)
        {
            sequenceController.pausePlayback(); //We pause here because that does not attempt to reset the sequence. This makes a nicer transition back to neutral with the timed blend below
            musclePositionController.timedBlend(musclePositionController.BindPosition, MedicalConfig.CameraTransitionTime);
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            sequenceController.playCurrentSequence();
        }

        void sequenceButton_MouseButtonClick(Widget source, EventArgs e)
        {
            sequenceMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        void sequenceController_PlaybackStopped(MovementSequenceController controller)
        {
            playButton.Enabled = true;
            stopButton.Enabled = false;
        }

        void sequenceController_PlaybackStarted(MovementSequenceController controller)
        {
            playButton.Enabled = false;
            stopButton.Enabled = true;
        }

        void sequenceController_CurrentSequenceChanged(MovementSequenceController controller)
        {
            if (controller.CurrentSequence == null)
            {
                nowPlaying.Caption = "None";
                playButton.Enabled = false;
                stopButton.Enabled = false;
            }
            else
            {
                nowPlaying.Caption = controller.CurrentSequence.Name;
                playButton.Enabled = true;
                stopButton.Enabled = false;
            }
        }

        protected override bool keepOpenFromPoint(int x, int y)
        {
            return (sequenceMenu.Visible && sequenceMenu.contains(x, y)) || base.keepOpenFromPoint(x, y);
        }
    }
}
