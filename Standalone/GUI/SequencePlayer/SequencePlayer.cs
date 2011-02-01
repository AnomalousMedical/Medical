using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical
{
    public class SequencePlayer : FixedSizeDialog
    {
        private MovementSequenceController sequenceController;
        private SequenceMenu sequenceMenu;
        private Button playButton;
        private Button stopButton;
        private Edit nowPlaying;

        public SequencePlayer(MovementSequenceController sequenceController)
            :base("Medical.GUI.SequencePlayer.SequencePlayer.layout")
        {
            this.sequenceController = sequenceController;
            sequenceMenu = new SequenceMenu(sequenceController);

            Button sequenceButton = window.findWidget("Sequence") as Button;
            sequenceButton.MouseButtonClick += new MyGUIEvent(sequenceButton_MouseButtonClick);

            playButton = window.findWidget("Play") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            stopButton = window.findWidget("Stop") as Button;
            stopButton.MouseButtonClick += new MyGUIEvent(stopButton_MouseButtonClick);

            nowPlaying = window.findWidget("NowPlaying") as Edit;

            sequenceController.PlaybackStarted += new MovementSequenceEvent(sequenceController_PlaybackStarted);
            sequenceController.PlaybackStopped += new MovementSequenceEvent(sequenceController_PlaybackStopped);
            sequenceController.CurrentSequenceChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceChanged);
        }

        void stopButton_MouseButtonClick(Widget source, EventArgs e)
        {
            sequenceController.stopPlayback();
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
    }
}
