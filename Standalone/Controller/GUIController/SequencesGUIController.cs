using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Engine.ObjectManagement;
using Engine.Resources;
using Medical.Muscles;
using MyGUIPlugin;

namespace Medical.GUI
{
    class SequencesGUIController
    {
        private MovementSequenceController sequenceController;
        private Button sequencesButton;
        private PopupMenu sequenceMenu;
        private Button playButton;
        private Button stopButton;
        private StaticText nowPlayingLabel;

        public SequencesGUIController(Gui ribbonLayout, MovementSequenceController movementSequenceController)
        {
            this.sequenceController = movementSequenceController;
            sequencesButton = ribbonLayout.findWidgetT("SequencesTab/SequenceButton") as Button;
            this.playButton = ribbonLayout.findWidgetT("SequencesTab/PlayButton") as Button;
            this.stopButton = ribbonLayout.findWidgetT("SequencesTab/StopButton") as Button;
            this.nowPlayingLabel = ribbonLayout.findWidgetT("SequencesTab/NowPlaying") as StaticText;

            sequenceMenu = ribbonLayout.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "SequencesMenu") as PopupMenu;
            sequenceMenu.Visible = false;

            sequencesButton.MouseButtonClick += new MyGUIEvent(sequencesButton_MouseButtonClick);
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);
            stopButton.MouseButtonClick += new MyGUIEvent(stopButton_MouseButtonClick);

            sequenceController.CurrentSequenceSetChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceSetChanged);
            sequenceController.PlaybackStarted += new MovementSequenceEvent(sequenceController_PlaybackStarted);
            sequenceController.PlaybackStopped += new MovementSequenceEvent(sequenceController_PlaybackStopped);
            sequenceController.CurrentSequenceChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceChanged);
        }

        void sequenceController_CurrentSequenceSetChanged(MovementSequenceController controller)
        {
            MovementSequenceSet currentSet = controller.SequenceSet;
            sequenceMenu.removeAllItems();
            foreach (MovementSequenceGroup sequenceGroup in currentSet.Groups)
            {
                MenuItem groupItem = sequenceMenu.addItem(sequenceGroup.Name, MenuItemType.Popup);
                //groupItem.Image = Resources.SequenceIconLarge;
                MenuCtrl groupItemChild = groupItem.createItemChild();
                foreach (MovementSequenceInfo sequenceInfo in sequenceGroup.Sequences)
                {
                    MenuItem sequenceItem = groupItemChild.addItem(sequenceInfo.Name, MenuItemType.Normal);
                    sequenceItem.MouseButtonClick += sequenceItem_Click;
                    sequenceItem.UserObject = sequenceInfo.FileName;
                    //if (sequenceInfo.Thumbnail == null)
                    //{
                    //    sequenceItem.Image = Resources.SequenceIconLarge;
                    //}
                    //else
                    //{
                    //    sequenceItem.Image = sequenceInfo.Thumbnail;
                    //}
                }
            }
        }

        void sequenceItem_Click(Widget sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {
                MovementSequence sequence = sequenceController.loadSequence(item.UserObject.ToString());
                sequenceController.stopPlayback();
                sequenceController.CurrentSequence = sequence;
                sequenceController.playCurrentSequence();
            }
            sequenceMenu.setVisibleSmooth(false);
        }

        void sequencesButton_MouseButtonClick(Widget source, EventArgs e)
        {
            sequenceMenu.setVisibleSmooth(true);
            LayerManager.Instance.upLayerItem(sequenceMenu);
            sequenceMenu.setPosition(sequencesButton.getAbsoluteLeft(), sequencesButton.getAbsoluteTop() + sequencesButton.getHeight());
        }

        void stopButton_MouseButtonClick(Widget source, EventArgs e)
        {
            sequenceController.stopPlayback();
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            sequenceController.playCurrentSequence();
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
                nowPlayingLabel.Caption = "None";
                playButton.Enabled = false;
                stopButton.Enabled = false;
            }
            else
            {
                nowPlayingLabel.Caption = controller.CurrentSequence.Name;
                playButton.Enabled = true;
                stopButton.Enabled = false;
            }
        }
    }
}
